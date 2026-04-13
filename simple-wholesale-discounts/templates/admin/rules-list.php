<?php
/**
 * Admin Template: Discount Rules List Page
 *
 * Rendered by SWD_Admin::render_rules_page().
 * Variables available:
 *   $list_table — SWD_Rules_List_Table instance (already prepared).
 *
 * @package SimpleWholesaleDiscounts
 */

if ( ! defined( 'ABSPATH' ) ) {
	exit;
}

// Show any result notice messages.
$message = isset( $_GET['message'] ) ? sanitize_key( $_GET['message'] ) : ''; // phpcs:ignore WordPress.Security.NonceVerification
$notices = array(
	'created'          => array( 'success', __( 'Discount rule created successfully.', 'simple-wholesale-discounts' ) ),
	'updated'          => array( 'success', __( 'Discount rule updated successfully.', 'simple-wholesale-discounts' ) ),
	'deleted'          => array( 'success', __( 'Discount rule deleted.', 'simple-wholesale-discounts' ) ),
	'bulk_deleted'     => array( 'success', __( 'Selected rules deleted.', 'simple-wholesale-discounts' ) ),
	'bulk_activated'   => array( 'success', __( 'Selected rules activated.', 'simple-wholesale-discounts' ) ),
	'bulk_deactivated' => array( 'success', __( 'Selected rules deactivated.', 'simple-wholesale-discounts' ) ),
);
?>

<div class="wrap swd-wrap">
	<h1 class="wp-heading-inline">
		<?php esc_html_e( 'Wholesale Discount Rules', 'simple-wholesale-discounts' ); ?>
	</h1>
	<a href="<?php echo esc_url( SWD_Admin::add_rule_url() ); ?>" class="page-title-action swd-add-rule-btn">
		+ <?php esc_html_e( 'Add New Rule', 'simple-wholesale-discounts' ); ?>
	</a>
	<hr class="wp-header-end">

	<?php if ( ! empty( $message ) && isset( $notices[ $message ] ) ) : ?>
		<div class="notice notice-<?php echo esc_attr( $notices[ $message ][0] ); ?> is-dismissible">
			<p><?php echo esc_html( $notices[ $message ][1] ); ?></p>
		</div>
	<?php endif; ?>

	<?php if ( ! empty( $list_table->items ) ) : ?>

		<form id="swd-rules-form" method="post" action="">
			<?php
			// Required for bulk action nonce verification.
			wp_nonce_field( 'bulk-rules' );
			$list_table->display();
			?>
		</form>

	<?php else : ?>

		<?php // Empty state — friendly illustration and CTA. ?>
		<div class="swd-empty-state">
			<div class="swd-empty-icon" aria-hidden="true">
				<svg width="80" height="80" viewBox="0 0 80 80" fill="none" xmlns="http://www.w3.org/2000/svg">
					<rect width="80" height="80" rx="16" fill="#f0eef8"/>
					<path d="M24 56V32a2 2 0 012-2h7m18 0h-8m-10 0h10m0 0v-8a2 2 0 012-2h4a2 2 0 012 2v8M24 40h32m-32 8h20" stroke="#7f54b3" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"/>
					<circle cx="52" cy="52" r="10" fill="#7f54b3"/>
					<path d="M52 48v4l3 3" stroke="#fff" stroke-width="2" stroke-linecap="round"/>
				</svg>
			</div>
			<h2><?php esc_html_e( 'No discount rules yet', 'simple-wholesale-discounts' ); ?></h2>
			<p><?php esc_html_e( 'Create your first wholesale discount rule to start rewarding bulk buyers.', 'simple-wholesale-discounts' ); ?></p>
			<a href="<?php echo esc_url( SWD_Admin::add_rule_url() ); ?>" class="button button-primary swd-cta-button">
				<?php esc_html_e( 'Create your first rule', 'simple-wholesale-discounts' ); ?>
			</a>
		</div>

	<?php endif; ?>
</div>
