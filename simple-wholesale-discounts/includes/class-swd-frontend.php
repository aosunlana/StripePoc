<?php
/**
 * Frontend Output
 *
 * Renders all customer-facing output:
 * - Discount badge on single product pages.
 * - Tiered pricing table on single product pages.
 * - Savings message on cart page.
 * - Discount line item on cart and checkout.
 *
 * @package SimpleWholesaleDiscounts
 */

if ( ! defined( 'ABSPATH' ) ) {
	exit;
}

/**
 * Class SWD_Frontend
 */
class SWD_Frontend {

	/** @var SWD_Frontend|null Singleton instance. */
	private static $instance = null;

	/**
	 * Get or create the singleton.
	 *
	 * @return SWD_Frontend
	 */
	public static function instance() {
		if ( null === self::$instance ) {
			self::$instance = new self();
		}
		return self::$instance;
	}

	/**
	 * Constructor — wire up frontend hooks.
	 */
	private function __construct() {
		// Enqueue frontend assets.
		add_action( 'wp_enqueue_scripts', array( $this, 'enqueue_frontend_assets' ) );

		// Discount badge and pricing table on single product pages.
		add_action( 'woocommerce_single_product_summary', array( $this, 'render_discount_badge' ), 25 );
		add_action( 'woocommerce_after_add_to_cart_form', array( $this, 'render_pricing_table' ) );

		// Savings message above cart totals.
		add_action( 'woocommerce_before_cart_totals', array( $this, 'render_savings_message' ) );
		add_action( 'woocommerce_review_order_before_order_total', array( $this, 'render_savings_message' ) );

		// Discount line item in cart / checkout totals.
		add_action( 'woocommerce_cart_totals_after_order_total', array( $this, 'render_discount_line' ) );
		add_action( 'woocommerce_review_order_after_order_total', array( $this, 'render_discount_line' ) );
	}

	// ---------------------------------------------------------------------------
	// Asset Enqueueing
	// ---------------------------------------------------------------------------

	/**
	 * Enqueue frontend CSS and JS only on relevant pages.
	 */
	public function enqueue_frontend_assets() {
		// Only load on product pages, cart, and checkout.
		if ( ! is_product() && ! is_cart() && ! is_checkout() ) {
			return;
		}

		$settings = get_option( 'swd_settings', array() );

		// Don't load anything if plugin is disabled.
		if ( empty( $settings['enable_plugin'] ) ) {
			return;
		}

		wp_enqueue_style(
			'swd-frontend',
			SWD_PLUGIN_URL . 'assets/css/frontend.css',
			array(),
			SWD_VERSION
		);

		// Frontend JS only on product pages (for any interactive elements).
		if ( is_product() ) {
			wp_enqueue_script(
				'swd-frontend',
				SWD_PLUGIN_URL . 'assets/js/frontend.js',
				array(), // No jQuery required.
				SWD_VERSION,
				true // Footer.
			);
		}
	}

	// ---------------------------------------------------------------------------
	// Single Product Page Hooks
	// ---------------------------------------------------------------------------

	/**
	 * Render the wholesale discount badge below the product price.
	 */
	public function render_discount_badge() {
		$settings = get_option( 'swd_settings', array() );

		if ( empty( $settings['enable_plugin'] ) || empty( $settings['show_discount_badge'] ) ) {
			return;
		}

		global $product;
		if ( ! $product ) {
			return;
		}

		$product_id = $product->get_id();
		$rules      = SWD_Discount::instance()->get_rules_for_product( $product_id );

		if ( empty( $rules ) ) {
			return;
		}

		$badge_text = ! empty( $settings['discount_badge_text'] )
			? $settings['discount_badge_text']
			: __( 'Buy in bulk and save! Wholesale pricing available.', 'simple-wholesale-discounts' );

		include SWD_PLUGIN_DIR . 'templates/frontend/discount-badge.php';
	}

	/**
	 * Render the tiered pricing table below the add to cart form.
	 * Only displayed when the product has multiple quantity-based rules.
	 */
	public function render_pricing_table() {
		$settings = get_option( 'swd_settings', array() );

		if ( empty( $settings['enable_plugin'] ) ) {
			return;
		}

		global $product;
		if ( ! $product ) {
			return;
		}

		$product_id = $product->get_id();
		$rules      = SWD_Discount::instance()->get_rules_for_product( $product_id );

		if ( empty( $rules ) ) {
			return;
		}

		$base_price = (float) $product->get_price();
		$rows       = $this->build_pricing_rows( $rules, $base_price );

		if ( empty( $rows ) ) {
			return;
		}

		include SWD_PLUGIN_DIR . 'templates/frontend/pricing-table.php';
	}

	// ---------------------------------------------------------------------------
	// Cart / Checkout Hooks
	// ---------------------------------------------------------------------------

	/**
	 * Render the savings message above cart/checkout totals.
	 */
	public function render_savings_message() {
		$settings = get_option( 'swd_settings', array() );

		if ( empty( $settings['enable_plugin'] ) || empty( $settings['show_savings_message'] ) ) {
			return;
		}

		if ( ! function_exists( 'WC' ) || ! WC()->session ) {
			return;
		}

		$savings = (float) WC()->session->get( 'swd_total_savings', 0 );

		if ( $savings <= 0 ) {
			return;
		}

		$message_template = ! empty( $settings['savings_message_text'] )
			? $settings['savings_message_text']
			: __( "You're saving {amount} on this order!", 'simple-wholesale-discounts' );

		$formatted_amount = wc_price( $savings );
		$message          = str_replace( '{amount}', $formatted_amount, $message_template );

		include SWD_PLUGIN_DIR . 'templates/frontend/savings-message.php';
	}

	/**
	 * Render the wholesale discount line item in the cart/checkout totals.
	 */
	public function render_discount_line() {
		$settings = get_option( 'swd_settings', array() );

		if ( empty( $settings['enable_plugin'] ) ) {
			return;
		}

		if ( ! function_exists( 'WC' ) || ! WC()->session ) {
			return;
		}

		$savings = (float) WC()->session->get( 'swd_total_savings', 0 );

		if ( $savings <= 0 ) {
			return;
		}

		$label = ! empty( $settings['discount_label'] )
			? $settings['discount_label']
			: __( 'Wholesale Discount', 'simple-wholesale-discounts' );

		// Output a negative line item row, styled like WooCommerce totals rows.
		?>
		<tr class="swd-discount-row">
			<th><?php echo esc_html( $label ); ?></th>
			<td><strong class="swd-discount-amount">-<?php echo wp_kses_post( wc_price( $savings ) ); ?></strong></td>
		</tr>
		<?php
	}

	// ---------------------------------------------------------------------------
	// Helpers
	// ---------------------------------------------------------------------------

	/**
	 * Build pricing table rows from rules for a given base price.
	 *
	 * Sorts rules by min_quantity so the table displays in ascending order.
	 *
	 * @param array $rules      Active rules for the product.
	 * @param float $base_price Product base price.
	 * @return array Array of row arrays with quantity_label, discount_label, price.
	 */
	private function build_pricing_rows( $rules, $base_price ) {
		// Sort by min_quantity ascending.
		usort(
			$rules,
			function ( $a, $b ) {
				return (int) $a->min_quantity - (int) $b->min_quantity;
			}
		);

		$rows = array();

		// Always add a "No discount" row for the base price (qty < first rule min).
		$first_rule_min = isset( $rules[0] ) ? (int) $rules[0]->min_quantity : 0;
		if ( $first_rule_min > 1 ) {
			$rows[] = array(
				'quantity_label' => '1 – ' . ( $first_rule_min - 1 ),
				'discount_label' => __( 'No discount', 'simple-wholesale-discounts' ),
				'price'          => wc_price( $base_price ),
				'is_discounted'  => false,
			);
		}

		foreach ( $rules as $rule ) {
			$min    = (int) $rule->min_quantity;
			$max    = (int) $rule->max_quantity;
			$saving = 0;

			if ( 'percentage' === $rule->discount_type ) {
				$pct    = min( 100, max( 0, (float) $rule->discount_value ) );
				$saving = $base_price * ( $pct / 100 );
				$label  = $rule->discount_value . '% ' . __( 'off', 'simple-wholesale-discounts' );
			} else {
				$saving = min( $base_price, max( 0, (float) $rule->discount_value ) );
				$label  = wc_price( $rule->discount_value ) . ' ' . __( 'off', 'simple-wholesale-discounts' );
			}

			$discounted_price = max( 0, $base_price - $saving );

			if ( $min > 0 && $max > 0 ) {
				$qty_label = $min . ' – ' . $max;
			} elseif ( $min > 0 ) {
				$qty_label = $min . '+';
			} else {
				$qty_label = __( 'Any quantity', 'simple-wholesale-discounts' );
			}

			$rows[] = array(
				'quantity_label' => $qty_label,
				'discount_label' => wp_kses_post( $label ),
				'price'          => wc_price( $discounted_price ),
				'is_discounted'  => true,
			);
		}

		return $rows;
	}
}
