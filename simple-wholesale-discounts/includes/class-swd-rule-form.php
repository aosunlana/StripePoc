<?php
/**
 * Rule Form Handler
 *
 * Processes form submissions for both adding and editing discount rules.
 * Handles server-side validation and database persistence.
 *
 * @package SimpleWholesaleDiscounts
 */

if ( ! defined( 'ABSPATH' ) ) {
	exit;
}

/**
 * Class SWD_Rule_Form
 */
class SWD_Rule_Form {

	/** @var array Validation errors encountered during form submission. */
	public $errors = array();

	/** @var object|null The rule currently being edited (null for new rules). */
	public $rule = null;

	/**
	 * Constructor — load rule data if editing.
	 */
	public function __construct() {
		// phpcs:ignore WordPress.Security.NonceVerification
		$rule_id = isset( $_GET['rule_id'] ) ? absint( $_GET['rule_id'] ) : 0;

		if ( $rule_id > 0 ) {
			$this->rule = SWD_Discount::get_rule( $rule_id );
		}
	}

	/**
	 * Process the form submission.
	 * Called before the template is output so redirects work.
	 */
	public function handle_submit() {
		if ( ! isset( $_POST['swd_rule_nonce'] ) ) {
			return; // Not a form submission.
		}

		// Verify nonce.
		if ( ! wp_verify_nonce( sanitize_key( wp_unslash( $_POST['swd_rule_nonce'] ) ), 'swd_save_rule' ) ) {
			wp_die( esc_html__( 'Nonce verification failed.', 'simple-wholesale-discounts' ) );
		}

		// Permission check.
		if ( ! current_user_can( 'manage_woocommerce' ) ) {
			wp_die( esc_html__( 'You do not have permission to do this.', 'simple-wholesale-discounts' ) );
		}

		$rule_id = isset( $_POST['rule_id'] ) ? absint( $_POST['rule_id'] ) : 0;

		// Sanitize all inputs.
		$data = $this->sanitize_form_data( $_POST );

		// Validate.
		$this->validate( $data );

		if ( ! empty( $this->errors ) ) {
			// Validation failed — form will re-render with errors.
			return;
		}

		// Save to database.
		$saved_id = SWD_Discount::save_rule( $data, $rule_id );

		if ( false === $saved_id ) {
			$this->errors['general'] = __( 'Failed to save the rule. Please try again.', 'simple-wholesale-discounts' );
			return;
		}

		// Success — redirect to rules list with notice.
		$message = $rule_id > 0 ? 'updated' : 'created';
		wp_safe_redirect( add_query_arg( 'message', $message, SWD_Admin::rules_page_url() ) );
		exit;
	}

	/**
	 * Sanitize raw POST data into a clean array.
	 *
	 * @param array $post Raw $_POST data.
	 * @return array Sanitized data.
	 */
	private function sanitize_form_data( $post ) {
		// Product IDs come as a comma-separated string of IDs.
		$product_ids = array();
		if ( ! empty( $post['product_ids'] ) ) {
			$raw_ids     = sanitize_text_field( wp_unslash( $post['product_ids'] ) );
			$product_ids = array_filter( array_map( 'absint', explode( ',', $raw_ids ) ) );
		}

		// Category IDs come as a regular array from checkboxes.
		$category_ids = array();
		if ( ! empty( $post['category_ids'] ) && is_array( $post['category_ids'] ) ) {
			$category_ids = array_filter( array_map( 'absint', $post['category_ids'] ) );
		}

		return array(
			'rule_name'      => sanitize_text_field( wp_unslash( $post['rule_name'] ?? '' ) ),
			'discount_type'  => sanitize_key( $post['discount_type'] ?? 'percentage' ),
			'discount_value' => floatval( $post['discount_value'] ?? 0 ),
			'apply_to'       => sanitize_key( $post['apply_to'] ?? 'all' ),
			'product_ids'    => array_values( $product_ids ),
			'category_ids'   => array_values( $category_ids ),
			'min_quantity'   => absint( $post['min_quantity'] ?? 0 ),
			'max_quantity'   => absint( $post['max_quantity'] ?? 0 ),
			'is_active'      => isset( $post['is_active'] ) ? 1 : 0,
		);
	}

	/**
	 * Validate form data and populate $this->errors.
	 *
	 * @param array $data Sanitized form data.
	 */
	private function validate( $data ) {
		if ( empty( $data['rule_name'] ) ) {
			$this->errors['rule_name'] = __( 'Rule name is required.', 'simple-wholesale-discounts' );
		}

		if ( ! in_array( $data['discount_type'], array( 'percentage', 'flat' ), true ) ) {
			$this->errors['discount_type'] = __( 'Please select a discount type.', 'simple-wholesale-discounts' );
		}

		if ( $data['discount_value'] <= 0 ) {
			$this->errors['discount_value'] = __( 'Discount value must be greater than zero.', 'simple-wholesale-discounts' );
		}

		if ( 'percentage' === $data['discount_type'] && $data['discount_value'] > 100 ) {
			$this->errors['discount_value'] = __( 'Percentage discount cannot exceed 100%.', 'simple-wholesale-discounts' );
		}

		if ( ! in_array( $data['apply_to'], array( 'all', 'products', 'categories' ), true ) ) {
			$this->errors['apply_to'] = __( 'Please select what this rule applies to.', 'simple-wholesale-discounts' );
		}

		if ( 'products' === $data['apply_to'] && empty( $data['product_ids'] ) ) {
			$this->errors['product_ids'] = __( 'Please select at least one product.', 'simple-wholesale-discounts' );
		}

		if ( 'categories' === $data['apply_to'] && empty( $data['category_ids'] ) ) {
			$this->errors['category_ids'] = __( 'Please select at least one category.', 'simple-wholesale-discounts' );
		}

		if ( $data['max_quantity'] > 0 && $data['min_quantity'] > 0 && $data['max_quantity'] < $data['min_quantity'] ) {
			$this->errors['max_quantity'] = __( 'Maximum quantity must be greater than or equal to the minimum quantity.', 'simple-wholesale-discounts' );
		}
	}

	// ---------------------------------------------------------------------------
	// Template Helpers — called from templates/admin/rule-form.php
	// ---------------------------------------------------------------------------

	/**
	 * Get a field value.
	 * Prefers POST data (when re-rendering after failed validation),
	 * then existing rule data, then the given default.
	 *
	 * @param string $field   Field name.
	 * @param mixed  $default Default value if field has no data.
	 * @return mixed
	 */
	public function field( $field, $default = '' ) {
		// After a failed form submission, re-use what the user entered.
		if ( ! empty( $_POST ) && isset( $_POST['swd_rule_nonce'] ) ) { // phpcs:ignore WordPress.Security.NonceVerification
			if ( 'product_ids' === $field ) {
				$raw = sanitize_text_field( wp_unslash( $_POST['product_ids'] ?? '' ) );
				return array_filter( array_map( 'absint', explode( ',', $raw ) ) );
			}
			if ( 'category_ids' === $field ) {
				return isset( $_POST['category_ids'] ) ? array_map( 'absint', (array) $_POST['category_ids'] ) : array();
			}
			if ( isset( $_POST[ $field ] ) ) {
				return sanitize_text_field( wp_unslash( $_POST[ $field ] ) );
			}
			if ( 'is_active' === $field ) {
				return isset( $_POST['is_active'] ) ? 1 : 0;
			}
		}

		// Editing existing rule.
		if ( $this->rule ) {
			if ( 'product_ids' === $field ) {
				$decoded = json_decode( $this->rule->product_ids, true );
				return is_array( $decoded ) ? $decoded : array();
			}
			if ( 'category_ids' === $field ) {
				$decoded = json_decode( $this->rule->category_ids, true );
				return is_array( $decoded ) ? $decoded : array();
			}
			if ( isset( $this->rule->$field ) ) {
				return $this->rule->$field;
			}
		}

		return $default;
	}

	/**
	 * Check whether the form has a specific error.
	 *
	 * @param string $field Field key.
	 * @return bool
	 */
	public function has_error( $field ) {
		return isset( $this->errors[ $field ] );
	}

	/**
	 * Output inline error message HTML for a field.
	 *
	 * @param string $field Field key.
	 */
	public function error_html( $field ) {
		if ( $this->has_error( $field ) ) {
			echo '<span class="swd-field-error">' . esc_html( $this->errors[ $field ] ) . '</span>';
		}
	}

	/**
	 * Return the page title for the form.
	 *
	 * @return string
	 */
	public function page_title() {
		return $this->rule
			? __( 'Edit Discount Rule', 'simple-wholesale-discounts' )
			: __( 'Add Discount Rule', 'simple-wholesale-discounts' );
	}

	/**
	 * Get all WooCommerce product categories as a nested array
	 * for the category checkbox list.
	 *
	 * @return array
	 */
	public function get_categories_nested() {
		$terms = get_terms(
			array(
				'taxonomy'   => 'product_cat',
				'hide_empty' => false,
				'orderby'    => 'name',
			)
		);

		if ( is_wp_error( $terms ) || empty( $terms ) ) {
			return array();
		}

		return $this->build_category_tree( $terms, 0 );
	}

	/**
	 * Recursively build nested category tree from flat terms array.
	 *
	 * @param array $terms  All WP Term objects.
	 * @param int   $parent_id Parent term ID.
	 * @param int   $depth  Current depth for indentation.
	 * @return array
	 */
	private function build_category_tree( $terms, $parent_id = 0, $depth = 0 ) {
		$output = array();
		foreach ( $terms as $term ) {
			if ( (int) $term->parent === (int) $parent_id ) {
				$term->depth    = $depth;
				$term->children = $this->build_category_tree( $terms, $term->term_id, $depth + 1 );
				$output[]       = $term;
			}
		}
		return $output;
	}

	/**
	 * Get selected product objects for displaying as tags in the form.
	 * Runs product queries only when editing an existing rule.
	 *
	 * @return array Array of arrays with id, name, price.
	 */
	public function get_selected_products() {
		$ids      = $this->field( 'product_ids', array() );
		$products = array();

		foreach ( (array) $ids as $pid ) {
			$product = wc_get_product( $pid );
			if ( $product ) {
				$products[] = array(
					'id'    => $product->get_id(),
					'name'  => $product->get_name(),
					'price' => wc_price( $product->get_price() ),
				);
			}
		}

		return $products;
	}
}
