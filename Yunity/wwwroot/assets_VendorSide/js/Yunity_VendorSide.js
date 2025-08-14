/**
  * Scroll top button
  */
let scrollTop = document.querySelector('.scroll-top');

function toggleScrollTop() {
   if (scrollTop) {
      window.scrollY > 100 ? scrollTop.classList.add('active') : scrollTop.classList.remove('active');
   }
}
scrollTop.addEventListener('click', (e) => {
   e.preventDefault();
   window.scrollTo({
      top: 0,
      behavior: 'smooth'
   });
});

window.addEventListener('load', toggleScrollTop);
document.addEventListener('scroll', toggleScrollTop);


/**
  * Login visibility eye
  */
document.addEventListener('DOMContentLoaded', function () {
    const passwordInput = document.getElementById('passwordInput');
    const toggleVisibility = document.getElementById('toggleVisibility');

    toggleVisibility.addEventListener('click', () => {
        if (passwordInput.type === 'password') {
            passwordInput.type = 'text';
            toggleVisibility.textContent = 'visibility';
        } else {
            passwordInput.type = 'password';
            toggleVisibility.textContent = 'visibility_off';
        }
    });
});

/**
  * Menu visited
  */
document.addEventListener('DOMContentLoaded', function () {
    var currentUrl = window.location.pathname;
    currentUrl = currentUrl.replace(/\\/g, '/');

    var menuLinks = document.querySelectorAll('.menu-group a');

    menuLinks.forEach(function (link) {
        var linkHref = link.getAttribute('href');
        linkHref = linkHref.replace(/\\/g, '/');

        if (linkHref === currentUrl) {
            link.classList.add('visited');
        } else {
            link.classList.remove('visited');
        }
    });
});











