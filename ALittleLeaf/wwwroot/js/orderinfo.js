document.addEventListener('DOMContentLoaded', function () {
    let storedAddressesSelect = document.getElementById('stored_addresses');
    let fullNameInput = document.getElementById('billing_address_full_name');
    let phoneInput = document.getElementById('billing_address_phone');
    let addressInput = document.getElementById('billing_address_address');

    // Lắng nghe sự kiện khi người dùng thay đổi giá trị của select box
    storedAddressesSelect.addEventListener('change', function () {
        let selectedOption = storedAddressesSelect.options[storedAddressesSelect.selectedIndex];
        if (selectedOption.value == 'add') {
            // Nếu người dùng chọn Thêm địa chỉ mới, reset các trường input
            fullNameInput.value = '';
            phoneInput.value = '';
            addressInput.value = '';
        } else {
            // Nếu người dùng chọn một địa chỉ trong cơ sở dữ liệu, gửi yêu cầu AJAX để lấy thông tin địa chỉ
            let addressId = parseInt(selectedOption.value);
            console.log("Address ID sent: ", addressId); // Kiểm tra giá trị này
            getAddressInfo(addressId);
        }
    });

    // Hàm để gửi yêu cầu AJAX để lấy thông tin địa chỉ từ máy chủ 
    function getAddressInfo(addressId) {
        let formData = new FormData();
        formData.append('addressId', addressId);

        fetch('/OrderInfo/GetAddressInfo', {
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
                if (data.error) {
                    console.error(data.error);
                    alert(data.error);
                    return;
                }

                fullNameInput.value = data.adrs_fullname || '';
                phoneInput.value = data.adrs_phone || '';
                addressInput.value = data.adrs_address || '';
            })
            .catch(error => {
                console.error('There has been a problem with your fetch operation:', error);
            });
    }
});

function checkInput() {
    var fullName = document.getElementById('billing_address_full_name').value;
    var phone = document.getElementById('billing_address_phone').value;
    var address = document.getElementById('billing_address_address').value;
    var fullNameError = document.getElementById('fullname_error');
    var phoneError = document.getElementById('phone_error');
    var addressError = document.getElementById('address_error');

    // Reset thông báo lỗi trước khi kiểm tra lại
    fullNameError.style.display = 'none';
    phoneError.style.display = 'none';
    addressError.style.display = 'none';

    var hasError = false;

    // Kiểm tra từng trường input
    if (fullName === '') {
        fullNameError.style.display = 'block';
        fullNameError.style.color = 'red';
        document.getElementById('billing_address_full_name').style.borderColor = 'red';
        hasError = true;
    }

    if (phone === '') {
        phoneError.style.display = 'block';
        phoneError.style.color = 'red';
        document.getElementById('billing_address_phone').style.borderColor = 'red';
        hasError = true;
    }

    if (address === '') {
        addressError.style.display = 'block';
        addressError.style.color = 'red';
        document.getElementById('billing_address_address').style.borderColor = 'red';
        hasError = true;
    }

    // Nếu có lỗi, ngăn chặn sự kiện submit mặc định của form
    if (hasError) {
        return false;
    }
    return true;
}

