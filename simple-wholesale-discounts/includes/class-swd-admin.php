<?php
/**
 * Admin Class
 *
 * Registers the WooCommerce submenu pages and enqueues admin assets.
 *
 * @package SimpleWholesaleDiscounts
 */

if ( ! defined( 'ABSPATH' ) ) {
	exit;
}

/**
 * Class SWD_Admin
 */
class SWD_Admin {

	/** @var SWD_Admin|null Singleton instance. */
	private static $instance = null;

	/**
	 * Get or create the singleton instance.
	 *
	 * @return SWD_Admin
	 */
	public static function instance() {
		if ( null === self::$instance ) {
			self::$instance = new self();
		}
		return self::$instance;
	}

	/**
	 * Constructor — wire up WordPress hooks.
	 */
	private function __construct() {
		add_action( 'admin_menu', array( $this, 'register_menus' ) );
		add_action( 'admin_enqueue_scripts', array( $this, 'enqueue_assets' ) );
	}

	/**
	 * Register the "Wholesale Discounts" submenu under WooCommerce.
	 */
	public function register_menus() {
		// Parent slug for WooCommerce admin pages.
		$parent_slug = 'woocommerce';

		// Default page — Discount Rules list.
		add_submenu_page(
			$parent_slug,
			__( 'Wholesale Discounts', 'simple-wholesale-discounts' ),
			__( 'Wholesale Discounts', 'simple-wholesale-discounts' ),
			'manage_woocommerce',
			'swd-rules',
			array( $this, 'render_rules_page' )
		);

		// Add/Edit Rule sub-page (hidden from nav but reachable via URL).
		add_submenu_page(
			$parent_slug,
			__( 'Add / Edit Rule', 'simple-wholesale-discounts' ),
			'', // Empty string hides it from the sidebar menu.
			'manage_woocommerce',
			'swd-rule-form',
			array( $this, 'render_rule_form_page' )
		);

		// Settings sub-page.
		add_submenu_page(
			$parent_slug,
			__( 'Wholesale Discount Settings', 'simple-wholesale-discounts' ),
			__( 'WD Settings', 'simple-wholesale-discounts' ),
			'manage_woocommerce',
			'swd-settings',
			array( $this, 'render_settings_page' )
		);
	}

	/**
	 * Enqueue admin CSS and JS only on our plugin pages.
	 *
	 * @param string $hook_suffix Current admin page hook suffix.
	 */
	public function enqueue_assets( $hook_suffix ) {
		// Our pages are registered under WooCommerce so their hooks
		// follow the pattern woocommerce_page_swd-*.
		$swd_pages = array(
			'woocommerce_page_swd-rules',
			'woocommerce_page_swd-rule-form',
			'woocommerce_page_swd-settings',
		);

		if ( ! in_array( $hook_suffix, $swd_pages, true ) ) {
			return;
		}

		// Admin stylesheet.
		wp_enqueue_style(
			'swd-admin',
			SWD_PLUGIN_URL . 'assets/css/admin.css',
			array( 'woocommerce_admin_styles' ),
			SWD_VERSION
		);

		// Admin JavaScript (module handles its own internal dependencies).
		wp_enqueue_script(
			'swd-admin',
			SWD_PLUGIN_URL . 'assets/js/admin.js',
			array( 'jquery', 'wp-util' ),
			SWD_VERSION,
			true
		);

		// Pass data from PHP to JavaScript via wp_localize_script.
		wp_localize_script(
			'swd-admin',
			'swdAdmin',
			array(
				'ajaxUrl'           => admin_url( 'admin-ajax.php' ),
				'nonce'             => wp_create_nonce( 'swd_admin_nonce' ),
				'searchNonce'       => wp_create_nonce( 'swd_search_products' ),
				'toggleNonce'       => wp_create_nonce( 'swd_toggle_rule_status' ),
				'currencySymbol'    => get_woocommerce_currency_symbol(),
				'i18n'              => array(
					'searching'         => __( 'Searching...', 'simple-wholesale-discounts' ),
					'noResults'         => __( 'No products found.', 'simple-wholesale-discounts' ),
					'typeToSearch'      => __( 'Type to search products...', 'simple-wholesale-discounts' ),
					'confirmDelete'     => __( 'Are you sure you want to delete this rule? This action cannot be undone.', 'simple-wholesale-discounts' ),
					'confirmBulkDelete' => __( 'Are you sure you want to delete the selected rules?', 'simple-wholesale-discounts' ),
					'activating'        => __( 'Activating...', 'simple-wholesale-discounts' ),
					'deactivating'      => __( 'Deactivating...', 'simple-wholesale-discounts' ),
				),
			)
		);

		// Select2 for enhanced dropdowns (WC ships it).
		wp_enqueue_style( 'select2' );
		wp_enqueue_script( 'selectWoo' );
	}

	// ---------------------------------------------------------------------------
	// Page Renderers
	// ---------------------------------------------------------------------------

	/**
	 * Render the Discount Rules list page.
	 */
	public function render_rules_page() {
		if ( ! current_user_can( 'manage_woocommerce' ) ) {
			wp_die( esc_html__( 'You do not have sufficient permissions to access this page.', 'simple-wholesale-discounts' ) );
		}

		// Instantiate the list table and process bulk actions.
		require_once SWD_PLUGIN_DIR . 'includes/class-swd-list-table.php';
		$list_table = new SWD_Rules_List_Table();
		$list_table->process_bulk_action();
		$list_table->prepare_items();

		// Load the template.
		include SWD_PLUGIN_DIR . 'templates/admin/rules-list.php';
	}

	/**
	 * Render the Add / Edit Rule form page.
	 */
	public function render_rule_form_page() {
		if ( ! current_user_can( 'manage_woocommerce' ) ) {
			wp_die( esc_html__( 'You do not have sufficient permissions to access this page.', 'simple-wholesale-discounts' ) );
		}

		require_once SWD_PLUGIN_DIR . 'includes/class-swd-rule-form.php';
		$form = new SWD_Rule_Form();
		$form->handle_submit();

		include SWD_PLUGIN_DIR . 'templates/admin/rule-form.php';
	}

	/**
	 * Render the Settings page.
	 */
	public function render_settings_page() {
		if ( ! current_user_can( 'manage_woocommerce' ) ) {
			wp_die( esc_html__( 'You do not have sufficient permissions to access this page.', 'simple-wholesale-discounts' ) );
		}

		require_once SWD_PLUGIN_DIR . 'includes/class-swd-settings.php';
		$settings = new SWD_Settings();
		$settings->render();
	}

	// ---------------------------------------------------------------------------
	// Static Helpers (used by templates)
	// ---------------------------------------------------------------------------

	/**
	 * Build the URL for the rules list page.
	 *
	 * @return string
	 */
	public static function rules_page_url() {
		return admin_url( 'admin.php?page=swd-rules' );
	}

	/**
	 * Build the URL for the Add Rule page.
	 *
	 * @return string
	 */
	public static function add_rule_url() {
		return admin_url( 'admin.php?page=swd-rule-form' );
	}

	/**
	 * Build the URL to edit a specific rule.
	 *
	 * @param int $rule_id Rule ID.
	 * @return string
	 */
	public static function edit_rule_url( $rule_id ) {
		return admin_url( 'admin.php?page=swd-rule-form&rule_id=' . absint( $rule_id ) );
	}

	/**
	 * Build the URL to delete a specific rule (with nonce).
	 *
	 * @param int $rule_id Rule ID.
	 * @return string
	 */
	public static function delete_rule_url( $rule_id ) {
		return wp_nonce_url(
			admin_url( 'admin.php?page=swd-rules&action=delete&rule_id=' . absint( $rule_id ) ),
			'swd_delete_rule_' . $rule_id
		);
	}
}
