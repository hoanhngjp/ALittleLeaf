document.addEventListener("DOMContentLoaded", function() {
    const radioWrappers = document.querySelectorAll(".radio-wrapper.content-box-row");

    radioWrappers.forEach(function(radioWrapper) {
        radioWrapper.addEventListener("click", function() {
            // Tìm input radio bên trong
            const radioInput = radioWrapper.querySelector("input[type='radio']");

            // Chọn input radio trong radio wrapper đang được nhấn
            radioInput.checked = true;

            // Hiển thị content-box-row-secondary tương ứng (nếu có)
            const secondaryRow = radioWrapper.nextElementSibling;
            if (secondaryRow && secondaryRow.classList.contains("content-box-row-secondary")) {
                // Ẩn tất cả các content-box-row-secondary khác
                const allSecondaryRows = document.querySelectorAll(".content-box-row-secondary");
                allSecondaryRows.forEach(function(row) {
                    row.style.display = "none";
                });

                // Hiển thị content-box-row-secondary tương ứng
                secondaryRow.style.display = "block";
            }
        });
    });
});