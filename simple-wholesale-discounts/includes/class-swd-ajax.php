<?php
/**
 * AJAX Handlers
 *
 * Registers and handles all wp_ajax_ actions for the plugin.
 *
 * Endpoints:
 *  - swd_search_products       : Product search in the rule form.
 *  - swd_toggle_rule_status    : Toggle rule active/inactive from the list table.
 *
 * @package SimpleWholesaleDiscounts
 */

if ( ! defined( 'ABSPATH' ) ) {
	exit;
}

/**
 * Class SWD_Ajax
 */
class SWD_Ajax {

	/** @var SWD_Ajax|null Singleton. */
	private static $instance = null;

	/**
	 * Get or create the singleton.
	 *
	 * @return SWD_Ajax
	 */
	public static function instance() {
		if ( null === self::$instance ) {
			self::$instance = new self();
		}
		return self::$instance;
	}

	/**
	 * Constructor — register AJAX actions.
	 * Both actions are admin-only (logged-in users required).
	 */
	private function __construct() {
		add_action( 'wp_ajax_swd_search_products',    array( $this, 'search_products' ) );
		add_action( 'wp_ajax_swd_toggle_rule_status', array( $this, 'toggle_rule_status' ) );
	}

	// ---------------------------------------------------------------------------
	// Endpoint: swd_search_products
	// ---------------------------------------------------------------------------

	/**
	 * Product search AJAX handler.
	 *
	 * Accepts: term (string), nonce (string)
	 * Returns JSON: { success: bool, data: [ { id, name, price, thumbnail_url } ] }
	 */
	public function search_products() {
		// Nonce verification.
		if ( ! check_ajax_referer( 'swd_search_products', 'nonce', false ) ) {
			wp_send_json_error( array( 'message' => __( 'Nonce verification failed.', 'simple-wholesale-discounts' ) ), 403 );
		}

		// Permission check.
		if ( ! current_user_can( 'manage_woocommerce' ) ) {
			wp_send_json_error( array( 'message' => __( 'Insufficient permissions.', 'simple-wholesale-discounts' ) ), 403 );
		}

		$term = isset( $_POST['term'] ) ? sanitize_text_field( wp_unslash( $_POST['term'] ) ) : '';

		if ( empty( $term ) || strlen( $term ) < 2 ) {
			wp_send_json_success( array() );
		}

		// Use WooCommerce product search.
		$args = array(
			'post_type'      => array( 'product', 'product_variation' ),
			'post_status'    => 'publish',
			'posts_per_page' => 15,
			's'              => $term,
		);

		$query    = new WP_Query( $args );
		$products = array();

		foreach ( $query->posts as $post ) {
			$product = wc_get_product( $post->ID );
			if ( ! $product ) {
				continue;
			}

			// Get thumbnail.
			$thumb_id  = $product->get_image_id();
			$thumb_url = $thumb_id ? wp_get_attachment_image_url( $thumb_id, 'thumbnail' ) : wc_placeholder_img_src( 'thumbnail' );

			$products[] = array(
				'id'            => $product->get_id(),
				'name'          => $product->get_name(),
				'price'         => wc_price( $product->get_price() ),
				'thumbnail_url' => esc_url( (string) $thumb_url ),
			);
		}

		wp_send_json_success( $products );
	}

	// ---------------------------------------------------------------------------
	// Endpoint: swd_toggle_rule_status
	// ---------------------------------------------------------------------------

	/**
	 * Toggle rule active/inactive AJAX handler.
	 *
	 * Accepts: rule_id (int), new_status (int 0|1), nonce (string)
	 * Returns JSON: { success: bool, data: { new_status: int, label: string } }
	 */
	public function toggle_rule_status() {
		// Nonce verification.
		if ( ! check_ajax_referer( 'swd_toggle_rule_status', 'nonce', false ) ) {
			wp_send_json_error( array( 'message' => __( 'Nonce verification failed.', 'simple-wholesale-discounts' ) ), 403 );
		}

		// Permission check.
		if ( ! current_user_can( 'manage_woocommerce' ) ) {
			wp_send_json_error( array( 'message' => __( 'Insufficient permissions.', 'simple-wholesale-discounts' ) ), 403 );
		}

		$rule_id    = isset( $_POST['rule_id'] ) ? absint( $_POST['rule_id'] ) : 0;
		$new_status = isset( $_POST['new_status'] ) ? ( absint( $_POST['new_status'] ) ? 1 : 0 ) : 0;

		if ( ! $rule_id ) {
			wp_send_json_error( array( 'message' => __( 'Invalid rule ID.', 'simple-wholesale-discounts' ) ), 400 );
		}

		$success = SWD_Discount::toggle_status( $rule_id, $new_status );

		if ( ! $success ) {
			wp_send_json_error( array( 'message' => __( 'Failed to update rule status.', 'simple-wholesale-discounts' ) ), 500 );
		}

		wp_send_json_success(
			array(
				'new_status' => $new_status,
				'label'      => $new_status ? __( 'Active', 'simple-wholesale-discounts' ) : __( 'Inactive', 'simple-wholesale-discounts' ),
			)
		);
	}
}
