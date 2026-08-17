const appToast = document.getElementById("appToast");
const toastTitle = document.getElementById("toastTitle");
const toastMessage = document.getElementById("toastMessage");
const toastCloseBtn = document.getElementById("toastCloseBtn");

let appToastTimer = null;


/* =========================================================
   SHOW TOAST
========================================================= */

function showToast(message, title = "Thành công") {

    // Nếu trang hiện tại không có toast thì dừng
    if (!appToast || !toastTitle || !toastMessage) {
        return;
    }


    // Nội dung
    toastTitle.textContent = title;
    toastMessage.innerHTML = message;


    // Hiện toast
    appToast.classList.add("show");


    // Nếu toast trước đang chạy thì hủy timer cũ
    if (appToastTimer) {
        clearTimeout(appToastTimer);
    }


    // Sau 2 giây tự biến mất
    appToastTimer = setTimeout(function () {

        hideToast();

    }, 2000);
}


/* =========================================================
   HIDE TOAST
========================================================= */

function hideToast() {

    if (!appToast) {
        return;
    }

    appToast.classList.remove("show");


    if (appToastTimer) {

        clearTimeout(appToastTimer);

        appToastTimer = null;
    }
}


/* =========================================================
   CLOSE BUTTON
========================================================= */

if (toastCloseBtn) {

    toastCloseBtn.addEventListener("click", function () {

        hideToast();

    });
}


/* =========================================================
   EXPORT GLOBAL FUNCTION
========================================================= */

window.showToast = showToast;
window.hideToast = hideToast;