/**
 * Simple Wholesale Discounts — Admin JavaScript
 *
 * Handles all interactive elements in the admin rule form and list table:
 *  1. Discount type card selection (Percentage / Flat).
 *  2. Apply-to card selection and conditional field visibility.
 *  3. AJAX product search with 300ms debounce and result rendering.
 *  4. Selected product tags (add / remove).
 *  5. Live discount value preview text.
 *  6. Live quantity range preview text.
 *  7. Inline rule status toggle AJAX on the list table.
 *  8. Delete confirmation on the list table.
 *
 * DROPDOWN POSITIONING STRATEGY
 * The search results dropdown uses position:fixed and is appended directly to
 * <body> at init time. This means no parent overflow:hidden can ever clip it.
 * Coordinates are set via getBoundingClientRect() so it stays anchored below
 * the search input regardless of page scroll.
 *
 * @package SimpleWholesaleDiscounts
 */

/* global swdAdmin, jQuery */
/* eslint-disable no-var */

(function ($) {
    'use strict';

    // =========================================================================
    // Constants from PHP (localized via wp_localize_script)
    // =========================================================================
    var AJAX_URL = swdAdmin.ajaxUrl;
    var SEARCH_NONCE = swdAdmin.searchNonce;
    var TOGGLE_NONCE = swdAdmin.toggleNonce;
    var CURRENCY = swdAdmin.currencySymbol;
    var I18N = swdAdmin.i18n;

    // =========================================================================
    // Discount Type Cards
    // =========================================================================

    function updateTypeCard(type) {
        $('.swd-type-card').each(function () {
            var cardType = $(this).find('input[type="radio"]').val();
            $(this).toggleClass('selected', cardType === type);
        });

        $('#discount_prefix').text(type === 'percentage' ? '%' : CURRENCY);
        updateDiscountPreview();
    }

    var initialType = $('input[name="discount_type"]:checked').val() || 'percentage';
    updateTypeCard(initialType);

    $('.swd-type-card').on('click', function () {
        var type = $(this).find('input[type="radio"]').val();
        $('input[name="discount_type"][value="' + type + '"]').prop('checked', true);
        updateTypeCard(type);
    });

    // =========================================================================
    // Live Discount Preview Text
    // =========================================================================

    function updateDiscountPreview() {
        var $preview = $('#discount_preview');
        if (!$preview.length) { return; }

        var value = parseFloat($('#discount_value').val()) || 0;
        var type = $('input[name="discount_type"]:checked').val() || 'percentage';

        if (value <= 0) {
            $preview.text('Enter a value above to see a preview.');
            return;
        }

        var msg = type === 'percentage'
            ? 'Customers will get ' + value + '% off'
            : 'Customers will get ' + CURRENCY + value.toFixed(2) + ' off';

        $preview.text(msg);
    }

    $('#discount_value').on('input', updateDiscountPreview);
    updateDiscountPreview();

    // =========================================================================
    // Apply-To Cards
    // =========================================================================

    function updateApplyToCard(applyTo) {
        $('.swd-apply-card').each(function () {
            var cardValue = $(this).find('input[type="radio"]').val();
            $(this).toggleClass('selected', cardValue === applyTo);
        });

        if (applyTo === 'products') {
            $('#swd-product-search-wrap').slideDown(200);
            $('#swd-category-list-wrap').slideUp(200);
        } else if (applyTo === 'categories') {
            $('#swd-category-list-wrap').slideDown(200);
            $('#swd-product-search-wrap').slideUp(200);
        } else {
            $('#swd-product-search-wrap').slideUp(200);
            $('#swd-category-list-wrap').slideUp(200);
        }
    }

    // Initialise immediately — no animation on page load.
    var initialApplyTo = $('input[name="apply_to"]:checked').val() || 'all';
    if (initialApplyTo === 'products') {
        $('#swd-product-search-wrap').show();
    } else if (initialApplyTo === 'categories') {
        $('#swd-category-list-wrap').show();
    }
    updateApplyToCard(initialApplyTo);

    $('.swd-apply-card').on('click', function () {
        var applyTo = $(this).find('input[type="radio"]').val();
        $('input[name="apply_to"][value="' + applyTo + '"]').prop('checked', true);
        updateApplyToCard(applyTo);
    });

    // =========================================================================
    // Product Search
    // =========================================================================

    var searchTimer = null;
    var $searchInput = $('#swd_product_search_input');
    var $searchDropdown = $('#swd_product_search_results');
    var $productIdsInput = $('#swd_product_ids');
    var $tagsContainer = $('#swd_selected_products');

    // -----------------------------------------------------------------
    // PORTAL: move the dropdown to <body> so it is never clipped by any
    // parent container's overflow setting.
    // -----------------------------------------------------------------
    if ($searchDropdown.length) {
        $searchDropdown.appendTo('body');
    }

    // -----------------------------------------------------------------
    // Positioning helper
    // Now that the dropdown lives in <body> we use position:fixed and
    // read the input's current viewport coordinates each time we open it.
    // -----------------------------------------------------------------
    function positionDropdown() {
        if (!$searchInput.length || !$searchDropdown.length) { return; }
        var rect = $searchInput[0].getBoundingClientRect();
        $searchDropdown.css({
            position: 'fixed',
            top: (rect.bottom + 4) + 'px',
            left: rect.left + 'px',
            width: rect.width + 'px',
        });
    }

    // Keep the dropdown anchored during scroll / resize.
    $(window).on('scroll.swd resize.swd', function () {
        if ($searchDropdown.is(':visible')) {
            positionDropdown();
        }
    });

    // -----------------------------------------------------------------
    // Selected products — pre-populate in edit mode.
    // -----------------------------------------------------------------
    var selectedProducts = {};

    if (typeof window.swdSelectedProducts !== 'undefined' && Array.isArray(window.swdSelectedProducts)) {
        window.swdSelectedProducts.forEach(function (product) {
            selectedProducts[product.id] = product;
        });
        renderTags();
    }

    // -----------------------------------------------------------------
    // Input handler — debounced 300 ms.
    // -----------------------------------------------------------------
    $searchInput.on('input', function () {
        clearTimeout(searchTimer);
        var term = $(this).val().trim();

        if (term.length < 2) {
            $searchDropdown.hide().empty();
            return;
        }

        searchTimer = setTimeout(function () {
            doProductSearch(term);
        }, 300);
    });

    // Close dropdown on outside click.
    $(document).on('click.swd', function (e) {
        var target = e.target;
        // Keep open if click is on the input, the wrapper, or inside the dropdown.
        if (
            $searchInput.is(target) ||
            $searchInput.closest('.swd-product-search').is(target) ||
            $searchDropdown.is(target) ||
            $searchDropdown.has(target).length
        ) {
            return;
        }
        $searchDropdown.hide();
    });

    // -----------------------------------------------------------------
    // AJAX product search.
    // -----------------------------------------------------------------
    function doProductSearch(term) {
        // Show "Searching..." and position the dropdown.
        positionDropdown();
        $searchDropdown
            .html('<div class="swd-search-searching">' + I18N.searching + '</div>')
            .show();

        $.ajax({
            url: AJAX_URL,
            method: 'POST',
            data: {
                action: 'swd_search_products',
                nonce: SEARCH_NONCE,
                term: term,
            },
            success: function (response) {
                if (!response.success || !response.data || response.data.length === 0) {
                    $searchDropdown.html('<div class="swd-search-no-results">' + I18N.noResults + '</div>');
                    return;
                }

                var html = '';
                response.data.forEach(function (product) {
                    if (selectedProducts[product.id]) { return; } // Already selected — skip.

                    html += '<div class="swd-search-result" tabindex="0" '
                        + 'data-id="' + product.id + '" '
                        + 'data-name="' + $('<div>').text(product.name).html() + '" '
                        + 'data-price="' + $('<div>').text(product.price).html() + '">'
                        + '<img src="' + product.thumbnail_url + '" alt="" />'
                        + '<div class="swd-search-result__info">'
                        + '<div class="swd-search-result__name">' + $('<div>').text(product.name).html() + '</div>'
                        + '<div class="swd-search-result__price">' + product.price + '</div>'
                        + '</div>'
                        + '</div>';
                });

                $searchDropdown.html(html || '<div class="swd-search-no-results">' + I18N.noResults + '</div>').show();
            },
            error: function () {
                $searchDropdown.html('<div class="swd-search-no-results">An error occurred. Please try again.</div>');
            },
        });
    }

    // Select a search result.
    $searchDropdown.on('click keydown', '.swd-search-result', function (e) {
        if (e.type === 'keydown' && e.key !== 'Enter' && e.key !== ' ') { return; }

        var $el = $(this);
        var id = parseInt($el.data('id'), 10);

        selectedProducts[id] = {
            id: id,
            name: $el.data('name'),
            price: $el.data('price'),
        };

        renderTags();
        $searchDropdown.hide().empty();
        $searchInput.val('').focus();
    });

    // Render selected product tags.
    function renderTags() {
        $tagsContainer.empty();

        Object.keys(selectedProducts).forEach(function (id) {
            var product = selectedProducts[id];
            $tagsContainer.append(
                $('<div class="swd-product-tag">'
                    + '<span class="swd-product-tag__name">' + $('<div>').text(product.name).html() + '</span>'
                    + '<button type="button" class="swd-product-tag__remove" data-id="' + id + '" aria-label="Remove">&times;</button>'
                    + '</div>'
                )
            );
        });

        // Update hidden comma-separated IDs input.
        $productIdsInput.val(Object.keys(selectedProducts).join(','));
    }

    // Remove a product tag.
    $tagsContainer.on('click', '.swd-product-tag__remove', function () {
        delete selectedProducts[$(this).data('id')];
        renderTags();
    });

    // =========================================================================
    // Live Quantity Range Preview
    // =========================================================================

    function updateQtyPreview() {
        var $preview = $('#qty_preview');
        if (!$preview.length) { return; }

        var min = parseInt($('#min_quantity').val(), 10) || 0;
        var max = parseInt($('#max_quantity').val(), 10) || 0;
        var msg;

        if (min === 0 && max === 0) {
            msg = 'This discount applies to any quantity.';
        } else if (min > 0 && max > 0) {
            msg = 'This discount applies when buying between ' + min + ' and ' + max + ' units.';
        } else if (min > 0) {
            msg = 'This discount applies when buying ' + min + ' or more units.';
        } else {
            msg = 'This discount applies when buying up to ' + max + ' units.';
        }

        $preview.text(msg);
    }

    $('#min_quantity, #max_quantity').on('input', updateQtyPreview);
    updateQtyPreview();

    // =========================================================================
    // Inline Status Toggle (List Table)
    // =========================================================================

    $('.swd-status-toggle').on('click', function () {
        var $btn = $(this);
        var ruleId = $btn.data('rule-id');
        var current = parseInt($btn.data('current-status'), 10);
        var newStatus = current ? 0 : 1;

        $btn.addClass('swd-loading').prop('disabled', true)
            .text(newStatus ? I18N.activating : I18N.deactivating);

        $.ajax({
            url: AJAX_URL,
            method: 'POST',
            data: {
                action: 'swd_toggle_rule_status',
                nonce: TOGGLE_NONCE,
                rule_id: ruleId,
                new_status: newStatus,
            },
            success: function (response) {
                if (response.success) {
                    var status = response.data.new_status;
                    $btn.removeClass('swd-loading active inactive')
                        .addClass(status ? 'active' : 'inactive')
                        .data('current-status', status)
                        .prop('disabled', false)
                        .text(response.data.label);
                } else {
                    $btn.removeClass('swd-loading').prop('disabled', false)
                        .text(current ? 'Active' : 'Inactive');
                }
            },
            error: function () {
                $btn.removeClass('swd-loading').prop('disabled', false)
                    .text(current ? 'Active' : 'Inactive');
            },
        });
    });

    // =========================================================================
    // Delete Confirmation
    // =========================================================================

    $(document).on('click', '.swd-delete-rule', function (e) {
        if (!window.confirm($(this).data('confirm') || I18N.confirmDelete)) {
            e.preventDefault();
        }
    });

    $('#doaction, #doaction2').on('click', function () {
        if ($(this).prev('select').val() === 'delete') {
            var $checked = $('input[name="rule[]"]:checked');
            if ($checked.length > 0 && !window.confirm(I18N.confirmBulkDelete)) {
                return false;
            }
        }
    });

})(jQuery);
