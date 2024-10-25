const nav = document.querySelector('nav');
const exitNav = document.querySelector('exitNav');
const bodyContent = document.querySelector('bodyContent');

exitNav.click = function(){
    nav.classList.toggle('hide');
    bodyContent.classList.toggle('expand');
}
