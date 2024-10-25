document.addEventListener("DOMContentLoaded", function() {
    var deleteButtons = document.querySelectorAll('.delete-address');

    deleteButtons.forEach(function(button) {
        button.addEventListener('click', function(event) {
            event.preventDefault();
            
            var addressId = this.getAttribute('data-address-id');

            if (confirm("Bạn có chắc chắn muốn xóa địa chỉ này không?")) {
                var xhr = new XMLHttpRequest();
                xhr.open('POST', './function/delete_address.php', true);
                xhr.setRequestHeader('Content-type', 'application/x-www-form-urlencoded');
                xhr.onload = function() {
                    if (xhr.status == 200) {
                        // Xóa địa chỉ khỏi giao diện sau khi xóa thành công
                        button.closest('.address-table-wrap').remove();
                        
                        // Hiển thị thông báo thành công
                        alert(xhr.responseText);
                    } else {
                        alert('Đã có lỗi xảy ra, vui lòng thử lại sau.');
                    }
                };
                
                xhr.send('address_id=' + addressId);
            }
        });
    });
});
