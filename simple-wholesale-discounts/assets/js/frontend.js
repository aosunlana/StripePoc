/**
 * Simple Wholesale Discounts — Frontend JavaScript
 *
 * Vanilla JS only — no jQuery dependency.
 * Loaded only on single product pages where needed.
 *
 * Currently provides:
 *  - Highlights the matching pricing table row when the quantity input changes
 *    on the product page (WooCommerce quantity field).
 *
 * @package SimpleWholesaleDiscounts
 */

(function () {
    'use strict';

    /**
     * Find the first element matching a selector within a scope.
     * @param {string} selector CSS selector.
     * @param {Element|Document} scope Parent element (default: document).
     * @returns {Element|null}
     */
    function qs(selector, scope) {
        return (scope || document).querySelector(selector);
    }

    // =========================================================================
    // Pricing Table — Highlight Active Row Based on Quantity Input
    // =========================================================================

    var pricingTable = qs('.swd-pricing-table');
    var qtyInput = qs('input.qty[name="quantity"]');

    if (pricingTable && qtyInput) {
        var rows = pricingTable.querySelectorAll('tbody tr');

        /**
         * Parse a quantity range label like "5 – 19" or "20+" into [min, max].
         * Returns [min, max] where max = Infinity for open-ended ranges.
         *
         * @param {string} label Table row quantity cell text.
         * @returns {[number, number]}
         */
        function parseRange(label) {
            label = label.trim();

            // Pattern: "5 – 19" (en-dash) or "5 - 19" (hyphen)
            var rangeParts = label.match(/^(\d+)\s*[–\-]\s*(\d+)$/);
            if (rangeParts) {
                return [parseInt(rangeParts[1], 10), parseInt(rangeParts[2], 10)];
            }

            // Pattern: "20+"
            var plusParts = label.match(/^(\d+)\+$/);
            if (plusParts) {
                return [parseInt(plusParts[1], 10), Infinity];
            }

            // Pattern: "1 – 4" first row (no discount).
            // Handled above by rangeParts.

            // Fallback — any quantity row.
            return [0, Infinity];
        }

        function highlightActiveRow() {
            var qty = parseInt(qtyInput.value, 10) || 1;

            rows.forEach(function (row) {
                row.classList.remove('swd-active-tier');
                var qtyCell = row.cells[0];
                if (!qtyCell) return;

                var range = parseRange(qtyCell.textContent);
                if (qty >= range[0] && qty <= range[1]) {
                    row.classList.add('swd-active-tier');
                }
            });
        }

        // Wire up the quantity input.
        qtyInput.addEventListener('input', highlightActiveRow);
        qtyInput.addEventListener('change', highlightActiveRow);

        // Run on page load.
        highlightActiveRow();

        // Also inject the active tier highlight style.
        var style = document.createElement('style');
        style.textContent = [
            '.swd-pricing-table tbody tr.swd-active-tier {',
            '  background: var(--swd-fe-primary-bg, #f0eef8) !important;',
            '  outline: 2px solid var(--swd-fe-primary, #7f54b3);',
            '  outline-offset: -2px;',
            '}',
        ].join('\n');
        document.head.appendChild(style);
    }

})();
