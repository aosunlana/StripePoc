<?php
/**
 * Plugin Activator
 *
 * Handles tasks that must run on plugin activation:
 * - Creates the custom swd_rules database table via dbDelta().
 * - Stores the DB schema version in wp_options for future upgrades.
 *
 * @package SimpleWholesaleDiscounts
 */

if ( ! defined( 'ABSPATH' ) ) {
	exit;
}

/**
 * Class SWD_Activator
 */
class SWD_Activator {

	/**
	 * Run activation tasks.
	 * Called by register_activation_hook() in the main plugin file.
	 */
	public static function activate() {
		self::create_tables();
		self::set_default_options();
	}

	/**
	 * Create (or upgrade) the swd_rules table using dbDelta().
	 *
	 * dbDelta() is safe to call on every activation — it only makes
	 * changes when the schema differs from the existing table definition.
	 */
	private static function create_tables() {
		global $wpdb;

		$table_name      = $wpdb->prefix . 'swd_rules';
		$charset_collate = $wpdb->get_charset_collate();

		// IMPORTANT: dbDelta() requires specific formatting:
		// - Two spaces between column name and type.
		// - PRIMARY KEY on a separate line (no leading spaces after the last column).
		$sql = "CREATE TABLE {$table_name} (
  id BIGINT(20) UNSIGNED NOT NULL AUTO_INCREMENT,
  rule_name VARCHAR(255) NOT NULL DEFAULT '',
  discount_type VARCHAR(20) NOT NULL DEFAULT 'percentage',
  discount_value DECIMAL(10,2) NOT NULL DEFAULT 0.00,
  apply_to VARCHAR(20) NOT NULL DEFAULT 'all',
  product_ids LONGTEXT NOT NULL DEFAULT '',
  category_ids LONGTEXT NOT NULL DEFAULT '',
  min_quantity INT(11) NOT NULL DEFAULT 0,
  max_quantity INT(11) NOT NULL DEFAULT 0,
  is_active TINYINT(1) NOT NULL DEFAULT 1,
  created_at DATETIME NOT NULL DEFAULT '0000-00-00 00:00:00',
  updated_at DATETIME NOT NULL DEFAULT '0000-00-00 00:00:00',
  PRIMARY KEY (id)
) {$charset_collate};";

		// dbDelta() is in wp-admin/includes/upgrade.php.
		require_once ABSPATH . 'wp-admin/includes/upgrade.php';
		dbDelta( $sql );

		// Store DB schema version so future updates can run migrations.
		update_option( 'swd_db_version', SWD_DB_VERSION );
	}

	/**
	 * Set sensible default option values for settings that haven't been
	 * configured by the site admin yet.
	 *
	 * add_option() is a no-op if the option already exists — safe to call
	 * on every activation.
	 */
	private static function set_default_options() {
		add_option(
			'swd_settings',
			array(
				'enable_plugin'             => 1,
				'discount_label'            => __( 'Wholesale Discount', 'simple-wholesale-discounts' ),
				'show_savings_message'      => 1,
				'savings_message_text'      => __( "You're saving {amount} on this order!", 'simple-wholesale-discounts' ),
				'show_discount_badge'       => 1,
				'discount_badge_text'       => __( 'Buy in bulk and save! Wholesale pricing available.', 'simple-wholesale-discounts' ),
			)
		);
	}
}
