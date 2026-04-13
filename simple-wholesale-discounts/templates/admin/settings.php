<?php
/**
 * Admin Template: Settings Page
 *
 * Rendered by SWD_Settings::render().
 *
 * @package SimpleWholesaleDiscounts
 */

if ( ! defined( 'ABSPATH' ) ) {
	exit;
}

$options = get_option( 'swd_settings', array() );
?>

<div class="wrap swd-wrap">
	<h1><?php esc_html_e( 'Wholesale Discount Settings', 'simple-wholesale-discounts' ); ?></h1>

	<form method="post" action="options.php">
		<?php
		settings_fields( SWD_Settings::SETTINGS_GROUP );
		?>

		<div class="swd-card">
			<div class="swd-card__header">
				<h2><?php esc_html_e( 'General', 'simple-wholesale-discounts' ); ?></h2>
			</div>
			<div class="swd-card__body">

				<!-- Enable Plugin -->
				<div class="swd-field">
					<label for="swd_enable_plugin"><?php esc_html_e( 'Enable Plugin', 'simple-wholesale-discounts' ); ?></label>
					<label class="swd-toggle-switch">
						<input
							type="checkbox"
							id="swd_enable_plugin"
							name="swd_settings[enable_plugin]"
							value="1"
							<?php checked( 1, ! empty( $options['enable_plugin'] ) ? 1 : 0 ); ?>
						>
						<span class="swd-toggle-slider"></span>
						<span class="swd-toggle-label"><?php esc_html_e( 'On', 'simple-wholesale-discounts' ); ?></span>
					</label>
					<p class="description"><?php esc_html_e( 'Master switch. When off, no discounts are applied even if active rules exist.', 'simple-wholesale-discounts' ); ?></p>
				</div>

				<!-- Discount Label -->
				<div class="swd-field">
					<label for="swd_discount_label"><?php esc_html_e( 'Discount Label', 'simple-wholesale-discounts' ); ?></label>
					<input
						type="text"
						id="swd_discount_label"
						name="swd_settings[discount_label]"
						class="regular-text"
						value="<?php echo esc_attr( $options['discount_label'] ?? __( 'Wholesale Discount', 'simple-wholesale-discounts' ) ); ?>"
					>
					<p class="description"><?php esc_html_e( 'Label shown next to the discount line in cart and checkout totals.', 'simple-wholesale-discounts' ); ?></p>
				</div>

			</div>
		</div>

		<div class="swd-card">
			<div class="swd-card__header">
				<h2><?php esc_html_e( 'Savings Message', 'simple-wholesale-discounts' ); ?></h2>
			</div>
			<div class="swd-card__body">

				<!-- Show Savings Message -->
				<div class="swd-field">
					<label for="swd_show_savings_message"><?php esc_html_e( 'Show Savings Message', 'simple-wholesale-discounts' ); ?></label>
					<label class="swd-toggle-switch">
						<input
							type="checkbox"
							id="swd_show_savings_message"
							name="swd_settings[show_savings_message]"
							value="1"
							<?php checked( 1, ! empty( $options['show_savings_message'] ) ? 1 : 0 ); ?>
						>
						<span class="swd-toggle-slider"></span>
						<span class="swd-toggle-label"><?php esc_html_e( 'On', 'simple-wholesale-discounts' ); ?></span>
					</label>
					<p class="description"><?php esc_html_e( 'Show an info box on the cart page telling customers how much they saved.', 'simple-wholesale-discounts' ); ?></p>
				</div>

				<!-- Savings Message Text -->
				<div class="swd-field">
					<label for="swd_savings_message_text"><?php esc_html_e( 'Savings Message Text', 'simple-wholesale-discounts' ); ?></label>
					<input
						type="text"
						id="swd_savings_message_text"
						name="swd_settings[savings_message_text]"
						class="large-text"
						value="<?php echo esc_attr( $options['savings_message_text'] ?? __( "You're saving {amount} on this order!", 'simple-wholesale-discounts' ) ); ?>"
					>
					<p class="description">
						<?php
						echo wp_kses(
							__( 'Use the <code>{amount}</code> placeholder to display the formatted savings total.', 'simple-wholesale-discounts' ),
							array( 'code' => array() )
						);
						?>
					</p>
				</div>

			</div>
		</div>

		<div class="swd-card">
			<div class="swd-card__header">
				<h2><?php esc_html_e( 'Product Page Badge', 'simple-wholesale-discounts' ); ?></h2>
			</div>
			<div class="swd-card__body">

				<!-- Show Discount Badge -->
				<div class="swd-field">
					<label for="swd_show_discount_badge"><?php esc_html_e( 'Show Discount Badge on Product Page', 'simple-wholesale-discounts' ); ?></label>
					<label class="swd-toggle-switch">
						<input
							type="checkbox"
							id="swd_show_discount_badge"
							name="swd_settings[show_discount_badge]"
							value="1"
							<?php checked( 1, ! empty( $options['show_discount_badge'] ) ? 1 : 0 ); ?>
						>
						<span class="swd-toggle-slider"></span>
						<span class="swd-toggle-label"><?php esc_html_e( 'On', 'simple-wholesale-discounts' ); ?></span>
					</label>
					<p class="description"><?php esc_html_e( 'Show a notice on single product pages when the product has an active wholesale discount rule.', 'simple-wholesale-discounts' ); ?></p>
				</div>

				<!-- Discount Badge Text -->
				<div class="swd-field">
					<label for="swd_discount_badge_text"><?php esc_html_e( 'Discount Badge Text', 'simple-wholesale-discounts' ); ?></label>
					<input
						type="text"
						id="swd_discount_badge_text"
						name="swd_settings[discount_badge_text]"
						class="large-text"
						value="<?php echo esc_attr( $options['discount_badge_text'] ?? __( 'Buy in bulk and save! Wholesale pricing available.', 'simple-wholesale-discounts' ) ); ?>"
					>
				</div>

			</div>
		</div>

		<div class="swd-form-footer">
			<?php submit_button( __( 'Save Settings', 'simple-wholesale-discounts' ), 'primary large', 'submit', false ); ?>
		</div>

	</form>
</div>
