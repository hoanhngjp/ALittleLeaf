// Lấy thẻ input số lượng và nút +/- từ DOM
var quantityInput = document.getElementById('quantity');
var plusBtn = document.getElementById('plusBtn');
var minusBtn = document.getElementById('minusBtn');

// Hàm kiểm tra và sửa lỗi input
function validateInput() {
    let value = parseInt(quantityInput.value);

    // Nếu không phải số hoặc nhỏ hơn 1 -> gán lại bằng 1
    if (isNaN(value) || value < 1) {
        quantityInput.value = 1;
    }
}

// 1. Chặn nhập ký tự không phải số (chỉ cho nhập 0-9)
quantityInput.addEventListener('keypress', function (e) {
    if (!/[0-9]/.test(e.key)) {
        e.preventDefault();
    }
});

// 2. Xử lý khi người dùng gõ xong hoặc paste dữ liệu sai (sự kiện input hoặc blur)
quantityInput.addEventListener('blur', function () {
    validateInput();
});

// 3. Logic nút cộng
plusBtn.addEventListener('click', function () {
    quantityInput.value = parseInt(quantityInput.value) + 1;
});

// 4. Logic nút trừ
minusBtn.addEventListener('click', function () {
    let currentValue = parseInt(quantityInput.value);
    if (currentValue > 1) {
        quantityInput.value = currentValue - 1;
    } else {
        quantityInput.value = 1; // Đảm bảo không bao giờ xuống dưới 1
    }
});