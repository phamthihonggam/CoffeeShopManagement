// ======================================================
// ROSALIE COFFEE
// ADMIN ORDER
// TOAST + UPDATE STATUS MODAL
// ======================================================

document.addEventListener("DOMContentLoaded", function () {

    // ==================================================
    // TOAST
    // ==================================================

    const toast =
        document.getElementById("adminToast");

    const toastClose =
        document.getElementById("adminToastClose");


    if (toast) {

        requestAnimationFrame(function () {

            toast.classList.add("show");

        });


        function hideToast() {

            toast.classList.remove("show");

            setTimeout(function () {

                toast.remove();

            }, 300);

        }


        toastClose?.addEventListener(
            "click",
            hideToast
        );


        setTimeout(
            hideToast,
            3000
        );

    }


    // ==================================================
    // UPDATE STATUS MODAL - ELEMENTS
    // ==================================================

    const modal =
        document.getElementById("orderStatusModal");

    const backdrop =
        document.getElementById("orderStatusBackdrop");

    const cancelButton =
        document.getElementById("orderStatusCancel");

    const orderIdInput =
        document.getElementById("orderStatusId");

    const orderIdText =
        document.getElementById("orderStatusOrderId");

    const statusSelect =
        document.getElementById("orderStatusSelect");

    const statusButtons =
        document.querySelectorAll(".js-order-status");


    // ==================================================
    // OPEN MODAL
    // ==================================================

    function openStatusModal(button) {

        if (!modal) {
            return;
        }


        const orderId =
            button.dataset.orderId;

        const currentStatus =
            button.dataset.currentStatus;


        // Gán mã đơn vào hidden input
        if (orderIdInput) {

            orderIdInput.value =
                orderId;

        }


        // Hiện mã đơn trên modal
        if (orderIdText) {

            orderIdText.textContent =
                "#" + orderId;

        }


        // Chọn trạng thái hiện tại
        if (statusSelect &&
            currentStatus) {

            statusSelect.value =
                currentStatus;

        }


        modal.classList.add(
            "show"
        );


        document.body.style.overflow =
            "hidden";

    }


    // ==================================================
    // CLOSE MODAL
    // ==================================================

    function closeStatusModal() {

        if (!modal) {
            return;
        }


        modal.classList.remove(
            "show"
        );


        document.body.style.overflow =
            "";

    }


    // ==================================================
    // STATUS BUTTON
    // ==================================================

    statusButtons.forEach(function (button) {

        button.addEventListener(
            "click",
            function () {

                openStatusModal(
                    button
                );

            }
        );

    });


    // ==================================================
    // CANCEL
    // ==================================================

    cancelButton?.addEventListener(
        "click",
        closeStatusModal
    );


    // ==================================================
    // CLICK BACKDROP
    // ==================================================

    backdrop?.addEventListener(
        "click",
        closeStatusModal
    );


    // ==================================================
    // ESC CLOSE
    // ==================================================

    document.addEventListener(
        "keydown",
        function (event) {

            if (
                event.key === "Escape" &&
                modal?.classList.contains("show")
            ) {

                closeStatusModal();

            }

        }
    );


    // ==================================================
    // FORM SUBMIT
    // ==================================================

    const statusForm =
        document.getElementById("orderStatusForm");

    const submitButton =
        statusForm?.querySelector(
            ".admin-order-modal-confirm"
        );


    statusForm?.addEventListener(
        "submit",
        function () {

            // Chống bấm liên tục
            if (submitButton) {

                submitButton.disabled = true;

                submitButton.innerHTML = `
                    <i class="fa-solid fa-spinner fa-spin"></i>
                    Đang cập nhật...
                `;

            }

        }
    );

});