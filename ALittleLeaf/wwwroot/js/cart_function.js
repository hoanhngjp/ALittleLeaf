document.addEventListener('DOMContentLoaded', function() {
    let quantityInputs = document.querySelectorAll('.item-quantity');

    quantityInputs.forEach(function(input) {
        input.addEventListener('change', function() {
            updateCartItem(this);
        });
    });

    let qtyMinusButtons = document.querySelectorAll('.qtyminus');
    let qtyPlusButtons = document.querySelectorAll('.qtyplys');

    qtyMinusButtons.forEach(function(button) {
        button.addEventListener('click', function() {
            let input = this.nextElementSibling;
            if (parseInt(input.value) > 1) {
                input.value = parseInt(input.value) - 1;
                updateCartItem(input);
            }
        });
    });

    qtyPlusButtons.forEach(function(button) {
        button.addEventListener('click', function() {
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

    // Tính toán tổng giá của sản phẩm
    let lineItemTotal = quantity * itemPrice;

    // Hiển thị giá của sản phẩm trong hàng trên giao diện
    let lineItemTotalElement = input.parentNode.nextElementSibling.querySelector('.line-iem-total');
    lineItemTotalElement.textContent = formatCurrency(lineItemTotal);

    // Gửi yêu cầu cập nhật giỏ hàng đến máy chủ
    let formData = new FormData();
    formData.append('product_id', productId);
    formData.append('quantity', quantity);

    fetch('./function/cart_function.php', {
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
        let totalPriceElement = document.querySelector('.total-price b');
        totalPriceElement.textContent = formatCurrency(data.total_price);
        location.reload();
    })
    .catch(error => {
        console.error('There has been a problem with your fetch operation:', error);
    });
}

function formatCurrency(amount) {
    return amount.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' });
}



