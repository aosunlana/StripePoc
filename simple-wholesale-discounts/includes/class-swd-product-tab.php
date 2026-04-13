<?php
/**
 * Product Data Tab — Wholesale Discounts
 *
 * Adds a "Wholesale Discounts" tab to the WooCommerce product data metabox.
 * Displays existing discount rules targeting this product and a quick-add form.
 *
 * @package SimpleWholesaleDiscounts
 */

if ( ! defined( 'ABSPATH' ) ) {
	exit;
}

/**
 * Class SWD_Product_Tab
 */
class SWD_Product_Tab {

	/** @var SWD_Product_Tab|null Singleton. */
	private static $instance = null;

	public static function instance() {
		if ( null === self::$instance ) {
			self::$instance = new self();
		}
		return self::$instance;
	}

	private function __construct() {
		add_filter( 'woocommerce_product_data_tabs',   array( $this, 'register_tab' ) );
		add_action( 'woocommerce_product_data_panels', array( $this, 'render_panel' ) );
		add_action( 'woocommerce_process_product_meta', array( $this, 'handle_save' ), 10, 1 );
		add_action( 'admin_enqueue_scripts',            array( $this, 'enqueue_assets' ) );
	}

	// ---------------------------------------------------------------------------
	// Tab Registration
	// ---------------------------------------------------------------------------

	public function register_tab( $tabs ) {
		$tabs['swd_discounts'] = array(
			'label'    => __( 'Wholesale Discounts', 'simple-wholesale-discounts' ),
			'target'   => 'swd_product_discount_panel',
			'class'    => array(),
			'priority' => 80,
		);
		return $tabs;
	}

	// ---------------------------------------------------------------------------
	// Panel Rendering
	// ---------------------------------------------------------------------------

	public function render_panel() {
		global $post;
		$product_id      = (int) $post->ID;
		$rules           = $this->get_rules_for_product( $product_id );
		$currency_symbol = get_woocommerce_currency_symbol();

		// Grab any transient notice left after a save.
		$notice = get_transient( 'swd_product_tab_notice_' . $product_id );
		if ( $notice ) {
			delete_transient( 'swd_product_tab_notice_' . $product_id );
		}
		?>
		<div id="swd_product_discount_panel" class="panel woocommerce_options_panel swd-pt-panel">

			<?php if ( $notice ) : ?>
				<div class="swd-pt-notice swd-pt-notice--<?php echo esc_attr( $notice['type'] ); ?>">
					<?php echo esc_html( $notice['message'] ); ?>
				</div>
			<?php endif; ?>

			<?php wp_nonce_field( 'swd_quick_add_rule_' . $product_id, 'swd_quick_add_nonce' ); ?>

			<!-- ============================================================
			     EXISTING RULES
			     ============================================================ -->
			<div class="swd-pt-section">
				<div class="swd-pt-section-header">
					<h3><?php esc_html_e( 'Discount Rules for This Product', 'simple-wholesale-discounts' ); ?></h3>
					<a href="<?php echo esc_url( SWD_Admin::rules_page_url() ); ?>" class="swd-pt-view-all">
						<?php esc_html_e( 'Manage all rules →', 'simple-wholesale-discounts' ); ?>
					</a>
				</div>

				<?php if ( ! empty( $rules ) ) : ?>
					<table class="swd-pt-table">
						<thead>
							<tr>
								<th><?php esc_html_e( 'Rule Name', 'simple-wholesale-discounts' ); ?></th>
								<th><?php esc_html_e( 'Discount', 'simple-wholesale-discounts' ); ?></th>
								<th><?php esc_html_e( 'Qty Range', 'simple-wholesale-discounts' ); ?></th>
								<th><?php esc_html_e( 'Status', 'simple-wholesale-discounts' ); ?></th>
								<th><?php esc_html_e( 'Actions', 'simple-wholesale-discounts' ); ?></th>
							</tr>
						</thead>
						<tbody>
							<?php foreach ( $rules as $rule ) : ?>
								<tr>
									<td class="swd-pt-name-cell"><?php echo esc_html( $rule->rule_name ); ?></td>
									<td><?php echo SWD_Discount::format_discount( $rule ); // phpcs:ignore WordPress.Security.EscapeOutput ?></td>
									<td><code><?php echo esc_html( $this->format_qty_range( $rule ) ); ?></code></td>
									<td>
										<span class="swd-pt-badge <?php echo $rule->is_active ? 'swd-pt-badge--active' : 'swd-pt-badge--inactive'; ?>">
											<?php echo $rule->is_active ? esc_html__( 'Active', 'simple-wholesale-discounts' ) : esc_html__( 'Inactive', 'simple-wholesale-discounts' ); ?>
										</span>
									</td>
									<td class="swd-pt-actions-cell">
										<a href="<?php echo esc_url( SWD_Admin::edit_rule_url( $rule->id ) ); ?>" class="swd-pt-action-link">
											<?php esc_html_e( 'Edit', 'simple-wholesale-discounts' ); ?>
										</a>
										<span class="swd-pt-divider">|</span>
										<a href="<?php echo esc_url( SWD_Admin::delete_rule_url( $rule->id ) ); ?>"
										   class="swd-pt-action-link swd-pt-action-link--danger"
										   onclick="return confirm('<?php esc_attr_e( 'Delete this discount rule?', 'simple-wholesale-discounts' ); ?>')">
											<?php esc_html_e( 'Delete', 'simple-wholesale-discounts' ); ?>
										</a>
									</td>
								</tr>
							<?php endforeach; ?>
						</tbody>
					</table>
				<?php else : ?>
					<p class="swd-pt-empty"><?php esc_html_e( 'No discount rules target this product yet. Use the quick-add form below to create one.', 'simple-wholesale-discounts' ); ?></p>
				<?php endif; ?>
			</div>

			<!-- ============================================================
			     QUICK-ADD RULE
			     ============================================================ -->
			<div class="swd-pt-section swd-pt-section--quickadd">
				<div class="swd-pt-section-header">
					<h3><?php esc_html_e( 'Quick-Add Discount Rule', 'simple-wholesale-discounts' ); ?></h3>
				</div>

				<div class="swd-pt-grid">

					<!-- Rule Name -->
					<div class="swd-pt-field swd-pt-field--full">
						<label for="swd_qa_rule_name"><?php esc_html_e( 'Rule Name', 'simple-wholesale-discounts' ); ?></label>
						<input type="text" id="swd_qa_rule_name" name="swd_qa_rule_name"
							placeholder="<?php esc_attr_e( 'e.g. Bulk Buyer 5+', 'simple-wholesale-discounts' ); ?>">
						<span class="swd-pt-hint"><?php esc_html_e( 'An internal label for this rule. Required to save.', 'simple-wholesale-discounts' ); ?></span>
					</div>

					<!-- Discount Type -->
					<div class="swd-pt-field">
						<label for="swd_qa_discount_type"><?php esc_html_e( 'Discount Type', 'simple-wholesale-discounts' ); ?></label>
						<select id="swd_qa_discount_type" name="swd_qa_discount_type">
							<option value="percentage"><?php esc_html_e( 'Percentage (%)', 'simple-wholesale-discounts' ); ?></option>
							<option value="flat"><?php echo esc_html( sprintf( 'Flat Amount (%s)', $currency_symbol ) ); ?></option>
						</select>
					</div>

					<!-- Discount Value -->
					<div class="swd-pt-field">
						<label for="swd_qa_discount_value"><?php esc_html_e( 'Discount Value', 'simple-wholesale-discounts' ); ?></label>
						<div class="swd-pt-input-group">
							<span class="swd-pt-input-prefix" id="swd_qa_prefix">%</span>
							<input type="number" id="swd_qa_discount_value" name="swd_qa_discount_value"
								placeholder="0" min="0" step="0.01">
						</div>
					</div>

					<!-- Min Quantity -->
					<div class="swd-pt-field">
						<label for="swd_qa_min_qty"><?php esc_html_e( 'Min Quantity', 'simple-wholesale-discounts' ); ?></label>
						<input type="number" id="swd_qa_min_qty" name="swd_qa_min_qty"
							placeholder="0" min="0" step="1">
						<span class="swd-pt-hint"><?php esc_html_e( '0 = no minimum', 'simple-wholesale-discounts' ); ?></span>
					</div>

					<!-- Max Quantity -->
					<div class="swd-pt-field">
						<label for="swd_qa_max_qty"><?php esc_html_e( 'Max Quantity', 'simple-wholesale-discounts' ); ?></label>
						<input type="number" id="swd_qa_max_qty" name="swd_qa_max_qty"
							placeholder="0" min="0" step="1">
						<span class="swd-pt-hint"><?php esc_html_e( '0 = unlimited', 'simple-wholesale-discounts' ); ?></span>
					</div>

					<!-- Active toggle -->
					<div class="swd-pt-field swd-pt-field--full">
						<label class="swd-pt-checkbox-label">
							<input type="checkbox" name="swd_qa_is_active" value="1" checked>
							<?php esc_html_e( 'Activate this rule immediately', 'simple-wholesale-discounts' ); ?>
						</label>
					</div>

				</div><!-- .swd-pt-grid -->

				<p class="swd-pt-save-note">
					<span class="dashicons dashicons-info-outline"></span>
					<?php esc_html_e( 'This rule will be saved when you click "Update" or "Publish" on the product.', 'simple-wholesale-discounts' ); ?>
				</p>

			</div><!-- .swd-pt-section--quickadd -->

		</div><!-- #swd_product_discount_panel -->
		<?php
	}

	// ---------------------------------------------------------------------------
	// Save Handler
	// ---------------------------------------------------------------------------

	public function handle_save( $product_id ) {
		if ( ! isset( $_POST['swd_quick_add_nonce'] ) ) {
			return;
		}

		if ( ! wp_verify_nonce(
			sanitize_key( wp_unslash( $_POST['swd_quick_add_nonce'] ) ),
			'swd_quick_add_rule_' . $product_id
		) ) {
			return;
		}

		if ( ! current_user_can( 'manage_woocommerce' ) ) {
			return;
		}

		// Only save if rule name field was filled.
		$rule_name = sanitize_text_field( wp_unslash( $_POST['swd_qa_rule_name'] ?? '' ) );
		if ( empty( $rule_name ) ) {
			return;
		}

		$discount_value = floatval( $_POST['swd_qa_discount_value'] ?? 0 );
		if ( $discount_value <= 0 ) {
			set_transient(
				'swd_product_tab_notice_' . $product_id,
				array( 'type' => 'error', 'message' => __( 'Rule not saved: discount value must be greater than zero.', 'simple-wholesale-discounts' ) ),
				30
			);
			return;
		}

		$data = array(
			'rule_name'      => $rule_name,
			'discount_type'  => sanitize_key( $_POST['swd_qa_discount_type'] ?? 'percentage' ),
			'discount_value' => $discount_value,
			'apply_to'       => 'products',
			'product_ids'    => array( $product_id ),
			'category_ids'   => array(),
			'min_quantity'   => absint( $_POST['swd_qa_min_qty'] ?? 0 ),
			'max_quantity'   => absint( $_POST['swd_qa_max_qty'] ?? 0 ),
			'is_active'      => isset( $_POST['swd_qa_is_active'] ) ? 1 : 0,
		);

		$saved = SWD_Discount::save_rule( $data, 0 );

		set_transient(
			'swd_product_tab_notice_' . $product_id,
			$saved
				? array( 'type' => 'success', 'message' => __( 'Discount rule added successfully.', 'simple-wholesale-discounts' ) )
				: array( 'type' => 'error',   'message' => __( 'Failed to save the discount rule.', 'simple-wholesale-discounts' ) ),
			30
		);
	}

	// ---------------------------------------------------------------------------
	// Asset Enqueueing
	// ---------------------------------------------------------------------------

	public function enqueue_assets( $hook_suffix ) {
		if ( ! in_array( $hook_suffix, array( 'post.php', 'post-new.php' ), true ) ) {
			return;
		}

		global $post;
		if ( ! $post || 'product' !== $post->post_type ) {
			return;
		}

		wp_enqueue_style(
			'swd-admin',
			SWD_PLUGIN_URL . 'assets/css/admin.css',
			array( 'woocommerce_admin_styles' ),
			SWD_VERSION
		);

		// Small inline JS: update the prefix symbol when discount type changes.
		$currency = get_woocommerce_currency_symbol();
		wp_add_inline_script( 'woocommerce_admin', "
(function(){
	document.addEventListener('DOMContentLoaded', function(){
		var sel    = document.getElementById('swd_qa_discount_type');
		var prefix = document.getElementById('swd_qa_prefix');
		if (!sel || !prefix) return;

		function updatePrefix() {
			prefix.textContent = sel.value === 'percentage' ? '%' : '" . esc_js( $currency ) . "';
		}

		sel.addEventListener('change', updatePrefix);
		updatePrefix();
	});
})();" );
	}

	// ---------------------------------------------------------------------------
	// Helpers
	// ---------------------------------------------------------------------------

	private function get_rules_for_product( $product_id ) {
		global $wpdb;
		$table = $wpdb->prefix . 'swd_rules';

		// phpcs:ignore WordPress.DB.DirectDatabaseQuery
		$rules = $wpdb->get_results(
			$wpdb->prepare( "SELECT * FROM {$table} WHERE apply_to = %s ORDER BY id ASC", 'products' )
		);

		if ( ! $rules ) {
			return array();
		}

		return array_values( array_filter(
			$rules,
			function ( $rule ) use ( $product_id ) {
				$ids = json_decode( $rule->product_ids, true );
				return is_array( $ids ) && in_array( (int) $product_id, array_map( 'intval', $ids ), true );
			}
		) );
	}

	private function format_qty_range( $rule ) {
		$min = (int) $rule->min_quantity;
		$max = (int) $rule->max_quantity;

		if ( 0 === $min && 0 === $max ) {
			return __( 'Any', 'simple-wholesale-discounts' );
		}
		if ( $min > 0 && $max > 0 ) {
			return $min . ' – ' . $max;
		}
		if ( $min > 0 ) {
			return $min . '+';
		}
		return '≤ ' . $max;
	}
}
