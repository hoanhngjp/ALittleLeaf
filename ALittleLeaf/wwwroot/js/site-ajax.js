$(document).ready(function () {

    // --- LOGIC XÓA KHỎI GIỎ HÀNG (AJAX) ---
    $('body').on('click', '.remove-from-cart-ajax', function (e) {
        e.preventDefault();
        var $thisButton = $(this);
        var productId = $thisButton.data('product-id');
        var $row = $thisButton.closest('.item-in-cart, .line-item-container');

        $.ajax({
            url: '/Cart/RemoveFromCart',
            type: 'POST',
            data: { productId: productId },
            success: function (response) {
                // response: { success, totalPrice, totalItems }
                if (response.success) {

                    // 1. Xóa hàng <tr>
                    $row.fadeOut(300, function () {
                        $(this).remove();

                        // 2. KIỂM TRA NẾU GIỎ HÀNG RỖNG (SAU KHI XÓA)
                        if (response.totalItems === 0) {

                            // Ẩn div giỏ hàng đầy (form, nút checkout, v.v.)
                            $('.cart-container').hide();

                            // Hiện div "Giỏ hàng của bạn đang trống"
                            $('.notifications').show();

                            // Cập nhật lại cho SideNav (nếu nó đang mở)
                            if ($('#cart-inside tbody tr').length === 0) {
                                $('#cart-inside tbody').html('<tr id="cart-empty-message"><td colspan="2">Hiện chưa có sản phẩm nào</td></tr>');
                            }
                        }
                    });

                    // 3. Cập nhật số lượng trên Header
                    $('.count').text(response.totalItems);

                    $('.total-items').text("Giỏ hàng (" + response.totalItems + ")");

                    // 4. Cập nhật text "Có X sản phẩm"
                    $('.count-cart span').text(response.totalItems + " sản phẩm");

                    // 5. Cập nhật Tổng tiền (ở cả SideNav và trang Cart)
                    var formattedTotal = response.totalPrice.toLocaleString('vi-VN') + '₫';
                    $('.cart-total-display').text(formattedTotal);
                }
            },
            error: function () {
                alert("Đã xảy ra lỗi. Vui lòng thử lại.");
            }
        });
    });

    // --- LOGIC CẬP NHẬT SỐ LƯỢNG (Tăng/Giảm) ---
    $('body').on('click', '.qty-btn', function () {
        var $button = $(this);
        var productId = $button.data('product-id');
        var $input = $button.closest('.qty-click').find('.item-quantity[data-product-id="' + productId + '"]');
        var currentQty = parseInt($input.val());
        var newQty = currentQty;

        if ($button.hasClass('qtyplus')) {
            newQty += 1;
        } else if ($button.hasClass('qtyminus')) {
            newQty -= 1;
        }

        if (newQty < 1) newQty = 1;

        $input.val(newQty);
        updateCartItem(productId, newQty);
    });

    // Hàm gọi AJAX chung để cập nhật
    function updateCartItem(productId, quantity) {
        $.ajax({
            url: '/Cart/UpdateCartItem',
            type: 'POST',
            data: {
                productId: productId,
                quantity: quantity
            },
            success: function (response) {
                if (response.success) {
                    var formattedLineTotal = response.lineItemTotal.toLocaleString('vi-VN') + '₫';
                    var formattedTotal = response.totalPrice.toLocaleString('vi-VN') + '₫';

                    // Cập nhật "Thành tiền" của dòng (trên trang Cart/Index)
                    $('.line-item-total-display[data-product-id="' + productId + '"]').text(formattedLineTotal);

                    // Cập nhật "Thành tiền" (trên SideNav)
                    $('#cart-inside .item-in-cart[data-product-id="' + productId + '"] .pro-price-cart').text(formattedLineTotal);
                    // Cập nhật "Số lượng" (trên SideNav)
                    $('#cart-inside .item-in-cart[data-product-id="' + productId + '"] .pro-quantity-cart').text(quantity);

                    // Cập nhật "Tổng tiền" (ở cả SideNav và trang Cart)
                    $('.cart-total-display').text(formattedTotal);

                    // Cập nhật số lượng trên Header
                    $('.count').text(response.totalItems);

                    $('.total-items').text("Giỏ hàng (" + response.totalItems + ")");

                    // Cập nhật số lượng trên trang Cart/Index
                    $('.count-cart span').text(response.totalItems + " sản phẩm");
                }
            },
            error: function () {
                alert("Lỗi khi cập nhật giỏ hàng.");
            }
        });
    }

    // --- LOGIC TÌM KIẾM (AJAX) ---
    var searchTimer;
    $('#live-search-input').keyup(function () {
        var keyword = $(this).val();
        clearTimeout(searchTimer);

        if (keyword) {
            searchTimer = setTimeout(function () {
                $.ajax({
                    url: '/Search/Search',
                    type: 'POST',
                    data: { q: keyword },
                    success: function (response) {
                        $('#live-search-results').html(response);
                    },
                    error: function () {
                        $('#live-search-results').html('<p>Lỗi khi tìm kiếm.</p>');
                    }
                });
            }, 300);
        } else {
            $('#live-search-results').html('');
        }
    });

});