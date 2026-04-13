<?php
/**
 * Frontend Template: Savings Message
 *
 * Shown above cart/checkout totals when the customer has active wholesale discounts.
 *
 * Variables available:
 *   $message  — string, fully formatted savings message with amount already replaced
 *   $savings  — float, total savings amount (unformatted)
 *
 * @package SimpleWholesaleDiscounts
 */

if ( ! defined( 'ABSPATH' ) ) {
	exit;
}
?>
<div class="swd-savings-message" role="note" aria-live="polite">
	<span class="swd-savings-message__icon" aria-hidden="true">
		<svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round">
			<polyline points="20 6 9 17 4 12"/>
		</svg>
	</span>
	<span class="swd-savings-message__text"><?php echo wp_kses_post( $message ); ?></span>
</div>
