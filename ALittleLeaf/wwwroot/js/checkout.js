$(document).ready(function() {
    $('#checkout_complete').submit(function(event) {
        event.preventDefault(); // Ngăn chặn form submit mặc định

        var selectedMethod = $('input[name="checkoutMethod"]:checked').val();
        $.ajax({
            url: './function/save_checkout_method.php',
            type: 'POST',
            data: { checkoutMethod: selectedMethod },
            success: function(response) {
                console.log(response); // Xử lý phản hồi từ server nếu cần
                $('#checkout_complete').unbind('submit').submit(); // Tiếp tục submit form sau khi lưu vào session
                window.location.href = "./account.php";
            },
            error: function(xhr, status, error) {
                console.error(xhr.responseText);
            }
        });
    });
});
