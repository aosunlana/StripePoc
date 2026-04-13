<?php
/**
 * Frontend Template: Discount Badge
 *
 * Rendered on single product pages when the product has active discount rules.
 * Variables available:
 *   $badge_text  — string, the badge message from settings
 *   $rules       — array of matching rule objects (for context)
 *
 * @package SimpleWholesaleDiscounts
 */

if ( ! defined( 'ABSPATH' ) ) {
	exit;
}
?>
<div class="swd-discount-badge" role="note" aria-label="<?php esc_attr_e( 'Wholesale discount available', 'simple-wholesale-discounts' ); ?>">
	<span class="swd-discount-badge__icon" aria-hidden="true">
		<svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round">
			<path d="M12 2l3.09 6.26L22 9.27l-5 4.87 1.18 6.88L12 17.77l-6.18 3.25L7 14.14 2 9.27l6.91-1.01L12 2z"/>
		</svg>
	</span>
	<span class="swd-discount-badge__text"><?php echo esc_html( $badge_text ); ?></span>
</div>
