/* =========================================================
   ROSALIE COFFEE
   GLOBAL APP TOAST
========================================================= */

let rosalieToastTimer = null;


/* =========================================================
   SHOW TOAST

   Backward compatible:
   showToast("Đăng xuất thành công!")

   Có thể dùng:
   showToast("Sai mật khẩu", "error")
========================================================= */

function showToast(
    message,
    type = "success",
    title = null,
    duration = 4000
) {

    const toast =
        document.getElementById(
            "rosalieToast"
        );


    const toastIcon =
        document.getElementById(
            "rosalieToastIcon"
        );


    const toastTitle =
        document.getElementById(
            "rosalieToastTitle"
        );


    const toastMessage =
        document.getElementById(
            "rosalieToastMessage"
        );


    const progress =
        document.getElementById(
            "rosalieToastProgress"
        );


    if (
        !toast
        ||
        !toastIcon
        ||
        !toastTitle
        ||
        !toastMessage
        ||
        !progress
    ) {
        return;
    }


    /* =====================================================
       CLEAR OLD TIMER
    ===================================================== */

    if (rosalieToastTimer) {

        clearTimeout(
            rosalieToastTimer
        );

    }


    /* =====================================================
       TYPE
    ===================================================== */

    const validTypes =
        [
            "success",
            "error",
            "warning",
            "info"
        ];


    if (!validTypes.includes(type)) {

        type =
            "success";

    }


    toast.classList.remove(
        "success",
        "error",
        "warning",
        "info",
        "show"
    );


    toast.classList.add(
        type
    );


    /* =====================================================
       CONFIG
    ===================================================== */

    let defaultTitle =
        "Thành công";


    let iconClass =
        "fa-solid fa-check";


    switch (type) {

        case "error":

            defaultTitle =
                "Có lỗi xảy ra";

            iconClass =
                "fa-solid fa-xmark";

            break;


        case "warning":

            defaultTitle =
                "Thông báo";

            iconClass =
                "fa-solid fa-exclamation";

            break;


        case "info":

            defaultTitle =
                "Thông tin";

            iconClass =
                "fa-solid fa-info";

            break;


        default:

            defaultTitle =
                "Thành công";

            iconClass =
                "fa-solid fa-check";

            break;

    }


    /* =====================================================
       CONTENT
    ===================================================== */

    toastTitle.textContent =
        title || defaultTitle;


    toastMessage.textContent =
        message || "";


    toastIcon.innerHTML =
        `<i class="${iconClass}"></i>`;


    /* =====================================================
       RESET PROGRESS
    ===================================================== */

    progress.style.animation =
        "none";


    void progress.offsetWidth;


    progress.style.animation =
        `rosalieToastProgress ${duration}ms linear forwards`;


    /* =====================================================
       SHOW
    ===================================================== */

    requestAnimationFrame(
        function () {

            toast.classList.add(
                "show"
            );

        }
    );


    /* =====================================================
       AUTO HIDE
    ===================================================== */

    rosalieToastTimer =
        setTimeout(
            function () {

                hideToast();

            },
            duration
        );
}


/* =========================================================
   HIDE
========================================================= */

function hideToast() {

    const toast =
        document.getElementById(
            "rosalieToast"
        );


    if (!toast) {
        return;
    }


    toast.classList.remove(
        "show"
    );


    if (rosalieToastTimer) {

        clearTimeout(
            rosalieToastTimer
        );


        rosalieToastTimer =
            null;

    }

}


/* =========================================================
   CLOSE BUTTON
========================================================= */

document.addEventListener(
    "DOMContentLoaded",
    function () {

        const closeButton =
            document.getElementById(
                "rosalieToastClose"
            );


        closeButton?.addEventListener(
            "click",
            function () {

                hideToast();

            }
        );

    }
);