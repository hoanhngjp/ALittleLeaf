// Lấy thẻ input số lượng và nút +/- từ DOM
var quantityInput = document.getElementById('quantity');
var plusBtn = document.getElementById('plusBtn');
var minusBtn = document.getElementById('minusBtn');

// Thêm sự kiện click cho nút "+"
plusBtn.addEventListener('click', function() {
    // Tăng giá trị số lượng khi nhấn nút "+"
    quantityInput.value = parseInt(quantityInput.value) + 1;
});

// Thêm sự kiện click cho nút "-"
minusBtn.addEventListener('click', function() {
    // Giảm giá trị số lượng khi nhấn nút "-"
    if (parseInt(quantityInput.value) > 1) {
        quantityInput.value = parseInt(quantityInput.value) - 1;
    }
});