<?php
/**
 * Settings Page
 *
 * Registers settings via WordPress Settings API and renders the settings page.
 *
 * @package SimpleWholesaleDiscounts
 */

if ( ! defined( 'ABSPATH' ) ) {
	exit;
}

/**
 * Class SWD_Settings
 */
class SWD_Settings {

	/** Settings option key in wp_options. */
	const OPTION_KEY = 'swd_settings';

	/** Settings group used with Settings API. */
	const SETTINGS_GROUP = 'swd_settings_group';

	/**
	 * Constructor — register settings with WordPress Settings API.
	 * Settings API hooks must fire on admin_init.
	 */
	public function __construct() {
		add_action( 'admin_init', array( $this, 'register_settings' ) );
	}

	/**
	 * Register the settings group and individual settings.
	 */
	public function register_settings() {
		register_setting(
			self::SETTINGS_GROUP,
			self::OPTION_KEY,
			array( $this, 'sanitize_settings' )
		);
	}

	/**
	 * Sanitize settings values before saving.
	 *
	 * @param array $input Raw submitted settings.
	 * @return array Sanitized settings.
	 */
	public function sanitize_settings( $input ) {
		$sanitized = array();

		$sanitized['enable_plugin']        = ! empty( $input['enable_plugin'] ) ? 1 : 0;
		$sanitized['discount_label']       = sanitize_text_field( $input['discount_label'] ?? '' );
		$sanitized['show_savings_message'] = ! empty( $input['show_savings_message'] ) ? 1 : 0;
		$sanitized['savings_message_text'] = sanitize_text_field( $input['savings_message_text'] ?? '' );
		$sanitized['show_discount_badge']  = ! empty( $input['show_discount_badge'] ) ? 1 : 0;
		$sanitized['discount_badge_text']  = sanitize_text_field( $input['discount_badge_text'] ?? '' );

		// Defaults for empty strings.
		if ( empty( $sanitized['discount_label'] ) ) {
			$sanitized['discount_label'] = __( 'Wholesale Discount', 'simple-wholesale-discounts' );
		}
		if ( empty( $sanitized['savings_message_text'] ) ) {
			$sanitized['savings_message_text'] = __( "You're saving {amount} on this order!", 'simple-wholesale-discounts' );
		}
		if ( empty( $sanitized['discount_badge_text'] ) ) {
			$sanitized['discount_badge_text'] = __( 'Buy in bulk and save! Wholesale pricing available.', 'simple-wholesale-discounts' );
		}

		return $sanitized;
	}

	/**
	 * Get a specific setting value.
	 *
	 * @param string $key     Setting key.
	 * @param mixed  $default Default value.
	 * @return mixed
	 */
	public static function get( $key, $default = null ) {
		$options = get_option( self::OPTION_KEY, array() );
		return isset( $options[ $key ] ) ? $options[ $key ] : $default;
	}

	/**
	 * Render the settings page — delegates to the template.
	 */
	public function render() {
		include SWD_PLUGIN_DIR . 'templates/admin/settings.php';
	}
}
