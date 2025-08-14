document.addEventListener("DOMContentLoaded", function () {
    const loginButton = document.getElementById("loginButton");
    const overlay = document.getElementById("overlay");
    const loginPopup = document.getElementById("loginPopup");

    // 打開登入視窗
    loginButton.addEventListener("click", function () {
        overlay.style.display = "block";
        loginPopup.style.display = "block";
    });

    // 點擊遮罩關閉登入視窗
    overlay.addEventListener("click", function () {
        overlay.style.display = "none";
        loginPopup.style.display = "none";
    });

    const backToTopButton = document.getElementById('backToTopButton');

    // 當頁面滾動出現按鈕
    window.onscroll = function () {
        if (document.body.scrollTop > 200 || document.documentElement.scrollTop > 200) {
            backToTopButton.style.display = "block";
        } else {
            backToTopButton.style.display = "none";
        }
    };

    // 點擊按鈕回到頂部
    backToTopButton.onclick = function () {
        window.scrollTo({
            top: 0,
            behavior: 'smooth'
        });
    };
});


