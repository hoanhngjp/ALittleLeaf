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
