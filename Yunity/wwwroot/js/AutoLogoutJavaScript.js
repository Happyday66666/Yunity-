let timeout;

// 設置登出時間 (以毫秒為單位)
const logoutTime = 30 * 60 * 1000; // 30 分鐘

// 監控用戶操作的事件
function resetTimer() {
    clearTimeout(timeout);
    timeout = setTimeout(async () => {
        alert("您已長時間未操作，將自動登出！");

        // 使用 Fetch 發送登出請求（改為 GET 方法）
        try {
            const response = await fetch('/Home/Logout', {
                method: 'GET', // 改為 GET 請求
                headers: {
                    'X-Requested-With': 'XMLHttpRequest' // 標記為 Ajax 請求
                }
            });

            if (response.ok) {
                console.log("用戶已成功登出");
                // 登出成功後跳轉到登入頁
                window.location.href = '/Home/Login'; // 可根據需要跳轉到登入頁
            } else {
                console.error("登出失敗");
            }
        } catch (error) {
            console.error("登出請求失敗：", error);
        }
    }, logoutTime);
}

// 監聽所有的用戶操作
window.onload = resetTimer;
document.onmousemove = resetTimer;
document.onkeydown = resetTimer;
document.ontouchstart = resetTimer; // 支援觸控設備


