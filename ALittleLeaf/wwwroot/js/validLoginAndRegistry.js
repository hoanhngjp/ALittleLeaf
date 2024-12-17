// Biến để lưu trữ trường nhập liệu hiện đang chứa lỗi
var currentErrorField = null; 
function convertDateFormat(isoDate) {
    // Tách chuỗi yyyy-MM-dd
    var parts = isoDate.split("-");
    return parts[1] + "/" + parts[2] + "/" + parts[0]; // Trả về định dạng mm/dd/yyyy
}

function validateBirthday(birthday) {
    // Kiểm tra định dạng mm/dd/yyyy
    var birthdayPattern = /^(0[1-9]|1[0-2])\/(0[1-9]|[12][0-9]|3[01])\/\d{4}$/;

    if (!birthdayPattern.test(birthday)) {
        return birthdayPattern.test(birthday);
    }

    // Tách ngày, tháng, năm từ chuỗi
    var parts = birthday.split("/");
    var month = parseInt(parts[0], 10);
    var day = parseInt(parts[1], 10);
    var year = parseInt(parts[2], 10);

    // Kiểm tra tính hợp lệ của ngày
    var date = new Date(year, month - 1, day); // Tháng trong JavaScript bắt đầu từ 0
    return (
        date.getFullYear() === year &&
        date.getMonth() === month - 1 &&
        date.getDate() === day
    );
}
function validateEmail(email) {
    var emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

    return emailPattern.test(email);
}

function checkStrongPass(password) {
    // Kiểm tra độ dài của mật khẩu
    if (password.length < 8) {
        document.getElementById('password-error').innerHTML = "*Mật khẩu phải dài hơn 8 ký tự";
        document.getElementById('password-error').style.display = "block";
        currentErrorField = document.getElementById('password');
        return false;
    }

    // Kiểm tra xem mật khẩu có chứa ít nhất một chữ cái viết hoa
    if (!/[A-Z]/.test(password)) {
        document.getElementById('password-error').innerHTML = "*Mật khẩu phải có chữ ký tự viết hoa";
        document.getElementById('password-error').style.display = "block";
        currentErrorField = document.getElementById('password');
        return false;    }

    // Kiểm tra xem mật khẩu có chứa ít nhất một chữ cái viết thường
    if (!/[a-z]/.test(password)) {
        document.getElementById('password-error').innerHTML = "*Mật khẩu phải có chữ ký tự viết thường";
        document.getElementById('password-error').style.display = "block";
        currentErrorField = document.getElementById('password');
        return false;
    }

    // Kiểm tra xem mật khẩu có chứa ít nhất một số
    if (!/\d/.test(password)) {
        document.getElementById('password-error').innerHTML = "*Mật khẩu phải có ít nhất 1 chữ số";
        document.getElementById('password-error').style.display = "block";
        currentErrorField = document.getElementById('password');
        return false;
    }

    // Kiểm tra xem mật khẩu có chứa ít nhất một ký tự đặc biệt
    if (!/[@$!%*?&]/.test(password)) {
        document.getElementById('password-error').innerHTML = "*Mật khẩu phải có ít nhất 1 ký tự đặc biệt";
        document.getElementById('password-error').style.display = "block";
        currentErrorField = document.getElementById('password');
        return false;
    }

    return true;
}

function validRegistry(fullname, birthday, email, password, address) {
    if (fullname == "") {
        document.getElementById('fullname-error').style.display = "block";
        currentErrorField = document.getElementById('fullname');
        return false;
    }

    if (birthday == "") {
        document.getElementById('birthday-error').innerHTML = "*Vui lòng nhập Ngày sinh của bạn";
        document.getElementById('birthday-error').style.display = "block";
        currentErrorField = document.getElementById('birthday');
        return false;
    }

    birthday = convertDateFormat(birthday);

    //Kiểm tra định dạng ngày sinh của người dùng
    if (!validateBirthday(birthday)) {
        document.getElementById('birthday-error').innerHTML = "*Vui lòng nhập đúng định dạng Ngày sinh mm/dd/yyyy";
        document.getElementById('birthday-error').style.display = "block";
        currentErrorField = document.getElementById('birthday');
        return false;
    }
    
    if (email == "") {
        document.getElementById('email-error').innerHTML = "*Vui lòng nhập Email của bạn";
        document.getElementById('email-error').style.display = "block";
        currentErrorField = document.getElementById('email');
        return false;
    }

    //Kiểm tra định dạng email của người dùng
    if (!validateEmail(email)) {
        document.getElementById('email-error').innerHTML = "*Vui lòng nhập đúng định dạng Email";
        document.getElementById('email-error').style.display = "block";
        currentErrorField = document.getElementById('email');
        return false;
    }

    if (password == "") {
        document.getElementById('password-error').innerHTML = "*Vui lòng nhập Mật khẩu của bạn";
        document.getElementById('password-error').style.display = "block";
        currentErrorField = document.getElementById('password');
        return false;
    }

    //Kiểm tra mật khẩu mạnh
    if (!checkStrongPass(password)) {
        currentErrorField = document.getElementById('password');
        return false;
    }

    if (address == "") {
        document.getElementById('address-error').innerHTML = "*Vui lòng nhập Địa chỉ của bạn";
        document.getElementById('address-error').style.display = "block";
        currentErrorField = document.getElementById('address');
        return false;
    }

    return true;
}

function checkRegistry() {
    
    document.querySelectorAll(".show-error").forEach(function(element) {
        element.style.display = "none";
    });

    //Biến nhận các thông tin
    var fullname = document.getElementById('fullname').value;
    var birthday = document.getElementById('birthday').value;
    var email = document.getElementById('email').value;
    var password = document.getElementById('password').value;
    var address = document.getElementById('address').value;

    //Kiểm tra người dùng đã nhập đầy đủ thông tin chưa
    if (!validRegistry(fullname, birthday, email, password, address)) {
        currentErrorField.focus();
        return false;
    }
    
    return true;
}

function checkLogin() {
    document.querySelectorAll(".show-error").forEach(function(element) {
        element.style.display = "none";
    });

    var email = document.getElementById('email').value;
    var password = document.getElementById('password').value;

    if (email == "") {
        currentErrorField = document.getElementById('email');
        document.getElementById('error-email').style.display = "block";
        currentErrorField.focus();
        return false;
    }

    if (password == "") {
        currentErrorField = document.getElementById('password');
        document.getElementById('error-password').style.display = "block";
        currentErrorField.focus();
        return false;
    }

    return true;
}