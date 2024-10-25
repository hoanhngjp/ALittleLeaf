var mainBody = document.getElementById('main-body');
var siteNav = document.getElementById('site-nav');
var openSearch = document.getElementById('open-site-search');
var openCart = document.getElementById('open-site-cart');
var siteSearch = document.getElementById('site-search');
var siteCart = document.getElementById('site-cart');
var siteCloseHandle = document.getElementById('site-close-handle');
var mainBody = document.getElementById('main-body');

document.addEventListener("DOMContentLoaded", function() {
    openSearch.addEventListener("click", function(event) {
        event.preventDefault();
        siteNav.classList.add("active");
        siteSearch.style.display = 'block';
    })


    openCart.addEventListener("click", function(event) {
        event.preventDefault();
        siteNav.classList.add("active");
        siteCart.style.display= 'block';
    })

    siteCloseHandle.addEventListener("click", function(event) {
        event.preventDefault();
        siteNav.classList.remove("active");
        siteSearch.classList.remove("showSearch");
        siteCart.classList.remove("showCart");
        siteSearch.style.display = 'none';
        siteCart.style.display= 'none';
    })
})