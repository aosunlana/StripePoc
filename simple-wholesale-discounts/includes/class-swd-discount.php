<?php
/**
 * Discount Calculation Engine
 *
 * Hooks into WooCommerce's cart calculation cycle and applies matching
 * wholesale discount rules to cart item prices.
 *
 * Logic overview:
 * 1. Load all active rules once per request (cached in a property).
 * 2. On woocommerce_before_calculate_totals, iterate each cart item.
 * 3. For each item, find rules that:
 *    a) Apply to this product (all / specific product IDs / category IDs).
 *    b) Have a quantity range that includes the item quantity.
 * 4. Of the matching rules, pick the one with the highest discount value.
 * 5. Apply the discount: modify $cart_item['data']->set_price().
 * 6. Track total savings so the frontend can display them.
 *
 * @package SimpleWholesaleDiscounts
 */

if ( ! defined( 'ABSPATH' ) ) {
	exit;
}

/**
 * Class SWD_Discount
 */
class SWD_Discount {

	/** @var SWD_Discount|null Singleton instance. */
	private static $instance = null;

	/** @var array|null Cached active rules from the database. */
	private $rules_cache = null;

	/**
	 * Get or create the singleton instance.
	 *
	 * @return SWD_Discount
	 */
	public static function instance() {
		if ( null === self::$instance ) {
			self::$instance = new self();
		}
		return self::$instance;
	}

	/**
	 * Constructor — wire up WooCommerce hooks.
	 */
	private function __construct() {
		// Priority 20 to run after other plugins may have set prices.
		add_action( 'woocommerce_before_calculate_totals', array( $this, 'apply_discounts' ), 20, 1 );
	}

	// ---------------------------------------------------------------------------
	// Public API used by other classes
	// ---------------------------------------------------------------------------

	/**
	 * Get all active rules (with in-request caching).
	 *
	 * @return array Array of rule objects from the database.
	 */
	public function get_active_rules() {
		if ( null !== $this->rules_cache ) {
			return $this->rules_cache;
		}

		global $wpdb;
		$table = $wpdb->prefix . 'swd_rules';

		// phpcs:ignore WordPress.DB.DirectDatabaseQuery
		$rules = $wpdb->get_results(
			$wpdb->prepare(
				"SELECT * FROM {$table} WHERE is_active = %d ORDER BY discount_value DESC",
				1
			)
		);

		$this->rules_cache = $rules ? $rules : array();

		return $this->rules_cache;
	}

	/**
	 * Get all rules (active and inactive) — used by admin list table.
	 *
	 * @param array $args Optional query args: orderby, order, search.
	 * @return array
	 */
	public static function get_all_rules( $args = array() ) {
		global $wpdb;
		$table = $wpdb->prefix . 'swd_rules';

		$defaults = array(
			'orderby' => 'id',
			'order'   => 'ASC',
			'search'  => '',
		);
		$args = wp_parse_args( $args, $defaults );

		$allowed_orderby = array( 'id', 'rule_name', 'discount_type', 'discount_value', 'is_active', 'created_at' );
		$orderby = in_array( $args['orderby'], $allowed_orderby, true ) ? $args['orderby'] : 'id';
		$order   = 'DESC' === strtoupper( $args['order'] ) ? 'DESC' : 'ASC';

		if ( ! empty( $args['search'] ) ) {
			// phpcs:ignore WordPress.DB.DirectDatabaseQuery
			$rules = $wpdb->get_results(
				$wpdb->prepare(
					"SELECT * FROM {$table} WHERE rule_name LIKE %s ORDER BY {$orderby} {$order}",
					'%' . $wpdb->esc_like( $args['search'] ) . '%'
				)
			);
		} else {
			// phpcs:ignore WordPress.DB.DirectDatabaseQuery
			$rules = $wpdb->get_results(
				"SELECT * FROM {$table} ORDER BY {$orderby} {$order}" // phpcs:ignore WordPress.DB.PreparedSQL.NotPrepared
			);
		}

		return $rules ? $rules : array();
	}

	/**
	 * Get a single rule by ID.
	 *
	 * @param int $rule_id Rule ID.
	 * @return object|null Rule object or null if not found.
	 */
	public static function get_rule( $rule_id ) {
		global $wpdb;
		$table = $wpdb->prefix . 'swd_rules';

		// phpcs:ignore WordPress.DB.DirectDatabaseQuery
		return $wpdb->get_row(
			$wpdb->prepare(
				"SELECT * FROM {$table} WHERE id = %d",
				absint( $rule_id )
			)
		);
	}

	/**
	 * Save (insert or update) a rule.
	 *
	 * @param array $data Sanitized rule data.
	 * @param int   $rule_id 0 for a new rule, positive int for update.
	 * @return int|false The rule ID on success, false on failure.
	 */
	public static function save_rule( $data, $rule_id = 0 ) {
		global $wpdb;
		$table = $wpdb->prefix . 'swd_rules';
		$now   = current_time( 'mysql' );

		$record = array(
			'rule_name'      => sanitize_text_field( $data['rule_name'] ),
			'discount_type'  => in_array( $data['discount_type'], array( 'percentage', 'flat' ), true ) ? $data['discount_type'] : 'percentage',
			'discount_value' => max( 0, floatval( $data['discount_value'] ) ),
			'apply_to'       => in_array( $data['apply_to'], array( 'all', 'products', 'categories' ), true ) ? $data['apply_to'] : 'all',
			'product_ids'    => wp_json_encode( array_map( 'absint', (array) ( $data['product_ids'] ?? array() ) ) ),
			'category_ids'   => wp_json_encode( array_map( 'absint', (array) ( $data['category_ids'] ?? array() ) ) ),
			'min_quantity'   => absint( $data['min_quantity'] ?? 0 ),
			'max_quantity'   => absint( $data['max_quantity'] ?? 0 ),
			'is_active'      => isset( $data['is_active'] ) ? ( $data['is_active'] ? 1 : 0 ) : 1,
			'updated_at'     => $now,
		);

		if ( $rule_id > 0 ) {
			// Update existing rule.
			$result = $wpdb->update( // phpcs:ignore WordPress.DB.DirectDatabaseQuery
				$table,
				$record,
				array( 'id' => $rule_id ),
				array( '%s', '%s', '%f', '%s', '%s', '%s', '%d', '%d', '%d', '%s' ),
				array( '%d' )
			);
			return ( false !== $result ) ? $rule_id : false;
		} else {
			// Insert new rule.
			$record['created_at'] = $now;
			$result = $wpdb->insert( // phpcs:ignore WordPress.DB.DirectDatabaseQuery
				$table,
				$record,
				array( '%s', '%s', '%f', '%s', '%s', '%s', '%d', '%d', '%d', '%s', '%s' )
			);
			return ( false !== $result ) ? $wpdb->insert_id : false;
		}
	}

	/**
	 * Delete a rule by ID.
	 *
	 * @param int $rule_id Rule ID.
	 * @return bool
	 */
	public static function delete_rule( $rule_id ) {
		global $wpdb;
		$table = $wpdb->prefix . 'swd_rules';

		// phpcs:ignore WordPress.DB.DirectDatabaseQuery
		$result = $wpdb->delete(
			$table,
			array( 'id' => absint( $rule_id ) ),
			array( '%d' )
		);

		return false !== $result;
	}

	/**
	 * Toggle a rule's active status.
	 *
	 * @param int $rule_id   Rule ID.
	 * @param int $new_status 1 = active, 0 = inactive.
	 * @return bool
	 */
	public static function toggle_status( $rule_id, $new_status ) {
		global $wpdb;
		$table = $wpdb->prefix . 'swd_rules';

		// phpcs:ignore WordPress.DB.DirectDatabaseQuery
		$result = $wpdb->update(
			$table,
			array(
				'is_active'  => $new_status ? 1 : 0,
				'updated_at' => current_time( 'mysql' ),
			),
			array( 'id' => absint( $rule_id ) ),
			array( '%d', '%s' ),
			array( '%d' )
		);

		return false !== $result;
	}

	/**
	 * Get active rules that apply to a given product.
	 * Used by the frontend to build the pricing table.
	 *
	 * @param int $product_id WooCommerce product ID.
	 * @return array Matching rule objects.
	 */
	public function get_rules_for_product( $product_id ) {
		$all_rules       = $this->get_active_rules();
		$product         = wc_get_product( $product_id );
		$category_ids    = $product ? wp_get_post_terms( $product_id, 'product_cat', array( 'fields' => 'ids' ) ) : array();
		$matching        = array();

		foreach ( $all_rules as $rule ) {
			if ( $this->rule_applies_to_product( $rule, $product_id, $category_ids ) ) {
				$matching[] = $rule;
			}
		}

		return $matching;
	}

	// ---------------------------------------------------------------------------
	// Core Discount Application
	// ---------------------------------------------------------------------------

	/**
	 * Main hook callback — apply discount rules to every cart item.
	 *
	 * @param WC_Cart $cart WooCommerce cart object.
	 */
	public function apply_discounts( $cart ) {
		// Safety checks.
		if ( is_admin() && ! defined( 'DOING_AJAX' ) ) {
			return;
		}
		if ( did_action( 'woocommerce_before_calculate_totals' ) > 1 ) {
			return;
		}

		// Check master enable toggle from settings.
		$settings = get_option( 'swd_settings', array() );
		if ( empty( $settings['enable_plugin'] ) ) {
			return;
		}

		$active_rules = $this->get_active_rules();
		if ( empty( $active_rules ) ) {
			return;
		}

		// Track total savings for the savings message display.
		$total_savings = 0.0;

		foreach ( $cart->get_cart() as $cart_item_key => $cart_item ) {
			/** @var WC_Product $product */
			$product    = $cart_item['data'];
			$product_id = $cart_item['product_id'];
			$quantity   = $cart_item['quantity'];

			// Get the current price (could already be modified by another plugin).
			$original_price = (float) $product->get_price();
			if ( $original_price <= 0 ) {
				continue;
			}

			// Get product categories once per item.
			$category_ids = wp_get_post_terms( $product_id, 'product_cat', array( 'fields' => 'ids' ) );

			// Find the best matching rule.
			$best_rule    = null;
			$best_savings = 0.0;

			foreach ( $active_rules as $rule ) {
				// 1. Does this rule target this product?
				if ( ! $this->rule_applies_to_product( $rule, $product_id, $category_ids ) ) {
					continue;
				}

				// 2. Is the quantity within the rule's range?
				if ( ! $this->quantity_in_range( $quantity, (int) $rule->min_quantity, (int) $rule->max_quantity ) ) {
					continue;
				}

				// 3. Calculate the saving this rule gives per unit.
				$saving = $this->calculate_saving( $original_price, $rule );

				// Keep the rule with the highest saving (most beneficial to customer).
				if ( $saving > $best_savings ) {
					$best_savings = $saving;
					$best_rule    = $rule;
				}
			}

			if ( null === $best_rule ) {
				continue;
			}

			// Apply the discount — clamp to 0 so price never goes negative.
			$discounted_price = max( 0, $original_price - $best_savings );
			$product->set_price( $discounted_price );

			// Accumulate total savings (per unit * quantity).
			$total_savings += $best_savings * $quantity;
		}

		// Store total savings in session for the frontend savings message.
		if ( function_exists( 'WC' ) && WC()->session ) {
			WC()->session->set( 'swd_total_savings', $total_savings );
		}
	}

	// ---------------------------------------------------------------------------
	// Private Helpers
	// ---------------------------------------------------------------------------

	/**
	 * Check whether a rule applies to a specific product.
	 *
	 * @param object $rule        Rule database row.
	 * @param int    $product_id  WooCommerce product ID.
	 * @param array  $category_ids Array of category IDs the product belongs to.
	 * @return bool
	 */
	private function rule_applies_to_product( $rule, $product_id, $category_ids ) {
		switch ( $rule->apply_to ) {
			case 'all':
				return true;

			case 'products':
				$ids = json_decode( $rule->product_ids, true );
				if ( ! is_array( $ids ) || empty( $ids ) ) {
					return false;
				}
				return in_array( (int) $product_id, array_map( 'intval', $ids ), true );

			case 'categories':
				$ids = json_decode( $rule->category_ids, true );
				if ( ! is_array( $ids ) || empty( $ids ) ) {
					return false;
				}
				$rule_cats = array_map( 'intval', $ids );
				foreach ( (array) $category_ids as $cat_id ) {
					if ( in_array( (int) $cat_id, $rule_cats, true ) ) {
						return true;
					}
				}
				return false;

			default:
				return false;
		}
	}

	/**
	 * Check whether a quantity falls within a rule's min/max range.
	 *
	 * @param int $quantity     Cart item quantity.
	 * @param int $min_quantity Rule minimum (0 = no minimum).
	 * @param int $max_quantity Rule maximum (0 = no maximum).
	 * @return bool
	 */
	private function quantity_in_range( $quantity, $min_quantity, $max_quantity ) {
		if ( $min_quantity > 0 && $quantity < $min_quantity ) {
			return false;
		}
		if ( $max_quantity > 0 && $quantity > $max_quantity ) {
			return false;
		}
		return true;
	}

	/**
	 * Calculate per-unit saving for a given rule and price.
	 *
	 * @param float  $price Original unit price.
	 * @param object $rule  Rule database row.
	 * @return float Saving per unit.
	 */
	private function calculate_saving( $price, $rule ) {
		$value = (float) $rule->discount_value;

		if ( 'percentage' === $rule->discount_type ) {
			// Cap percentage at 100%.
			$pct = min( 100, max( 0, $value ) );
			return $price * ( $pct / 100 );
		} else {
			// Flat discount — cannot exceed item price.
			return min( $price, max( 0, $value ) );
		}
	}

	/**
	 * Format discount value as a human-readable string.
	 *
	 * @param object $rule Rule database row.
	 * @return string e.g. "10%" or "$5.00 flat"
	 */
	public static function format_discount( $rule ) {
		if ( 'percentage' === $rule->discount_type ) {
			return esc_html( $rule->discount_value ) . '%';
		}
		return wc_price( $rule->discount_value ) . ' ' . esc_html__( 'flat', 'simple-wholesale-discounts' );
	}
}
