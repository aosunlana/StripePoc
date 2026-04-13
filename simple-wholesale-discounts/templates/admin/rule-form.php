<?php
/**
 * Admin Template: Add / Edit Rule Form
 *
 * Rendered by SWD_Admin::render_rule_form_page().
 * Variables available:
 *   $form — SWD_Rule_Form instance (already processed submit if POST).
 *
 * @package SimpleWholesaleDiscounts
 */

if ( ! defined( 'ABSPATH' ) ) {
	exit;
}

$is_edit        = null !== $form->rule;
$currency_symbol = get_woocommerce_currency_symbol();

// Pre-populate JS data for selected products (edit mode).
$selected_products_json = wp_json_encode( $form->get_selected_products() );
?>

<div class="wrap swd-wrap swd-form-wrap">
	<h1><?php echo esc_html( $form->page_title() ); ?></h1>

	<?php if ( ! empty( $form->errors['general'] ) ) : ?>
		<div class="notice notice-error">
			<p><?php echo esc_html( $form->errors['general'] ); ?></p>
		</div>
	<?php endif; ?>

	<form id="swd-rule-form" method="post" action="<?php echo esc_url( admin_url( 'admin.php?page=swd-rule-form' . ( $is_edit ? '&rule_id=' . absint( $form->rule->id ) : '' ) ) ); ?>">
		<?php wp_nonce_field( 'swd_save_rule', 'swd_rule_nonce' ); ?>
		<input type="hidden" name="rule_id" value="<?php echo $is_edit ? absint( $form->rule->id ) : 0; ?>">

		<!-- ================================================================
		     SECTION 1 — RULE DETAILS
		     ================================================================ -->
		<div class="swd-card">
			<div class="swd-card__header">
				<h2><?php esc_html_e( 'Rule Details', 'simple-wholesale-discounts' ); ?></h2>
			</div>
			<div class="swd-card__body">

				<div class="swd-field <?php echo $form->has_error( 'rule_name' ) ? 'has-error' : ''; ?>">
					<label for="rule_name"><?php esc_html_e( 'Rule Name', 'simple-wholesale-discounts' ); ?> <span class="required">*</span></label>
					<input
						type="text"
						id="rule_name"
						name="rule_name"
						class="regular-text"
						value="<?php echo esc_attr( $form->field( 'rule_name' ) ); ?>"
						placeholder="<?php esc_attr_e( 'e.g. Bulk Buyer Discount', 'simple-wholesale-discounts' ); ?>"
						required
					>
					<?php $form->error_html( 'rule_name' ); ?>
					<p class="description"><?php esc_html_e( 'A friendly internal label to identify this rule.', 'simple-wholesale-discounts' ); ?></p>
				</div>

				<div class="swd-field">
					<label><?php esc_html_e( 'Status', 'simple-wholesale-discounts' ); ?></label>
					<label class="swd-toggle-switch">
						<input
							type="checkbox"
							name="is_active"
							id="is_active"
							value="1"
							<?php checked( 1, (int) $form->field( 'is_active', 1 ) ); ?>
						>
						<span class="swd-toggle-slider"></span>
						<span class="swd-toggle-label"><?php esc_html_e( 'Active', 'simple-wholesale-discounts' ); ?></span>
					</label>
					<p class="description"><?php esc_html_e( 'Inactive rules are saved but never applied to the cart.', 'simple-wholesale-discounts' ); ?></p>
				</div>

			</div>
		</div>

		<!-- ================================================================
		     SECTION 2 — DISCOUNT CONFIGURATION
		     ================================================================ -->
		<div class="swd-card">
			<div class="swd-card__header">
				<h2><?php esc_html_e( 'Discount Configuration', 'simple-wholesale-discounts' ); ?></h2>
			</div>
			<div class="swd-card__body">

				<div class="swd-field <?php echo $form->has_error( 'discount_type' ) ? 'has-error' : ''; ?>">
					<label><?php esc_html_e( 'Discount Type', 'simple-wholesale-discounts' ); ?> <span class="required">*</span></label>
					<div class="swd-type-cards" role="group" aria-label="<?php esc_attr_e( 'Discount Type', 'simple-wholesale-discounts' ); ?>">
						<label class="swd-type-card <?php echo 'percentage' === $form->field( 'discount_type', 'percentage' ) ? 'selected' : ''; ?>" for="type_percentage">
							<input type="radio" name="discount_type" id="type_percentage" value="percentage" <?php checked( 'percentage', $form->field( 'discount_type', 'percentage' ) ); ?>>
							<span class="swd-type-card__icon">%</span>
							<span class="swd-type-card__title"><?php esc_html_e( 'Percentage', 'simple-wholesale-discounts' ); ?></span>
							<span class="swd-type-card__desc"><?php esc_html_e( 'e.g. 15% off the product price', 'simple-wholesale-discounts' ); ?></span>
						</label>
						<label class="swd-type-card <?php echo 'flat' === $form->field( 'discount_type', 'percentage' ) ? 'selected' : ''; ?>" for="type_flat">
							<input type="radio" name="discount_type" id="type_flat" value="flat" <?php checked( 'flat', $form->field( 'discount_type', 'percentage' ) ); ?>>
							<span class="swd-type-card__icon"><?php echo esc_html( $currency_symbol ); ?></span>
							<span class="swd-type-card__title"><?php esc_html_e( 'Flat Amount', 'simple-wholesale-discounts' ); ?></span>
							<span class="swd-type-card__desc"><?php esc_html_e( 'e.g. $10.00 off the product price', 'simple-wholesale-discounts' ); ?></span>
						</label>
					</div>
					<?php $form->error_html( 'discount_type' ); ?>
				</div>

				<div class="swd-field <?php echo $form->has_error( 'discount_value' ) ? 'has-error' : ''; ?>">
					<label for="discount_value"><?php esc_html_e( 'Discount Value', 'simple-wholesale-discounts' ); ?> <span class="required">*</span></label>
					<div class="swd-input-prefix-wrap">
						<span class="swd-input-prefix" id="discount_prefix"></span>
						<input
							type="number"
							id="discount_value"
							name="discount_value"
							class="small-text"
							value="<?php echo esc_attr( $form->field( 'discount_value', '' ) ); ?>"
							min="0"
							step="0.01"
							placeholder="0"
						>
					</div>
					<?php $form->error_html( 'discount_value' ); ?>
					<p class="description swd-discount-preview" id="discount_preview">
						<?php esc_html_e( 'Enter a value above to see a preview.', 'simple-wholesale-discounts' ); ?>
					</p>
				</div>

			</div>
		</div>

		<!-- ================================================================
		     SECTION 3 — APPLY THIS RULE TO
		     ================================================================ -->
		<div class="swd-card">
			<div class="swd-card__header">
				<h2><?php esc_html_e( 'Apply This Rule To', 'simple-wholesale-discounts' ); ?></h2>
			</div>
			<div class="swd-card__body">

				<div class="swd-field <?php echo $form->has_error( 'apply_to' ) ? 'has-error' : ''; ?>">
					<div class="swd-apply-cards" role="group" aria-label="<?php esc_attr_e( 'Apply Rule To', 'simple-wholesale-discounts' ); ?>">

						<label class="swd-apply-card <?php echo 'all' === $form->field( 'apply_to', 'all' ) ? 'selected' : ''; ?>" for="apply_all">
							<input type="radio" name="apply_to" id="apply_all" value="all" <?php checked( 'all', $form->field( 'apply_to', 'all' ) ); ?>>
							<span class="swd-apply-card__icon">
								<svg width="28" height="28" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><circle cx="12" cy="12" r="10"/><path d="M12 8v4l3 3"/></svg>
							</span>
							<span class="swd-apply-card__title"><?php esc_html_e( 'All Products', 'simple-wholesale-discounts' ); ?></span>
							<span class="swd-apply-card__desc"><?php esc_html_e( 'Rule applies to every product in the store', 'simple-wholesale-discounts' ); ?></span>
						</label>

						<label class="swd-apply-card <?php echo 'products' === $form->field( 'apply_to', 'all' ) ? 'selected' : ''; ?>" for="apply_products">
							<input type="radio" name="apply_to" id="apply_products" value="products" <?php checked( 'products', $form->field( 'apply_to', 'all' ) ); ?>>
							<span class="swd-apply-card__icon">
								<svg width="28" height="28" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M20 7H4l1.5 9h13L20 7z"/><circle cx="9" cy="20" r="1"/><circle cx="16" cy="20" r="1"/><path d="M9 7V5a3 3 0 016 0v2"/></svg>
							</span>
							<span class="swd-apply-card__title"><?php esc_html_e( 'Specific Products', 'simple-wholesale-discounts' ); ?></span>
							<span class="swd-apply-card__desc"><?php esc_html_e( 'Choose individual products', 'simple-wholesale-discounts' ); ?></span>
						</label>

						<label class="swd-apply-card <?php echo 'categories' === $form->field( 'apply_to', 'all' ) ? 'selected' : ''; ?>" for="apply_categories">
							<input type="radio" name="apply_to" id="apply_categories" value="categories" <?php checked( 'categories', $form->field( 'apply_to', 'all' ) ); ?>>
							<span class="swd-apply-card__icon">
								<svg width="28" height="28" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><rect x="3" y="3" width="7" height="7"/><rect x="14" y="3" width="7" height="7"/><rect x="3" y="14" width="7" height="7"/><rect x="14" y="14" width="7" height="7"/></svg>
							</span>
							<span class="swd-apply-card__title"><?php esc_html_e( 'Specific Categories', 'simple-wholesale-discounts' ); ?></span>
							<span class="swd-apply-card__desc"><?php esc_html_e( 'Choose product categories', 'simple-wholesale-discounts' ); ?></span>
						</label>

					</div>
					<?php $form->error_html( 'apply_to' ); ?>
				</div>

				<!-- Product Search (shown when apply_to = products) -->
				<div id="swd-product-search-wrap" class="swd-dependent-field" style="display:none;">
					<div class="swd-field <?php echo $form->has_error( 'product_ids' ) ? 'has-error' : ''; ?>">
						<label><?php esc_html_e( 'Search Products', 'simple-wholesale-discounts' ); ?></label>
						<div class="swd-product-search">
							<input
								type="text"
								id="swd_product_search_input"
								class="regular-text"
								placeholder="<?php esc_attr_e( 'Type to search products...', 'simple-wholesale-discounts' ); ?>"
								autocomplete="off"
							>
							<div id="swd_product_search_results" class="swd-search-dropdown" style="display:none;"></div>
						</div>
						<!-- Hidden input carries comma-separated product IDs to POST. -->
						<input type="hidden" name="product_ids" id="swd_product_ids" value="<?php echo esc_attr( implode( ',', (array) $form->field( 'product_ids', array() ) ) ); ?>">
						<div id="swd_selected_products" class="swd-selected-tags"></div>
						<?php $form->error_html( 'product_ids' ); ?>
						<!-- Hidden data for JS to pre-populate edit mode tags. -->
						<script>window.swdSelectedProducts = <?php echo $selected_products_json; // phpcs:ignore WordPress.Security.EscapeOutput ?>;</script>
					</div>
				</div>

				<!-- Category Checkboxes (shown when apply_to = categories) -->
				<div id="swd-category-list-wrap" class="swd-dependent-field" style="display:none;">
					<div class="swd-field <?php echo $form->has_error( 'category_ids' ) ? 'has-error' : ''; ?>">
						<label><?php esc_html_e( 'Product Categories', 'simple-wholesale-discounts' ); ?></label>
						<div class="swd-category-list">
							<?php
							$selected_cats = (array) $form->field( 'category_ids', array() );
							$categories    = $form->get_categories_nested();

							// Recursive function to render nested category checkboxes.
							function swd_render_categories( $categories, $selected_cats ) {
								foreach ( $categories as $cat ) {
									$indent = str_repeat( '&nbsp;&nbsp;&nbsp;', $cat->depth );
									?>
									<label class="swd-cat-item" style="padding-left:<?php echo absint( $cat->depth * 20 ); ?>px;">
										<input
											type="checkbox"
											name="category_ids[]"
											value="<?php echo absint( $cat->term_id ); ?>"
											<?php checked( in_array( $cat->term_id, array_map( 'intval', $selected_cats ), true ) ); ?>
										>
										<?php echo esc_html( $cat->name ); ?>
										<span class="swd-cat-count">(<?php echo absint( $cat->count ); ?>)</span>
									</label>
									<?php
									if ( ! empty( $cat->children ) ) {
										swd_render_categories( $cat->children, $selected_cats );
									}
								}
							}

							if ( ! empty( $categories ) ) {
								swd_render_categories( $categories, $selected_cats );
							} else {
								echo '<p class="description">' . esc_html__( 'No product categories found.', 'simple-wholesale-discounts' ) . '</p>';
							}
							?>
						</div>
						<?php $form->error_html( 'category_ids' ); ?>
					</div>
				</div>

			</div>
		</div>

		<!-- ================================================================
		     SECTION 4 — QUANTITY RULES
		     ================================================================ -->
		<div class="swd-card">
			<div class="swd-card__header">
				<h2><?php esc_html_e( 'Quantity Rules', 'simple-wholesale-discounts' ); ?></h2>
			</div>
			<div class="swd-card__body">

				<div class="swd-quantity-row">
					<div class="swd-field <?php echo $form->has_error( 'min_quantity' ) ? 'has-error' : ''; ?>">
						<label for="min_quantity"><?php esc_html_e( 'Minimum Quantity', 'simple-wholesale-discounts' ); ?></label>
						<input
							type="number"
							id="min_quantity"
							name="min_quantity"
							class="small-text"
							value="<?php echo esc_attr( $form->field( 'min_quantity', 0 ) ); ?>"
							min="0"
							step="1"
							placeholder="<?php esc_attr_e( 'e.g. 5', 'simple-wholesale-discounts' ); ?>"
						>
						<?php $form->error_html( 'min_quantity' ); ?>
						<p class="description"><?php esc_html_e( 'Discount applies when customer buys at least this many units. Leave 0 for no minimum.', 'simple-wholesale-discounts' ); ?></p>
					</div>

					<div class="swd-field <?php echo $form->has_error( 'max_quantity' ) ? 'has-error' : ''; ?>">
						<label for="max_quantity"><?php esc_html_e( 'Maximum Quantity', 'simple-wholesale-discounts' ); ?></label>
						<input
							type="number"
							id="max_quantity"
							name="max_quantity"
							class="small-text"
							value="<?php echo esc_attr( $form->field( 'max_quantity', 0 ) ); ?>"
							min="0"
							step="1"
							placeholder="<?php esc_attr_e( 'e.g. 50', 'simple-wholesale-discounts' ); ?>"
						>
						<?php $form->error_html( 'max_quantity' ); ?>
						<p class="description"><?php esc_html_e( 'Discount stops applying above this quantity. Leave 0 for unlimited.', 'simple-wholesale-discounts' ); ?></p>
					</div>
				</div>

				<p class="description swd-qty-preview" id="qty_preview">
					<?php esc_html_e( 'This discount applies to any quantity.', 'simple-wholesale-discounts' ); ?>
				</p>

			</div>
		</div>

		<!-- ================================================================
		     FORM FOOTER
		     ================================================================ -->
		<div class="swd-form-footer">
			<button type="submit" class="button button-primary button-large">
				<?php esc_html_e( 'Save Rule', 'simple-wholesale-discounts' ); ?>
			</button>
			<a href="<?php echo esc_url( SWD_Admin::rules_page_url() ); ?>" class="swd-cancel-link">
				<?php esc_html_e( 'Cancel', 'simple-wholesale-discounts' ); ?>
			</a>
		</div>

	</form>
</div>
