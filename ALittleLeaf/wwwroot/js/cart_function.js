document.addEventListener('DOMContentLoaded', function () {
    let quantityInputs = document.querySelectorAll('.item-quantity');
    quantityInputs.forEach(function (input) {
        input.addEventListener('change', function () {
            // Đảm bảo số lượng không nhỏ hơn 1
            if (parseInt(input.value) < 1 || isNaN(input.value)) {
                input.value = 1;
            }
            updateCartItem(input);
        });
    });

    let qtyMinusButtons = document.querySelectorAll('.qtyminus');
    let qtyPlusButtons = document.querySelectorAll('.qtyplus');

    qtyMinusButtons.forEach(function (button) {
        button.addEventListener('click', function () {
            let input = this.nextElementSibling;
            if (parseInt(input.value) > 1) {
                input.value = parseInt(input.value) - 1;
                updateCartItem(input);
            }
        });
    });

    qtyPlusButtons.forEach(function (button) {
        button.addEventListener('click', function () {
            let input = this.previousElementSibling;
            input.value = parseInt(input.value) + 1;
            updateCartItem(input);
        });
    });
});

function updateCartItem(input) {
    let productId = input.dataset.productId;
    let quantity = parseInt(input.value);
    let itemPrice = parseFloat(input.dataset.price);

    // Kiểm tra giá trị của productId và quantity
    console.log("Product ID: " + productId);
    console.log("Quantity: " + quantity);

    // Tính toán tổng giá của sản phẩm
    let lineItemTotal = quantity * itemPrice;

    // Hiển thị giá của sản phẩm trong hàng trên giao diện
    let lineItemTotalElement = input.closest('tr').querySelector('.line-item-total');
    if (lineItemTotalElement) {
        lineItemTotalElement.textContent = formatCurrency(lineItemTotal);
    }

    // Gửi yêu cầu cập nhật giỏ hàng đến máy chủ
    let formData = new FormData();
    formData.append('productId', productId);
    formData.append('quantity', quantity);

    fetch('/Cart/UpdateCartItem', {
        method: 'POST',
        body: formData
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            if (data.success) {
                // Cập nhật tổng tiền giỏ hàng
                let totalPriceElement = document.querySelector('.total-price b');
                if (totalPriceElement) {
                    totalPriceElement.textContent = formatCurrency(data.total_price);
                }
                location.reload();
            } else {
                console.error('Cập nhật không thành công:', data.message);
            }
        })
        .catch(error => {
            console.error('There has been a problem with your fetch operation:', error);
        });
}
function formatCurrency(amount) {
    if (isNaN(amount)) {
        console.error('Giá trị không hợp lệ:', amount);
        return '0₫'; // Giá trị mặc định nếu không hợp lệ
    }
    return amount.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' });
}
document.addEventListener('DOMContentLoaded', function () {
    let removeButtons = document.querySelectorAll('.remove-from-cart');

    removeButtons.forEach(function (button) {
        button.addEventListener('click', function (event) {
            event.preventDefault();  // Ngừng hành động mặc định của liên kết

            let productId = this.dataset.productId;  // Lấy productId từ data-product-id
            console.log("Product ID to remove:", productId);  // Kiểm tra ID sản phẩm

            // Gửi yêu cầu xóa sản phẩm từ giỏ hàng
            removeFromCart(productId);
        });
    });
});

function removeFromCart(productId) {
    let formData = new FormData();
    formData.append('productId', productId);  // Thêm ID sản phẩm vào FormData

    fetch('/Cart/RemoveFromCart', {
        method: 'POST',
        body: formData
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                // Nếu thành công, cập nhật lại giỏ hàng
                alert("Sản phẩm đã được xóa khỏi giỏ hàng");

                // Xóa sản phẩm khỏi giao diện
                let productRow = document.querySelector(`[data-product-id="${productId}"]`).closest('tr');
                if (productRow) {
                    productRow.remove();
                }

                // Cập nhật lại tổng tiền giỏ hàng nếu cần
                let totalPriceElement = document.querySelector('.total-price b');
                if (totalPriceElement) {
                    totalPriceElement.textContent = formatCurrency(data.total_price);
                }
                location.reload();
            } else {
                console.error('Lỗi khi xóa sản phẩm:', data.message);
            }
        })
        .catch(error => {
            console.error('Có lỗi xảy ra:', error);
        });
}

