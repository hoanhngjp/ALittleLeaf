var currentErrorField = null; 

function checkLogin(){

    document.querySelectorAll(".show-error").forEach(function(element) {
        element.style.display = "none";
    });

    var email = document.getElementById('email').value;
    var password = document.getElementById('password').value;

    if (email == "") {
        currentErrorField = document.getElementById('email');
        document.getElementById('error-username').style.display = "block";
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