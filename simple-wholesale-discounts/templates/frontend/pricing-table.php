<?php
/**
 * Frontend Template: Tiered Pricing Table
 *
 * Shown below the add to cart form on single product pages when the product has
 * active quantity-based discount rules.
 *
 * Variables available:
 *   $rows  — array of row arrays built by SWD_Frontend::build_pricing_rows():
 *             { quantity_label, discount_label, price, is_discounted }
 *
 * @package SimpleWholesaleDiscounts
 */

if ( ! defined( 'ABSPATH' ) ) {
	exit;
}
?>
<div class="swd-pricing-table" aria-label="<?php esc_attr_e( 'Wholesale pricing tiers', 'simple-wholesale-discounts' ); ?>">
	<h4 class="swd-pricing-table__title"><?php esc_html_e( 'Wholesale Pricing', 'simple-wholesale-discounts' ); ?></h4>
	<table role="table">
		<thead>
			<tr>
				<th scope="col"><?php esc_html_e( 'Quantity', 'simple-wholesale-discounts' ); ?></th>
				<th scope="col"><?php esc_html_e( 'Discount', 'simple-wholesale-discounts' ); ?></th>
				<th scope="col"><?php esc_html_e( 'You Pay', 'simple-wholesale-discounts' ); ?></th>
			</tr>
		</thead>
		<tbody>
			<?php foreach ( $rows as $row ) : ?>
				<tr class="<?php echo $row['is_discounted'] ? 'swd-discounted-row' : ''; ?>">
					<td><?php echo esc_html( $row['quantity_label'] ); ?></td>
					<td><?php echo wp_kses_post( $row['discount_label'] ); ?></td>
					<td class="swd-price-cell"><?php echo wp_kses_post( $row['price'] ); ?></td>
				</tr>
			<?php endforeach; ?>
		</tbody>
	</table>
</div>
