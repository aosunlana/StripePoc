<?php
/**
 * WP_List_Table Extension for Discount Rules
 *
 * Renders the rules list in the admin with sortable columns,
 * bulk actions, and inline delete/toggle links.
 *
 * @package SimpleWholesaleDiscounts
 */

if ( ! defined( 'ABSPATH' ) ) {
	exit;
}

// Load parent class if not available (outside normal admin init flow).
if ( ! class_exists( 'WP_List_Table' ) ) {
	require_once ABSPATH . 'wp-admin/includes/class-wp-list-table.php';
}

/**
 * Class SWD_Rules_List_Table
 */
class SWD_Rules_List_Table extends WP_List_Table {

	/**
	 * Constructor — set up table configuration.
	 */
	public function __construct() {
		parent::__construct(
			array(
				'singular' => 'rule',
				'plural'   => 'rules',
				'ajax'     => false,
			)
		);
	}

	/**
	 * Define columns shown in the table.
	 *
	 * @return array Associative array of column_key => column_label.
	 */
	public function get_columns() {
		return array(
			'cb'             => '<input type="checkbox" />',
			'rule_name'      => __( 'Rule Name', 'simple-wholesale-discounts' ),
			'discount'       => __( 'Discount', 'simple-wholesale-discounts' ),
			'applies_to'     => __( 'Applies To', 'simple-wholesale-discounts' ),
			'quantity_range' => __( 'Quantity Range', 'simple-wholesale-discounts' ),
			'is_active'      => __( 'Status', 'simple-wholesale-discounts' ),
			'actions'        => __( 'Actions', 'simple-wholesale-discounts' ),
		);
	}

	/**
	 * Define which columns are sortable.
	 *
	 * @return array
	 */
	public function get_sortable_columns() {
		return array(
			'rule_name'  => array( 'rule_name', false ),
			'discount'   => array( 'discount_value', false ),
			'is_active'  => array( 'is_active', false ),
		);
	}

	/**
	 * Define the available bulk actions.
	 *
	 * @return array
	 */
	public function get_bulk_actions() {
		return array(
			'activate'   => __( 'Activate', 'simple-wholesale-discounts' ),
			'deactivate' => __( 'Deactivate', 'simple-wholesale-discounts' ),
			'delete'     => __( 'Delete', 'simple-wholesale-discounts' ),
		);
	}

	/**
	 * Process any submitted bulk action.
	 */
	public function process_bulk_action() {
		$action = $this->current_action();

		if ( ! $action ) {
			return;
		}

		// Verify nonce for bulk actions.
		if ( ! isset( $_REQUEST['_wpnonce'] ) || ! wp_verify_nonce( sanitize_key( wp_unslash( $_REQUEST['_wpnonce'] ) ), 'bulk-rules' ) ) {
			if ( 'delete' === $action || 'activate' === $action || 'deactivate' === $action ) {
				wp_die( esc_html__( 'Nonce verification failed.', 'simple-wholesale-discounts' ) );
			}
		}

		// Handle single row delete (via action link).
		if ( 'delete' === $action && isset( $_GET['rule_id'] ) ) {
			$rule_id = absint( $_GET['rule_id'] );
			if ( isset( $_GET['_wpnonce'] ) && wp_verify_nonce( sanitize_key( wp_unslash( $_GET['_wpnonce'] ) ), 'swd_delete_rule_' . $rule_id ) ) {
				SWD_Discount::delete_rule( $rule_id );
				wp_safe_redirect( add_query_arg( 'message', 'deleted', SWD_Admin::rules_page_url() ) );
				exit;
			}
			return;
		}

		// Handle bulk actions.
		$rule_ids = isset( $_POST['rule'] ) ? array_map( 'absint', (array) $_POST['rule'] ) : array();

		if ( empty( $rule_ids ) ) {
			return;
		}

		switch ( $action ) {
			case 'delete':
				foreach ( $rule_ids as $id ) {
					SWD_Discount::delete_rule( $id );
				}
				$message = 'bulk_deleted';
				break;

			case 'activate':
				foreach ( $rule_ids as $id ) {
					SWD_Discount::toggle_status( $id, 1 );
				}
				$message = 'bulk_activated';
				break;

			case 'deactivate':
				foreach ( $rule_ids as $id ) {
					SWD_Discount::toggle_status( $id, 0 );
				}
				$message = 'bulk_deactivated';
				break;

			default:
				return;
		}

		wp_safe_redirect( add_query_arg( 'message', $message, SWD_Admin::rules_page_url() ) );
		exit;
	}

	/**
	 * Fetch data and prepare items for display.
	 */
	public function prepare_items() {
		$per_page     = 20;
		$current_page = $this->get_pagenum();

		// phpcs:disable WordPress.Security.NonceVerification
		$orderby = isset( $_REQUEST['orderby'] ) ? sanitize_key( $_REQUEST['orderby'] ) : 'id';
		$order   = isset( $_REQUEST['order'] ) ? sanitize_key( $_REQUEST['order'] ) : 'ASC';
		$search  = isset( $_REQUEST['s'] ) ? sanitize_text_field( wp_unslash( $_REQUEST['s'] ) ) : '';
		// phpcs:enable

		$all_rules = SWD_Discount::get_all_rules(
			array(
				'orderby' => $orderby,
				'order'   => $order,
				'search'  => $search,
			)
		);

		$total_items = count( $all_rules );

		$this->set_pagination_args(
			array(
				'total_items' => $total_items,
				'per_page'    => $per_page,
			)
		);

		// Slice for current page.
		$this->items = array_slice( $all_rules, ( $current_page - 1 ) * $per_page, $per_page );

		// Required to declare columns.
		$this->_column_headers = array(
			$this->get_columns(),
			array(),
			$this->get_sortable_columns(),
		);
	}

	/**
	 * Render the checkbox column.
	 *
	 * @param object $item Current row rule object.
	 * @return string
	 */
	public function column_cb( $item ) {
		return sprintf(
			'<input type="checkbox" name="rule[]" value="%d" />',
			absint( $item->id )
		);
	}

	/**
	 * Render the rule_name column with row actions.
	 *
	 * @param object $item Current row rule object.
	 * @return string
	 */
	public function column_rule_name( $item ) {
		$edit_url   = SWD_Admin::edit_rule_url( $item->id );
		$delete_url = SWD_Admin::delete_rule_url( $item->id );

		$name = '<strong><a href="' . esc_url( $edit_url ) . '">' . esc_html( $item->rule_name ) . '</a></strong>';

		$row_actions = array(
			'edit'   => '<a href="' . esc_url( $edit_url ) . '">' . esc_html__( 'Edit', 'simple-wholesale-discounts' ) . '</a>',
			'delete' => '<a href="' . esc_url( $delete_url ) . '" class="swd-delete-rule" data-confirm="' . esc_attr__( 'Are you sure you want to delete this rule? This action cannot be undone.', 'simple-wholesale-discounts' ) . '" style="color:#b32d2e;">' . esc_html__( 'Delete', 'simple-wholesale-discounts' ) . '</a>',
		);

		return $name . $this->row_actions( $row_actions );
	}

	/**
	 * Render the discount column.
	 *
	 * @param object $item Current row rule object.
	 * @return string
	 */
	public function column_discount( $item ) {
		return SWD_Discount::format_discount( $item );
	}

	/**
	 * Render the applies_to column.
	 *
	 * @param object $item Current row rule object.
	 * @return string
	 */
	public function column_applies_to( $item ) {
		switch ( $item->apply_to ) {
			case 'all':
				return '<span class="swd-applies-all">' . esc_html__( 'All Products', 'simple-wholesale-discounts' ) . '</span>';

			case 'products':
				$ids      = json_decode( $item->product_ids, true );
				$ids      = is_array( $ids ) ? array_map( 'absint', $ids ) : array();
				$names    = array();
				foreach ( array_slice( $ids, 0, 3 ) as $pid ) {
					$product = wc_get_product( $pid );
					if ( $product ) {
						$names[] = esc_html( $product->get_name() );
					}
				}
				$output = implode( ', ', $names );
				if ( count( $ids ) > 3 ) {
					$output .= ' <em>+' . ( count( $ids ) - 3 ) . ' ' . esc_html__( 'more', 'simple-wholesale-discounts' ) . '</em>';
				}
				return $output ?: '<em>' . esc_html__( 'None selected', 'simple-wholesale-discounts' ) . '</em>';

			case 'categories':
				$ids   = json_decode( $item->category_ids, true );
				$ids   = is_array( $ids ) ? array_map( 'absint', $ids ) : array();
				$names = array();
				foreach ( array_slice( $ids, 0, 3 ) as $tid ) {
					$term = get_term( $tid, 'product_cat' );
					if ( $term && ! is_wp_error( $term ) ) {
						$names[] = esc_html( $term->name );
					}
				}
				$output = implode( ', ', $names );
				if ( count( $ids ) > 3 ) {
					$output .= ' <em>+' . ( count( $ids ) - 3 ) . ' ' . esc_html__( 'more', 'simple-wholesale-discounts' ) . '</em>';
				}
				return $output ?: '<em>' . esc_html__( 'None selected', 'simple-wholesale-discounts' ) . '</em>';

			default:
				return '—';
		}
	}

	/**
	 * Render the quantity_range column.
	 *
	 * @param object $item Current row rule object.
	 * @return string
	 */
	public function column_quantity_range( $item ) {
		$min = (int) $item->min_quantity;
		$max = (int) $item->max_quantity;

		if ( 0 === $min && 0 === $max ) {
			return esc_html__( 'Any quantity', 'simple-wholesale-discounts' );
		}

		$min_label = $min > 0 ? sprintf( 'Min: %d', $min ) : esc_html__( 'No min', 'simple-wholesale-discounts' );
		$max_label = $max > 0 ? sprintf( 'Max: %d', $max ) : esc_html__( 'No max', 'simple-wholesale-discounts' );

		return esc_html( $min_label ) . ' / ' . esc_html( $max_label );
	}

	/**
	 * Render the status (is_active) column as a clickable toggle.
	 *
	 * The toggle is AJAX-powered — clicking it calls swd_toggle_rule_status
	 * without reloading the page.
	 *
	 * @param object $item Current row rule object.
	 * @return string
	 */
	public function column_is_active( $item ) {
		$is_active = (int) $item->is_active;
		$label     = $is_active ? __( 'Active', 'simple-wholesale-discounts' ) : __( 'Inactive', 'simple-wholesale-discounts' );
		$class     = $is_active ? 'swd-status-toggle active' : 'swd-status-toggle inactive';

		return sprintf(
			'<button type="button" class="%s" data-rule-id="%d" data-current-status="%d" aria-label="%s">%s</button>',
			esc_attr( $class ),
			absint( $item->id ),
			$is_active,
			esc_attr( $label ),
			esc_html( $label )
		);
	}

	/**
	 * Render the actions column.
	 *
	 * @param object $item Current row rule object.
	 * @return string
	 */
	public function column_actions( $item ) {
		$edit_url   = SWD_Admin::edit_rule_url( $item->id );
		$delete_url = SWD_Admin::delete_rule_url( $item->id );

		return '<a href="' . esc_url( $edit_url ) . '" class="button button-small">' . esc_html__( 'Edit', 'simple-wholesale-discounts' ) . '</a> '
			. '<a href="' . esc_url( $delete_url ) . '" class="button button-small swd-delete-rule" data-confirm="' . esc_attr__( 'Are you sure you want to delete this rule?', 'simple-wholesale-discounts' ) . '">' . esc_html__( 'Delete', 'simple-wholesale-discounts' ) . '</a>';
	}

	/**
	 * Default column renderer for any column without a specific method.
	 *
	 * @param object $item        Current row rule object.
	 * @param string $column_name Column key.
	 * @return string
	 */
	public function column_default( $item, $column_name ) {
		return isset( $item->$column_name ) ? esc_html( $item->$column_name ) : '—';
	}

	/**
	 * Shown when there are no items in the table.
	 */
	public function no_items() {
		// Empty state is rendered by the template — output nothing here.
	}
}
