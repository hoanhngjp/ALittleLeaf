document.addEventListener("DOMContentLoaded", function() {
    var showEditButtons = document.querySelectorAll(".showEdit");

    showEditButtons.forEach(function(button) {
        button.addEventListener("click", function(event) {
            event.preventDefault();
            var parentDiv = button.closest('.address-table-wrap');
            var viewAddress = parentDiv.querySelector(".view_address");
            var editAddress = parentDiv.querySelector(".edit_address");

            if (viewAddress.style.display === "none") {
                viewAddress.style.display = "block";
                editAddress.style.display = "none";
            } else {
                viewAddress.style.display = "none";
                editAddress.style.display = "block";
            }
        });
    });
});

var showAddNewAddress = document.getElementById("add-new-address");
var addAddressDiv = document.getElementById("add_address");

document.addEventListener("DOMContentLoaded", function() {
    showAddNewAddress.addEventListener("click", function(event){
        event.preventDefault;
        if (addAddressDiv.style.display == "none"){
            addAddressDiv.style.display = "block";
        } else{
            addAddressDiv.style.display = "none";
        }
    })
})

$(document).on('submit', '#address_form', function (e) {
    e.preventDefault();

    // Lấy trạng thái checkbox và giá trị
    var isChecked = $('#address_default_address_new').is(':checked'); // true/false
    var formData = $(this).serialize(); // Lấy dữ liệu từ form

    console.log("Checkbox checked:", isChecked); // true nếu được chọn
    console.log("Form Data:", formData); // Xem toàn bộ dữ liệu gửi đi

    $.ajax({
        url: '/Address/EditAddress',
        method: 'POST',
        data: formData,
        success: function (response) {
            if (response.success) {
                alert(response.message);
                location.reload();
            } else {
                alert(response.message);
            }
        },
        error: function () {
            alert('An error occurred. Please try again.');
        }
    });
});



