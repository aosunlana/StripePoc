<?php
/**
 * Plugin Name: Simple Wholesale Discounts
 * Description: Lightweight wholesale discount rules for WooCommerce. Set flat or percentage discounts on products or categories based on quantity ranges. Simple, fast, no bloat.
 * Version: 1.0.0
 * Requires at least: 6.0
 * Requires PHP: 7.4
 * Requires Plugins: woocommerce
 * Author: Simple Wholesale Discounts
 * License: GPL-2.0+
 * License URI: https://www.gnu.org/licenses/gpl-2.0.html
 * Text Domain: simple-wholesale-discounts
 * Domain Path: /languages
 *
 * @package SimpleWholesaleDiscounts
 */

// Prevent direct access.
if ( ! defined( 'ABSPATH' ) ) {
	exit;
}

// Plugin constants.
define( 'SWD_VERSION', '1.0.0' );
define( 'SWD_DB_VERSION', '1.0.0' );
define( 'SWD_PLUGIN_FILE', __FILE__ );
define( 'SWD_PLUGIN_DIR', plugin_dir_path( __FILE__ ) );
define( 'SWD_PLUGIN_URL', plugin_dir_url( __FILE__ ) );
define( 'SWD_PLUGIN_BASENAME', plugin_basename( __FILE__ ) );

/**
 * Check if WooCommerce is active.
 * Returns true if WooCommerce is active (or in multisite network active).
 *
 * @return bool
 */
function swd_is_woocommerce_active() {
	$active_plugins = (array) get_option( 'active_plugins', array() );

	if ( is_multisite() ) {
		$active_plugins = array_merge( $active_plugins, array_keys( get_site_option( 'active_sitewide_plugins', array() ) ) );
	}

	return in_array( 'woocommerce/woocommerce.php', $active_plugins, true )
		|| class_exists( 'WooCommerce' );
}

/**
 * Show an admin notice if WooCommerce is not active.
 */
function swd_woocommerce_missing_notice() {
	?>
	<div class="notice notice-error">
		<p>
			<strong><?php esc_html_e( 'Simple Wholesale Discounts', 'simple-wholesale-discounts' ); ?></strong>
			<?php esc_html_e( 'requires WooCommerce to be installed and active. Please install or activate WooCommerce before using this plugin.', 'simple-wholesale-discounts' ); ?>
		</p>
	</div>
	<?php
}

/**
 * Deactivate the plugin gracefully if WooCommerce is missing.
 */
function swd_deactivate_self() {
	deactivate_plugins( SWD_PLUGIN_BASENAME );

	// Remove the "Plugin activated" notice WP may have added.
	if ( isset( $_GET['activate'] ) ) { // phpcs:ignore WordPress.Security.NonceVerification
		unset( $_GET['activate'] );
	}
}

/**
 * Boot the plugin.
 * Called on plugins_loaded so WooCommerce is already registered.
 */
function swd_init() {

	// Load text domain for translations.
	load_plugin_textdomain(
		'simple-wholesale-discounts',
		false,
		dirname( SWD_PLUGIN_BASENAME ) . '/languages'
	);

	// Guard: WooCommerce must be active.
	if ( ! swd_is_woocommerce_active() ) {
		add_action( 'admin_notices', 'swd_woocommerce_missing_notice' );
		add_action( 'admin_init', 'swd_deactivate_self' );
		return;
	}

	// Load all classes.
	swd_load_includes();

	// Initialise each component.
	SWD_Discount::instance();
	SWD_Frontend::instance();

	if ( is_admin() ) {
		SWD_Admin::instance();
		SWD_Ajax::instance();
		SWD_Product_Tab::instance();
	}
}
add_action( 'plugins_loaded', 'swd_init' );

/**
 * Require all include files.
 */
function swd_load_includes() {
	$includes = array(
		'includes/class-swd-activator.php',
		'includes/class-swd-admin.php',
		'includes/class-swd-list-table.php',
		'includes/class-swd-rule-form.php',
		'includes/class-swd-settings.php',
		'includes/class-swd-ajax.php',
		'includes/class-swd-discount.php',
		'includes/class-swd-frontend.php',
		'includes/class-swd-product-tab.php',
	);

	foreach ( $includes as $file ) {
		require_once SWD_PLUGIN_DIR . $file;
	}
}

// ---------------------------------------------------------------------------
// Activation / Deactivation / Uninstall Hooks
// ---------------------------------------------------------------------------

/**
 * Plugin activation hook.
 * Creates the database table and stores db version.
 */
function swd_activate() {
	// We need the Activator class but includes aren't loaded yet at this stage.
	require_once plugin_dir_path( __FILE__ ) . 'includes/class-swd-activator.php';
	SWD_Activator::activate();
}
register_activation_hook( __FILE__, 'swd_activate' );

/**
 * Plugin deactivation hook (cleanup transients etc.).
 */
function swd_deactivate() {
	// Nothing to do on deactivation. Rules remain in DB.
}
register_deactivation_hook( __FILE__, 'swd_deactivate' );
