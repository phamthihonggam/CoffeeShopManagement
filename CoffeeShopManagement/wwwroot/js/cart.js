// =========================================================
// ROSALIE COFFEE
// CART
// =========================================================
//
// Cart chỉ xử lý:
// - Toast thông báo
// - Popup xác nhận xóa sản phẩm
//
// Voucher + phí giao hàng được xử lý tại Checkout.
//
// =========================================================


// =========================================================
// CART TOAST
// =========================================================

function showCartToast(
    title,
    message,
    type = "warning"
) {

    const toast =
        document.getElementById(
            "cartToast"
        );


    if (!toast) {
        return;
    }


    // =====================================================
    // TOAST TYPE
    // =====================================================

    toast.className =
        "cart-toast show " + type;


    // =====================================================
    // TITLE
    // =====================================================

    const titleElement =
        document.getElementById(
            "cartToastTitle"
        );


    if (titleElement) {

        titleElement.innerText =
            title;
    }


    // =====================================================
    // MESSAGE
    // =====================================================

    const messageElement =
        document.getElementById(
            "cartToastMessage"
        );


    if (messageElement) {

        messageElement.innerText =
            message;
    }


    // =====================================================
    // ICON
    // =====================================================

    const icon =
        document.getElementById(
            "cartToastIcon"
        );


    if (icon) {

        switch (type) {

            case "success":

                icon.className =
                    "bi bi-check-circle-fill";

                break;


            case "error":

                icon.className =
                    "bi bi-x-circle-fill";

                break;


            default:

                icon.className =
                    "bi bi-exclamation-circle-fill";

                break;
        }
    }


    // =====================================================
    // AUTO CLOSE
    // =====================================================

    clearTimeout(
        window.cartToastTimer
    );


    window.cartToastTimer =
        setTimeout(
            () => {

                toast
                    .classList
                    .remove("show");

            },
            3000
        );
}


// =========================================================
// CLOSE CART TOAST
// =========================================================

function closeCartToast() {

    const toast =
        document.getElementById(
            "cartToast"
        );


    if (!toast) {
        return;
    }


    toast
        .classList
        .remove("show");
}


// =========================================================
// REMOVE PRODUCT CONFIRM
// =========================================================

function initRemoveConfirm() {

    const overlay =
        document.getElementById(
            "removeConfirmOverlay"
        );


    const message =
        document.getElementById(
            "removeConfirmMessage"
        );


    const cancelButton =
        document.getElementById(
            "cancelRemoveBtn"
        );


    const confirmButton =
        document.getElementById(
            "confirmRemoveBtn"
        );


    // Không có popup thì bỏ qua
    if (
        !overlay ||
        !cancelButton ||
        !confirmButton
    ) {
        return;
    }


    let removeUrl =
        null;


    // =====================================================
    // OPEN MODAL
    // =====================================================

    document
        .querySelectorAll(
            ".remove-btn"
        )
        .forEach(
            button => {

                button.addEventListener(
                    "click",
                    function () {

                        // URL thật để xóa
                        removeUrl =
                            this.dataset.removeUrl;


                        // Tên sản phẩm
                        const productName =
                            this.dataset.productName;


                        // Nội dung thông báo
                        if (message) {

                            if (productName) {

                                message.textContent =
                                    `Bạn có chắc muốn xóa "${productName}" khỏi giỏ hàng?`;

                            }
                            else {

                                message.textContent =
                                    "Bạn có chắc muốn xóa sản phẩm này khỏi giỏ hàng?";

                            }
                        }


                        // Hiện popup
                        overlay
                            .classList
                            .add("show");


                        // Không cho body cuộn phía sau popup
                        document
                            .body
                            .classList
                            .add("remove-modal-open");

                    }
                );

            }
        );


    // =====================================================
    // CLOSE MODAL
    // =====================================================

    function closeRemoveConfirm() {

        overlay
            .classList
            .remove("show");


        document
            .body
            .classList
            .remove("remove-modal-open");


        removeUrl =
            null;
    }


    // =====================================================
    // KHÔNG
    // =====================================================

    cancelButton.addEventListener(
        "click",
        () => {

            closeRemoveConfirm();

        }
    );


    // =====================================================
    // CÓ, XÓA
    // =====================================================

    confirmButton.addEventListener(
        "click",
        () => {

            if (!removeUrl) {
                return;
            }


            // Khóa nút tránh click 2 lần
            confirmButton.disabled =
                true;


            confirmButton.innerHTML = `
                <i class="fas fa-spinner fa-spin"></i>
                Đang xóa...
            `;


            // Chuyển tới action Remove của CartController
            window.location.href =
                removeUrl;

        }
    );


    // =====================================================
    // CLICK NGOÀI MODAL => ĐÓNG
    // =====================================================

    overlay.addEventListener(
        "click",
        event => {

            if (
                event.target === overlay
            ) {

                closeRemoveConfirm();

            }
        }
    );


    // =====================================================
    // ESC => ĐÓNG
    // =====================================================

    document.addEventListener(
        "keydown",
        event => {

            if (
                event.key === "Escape" &&
                overlay
                    .classList
                    .contains("show")
            ) {

                closeRemoveConfirm();

            }
        }
    );
}


// =========================================================
// DOM CONTENT LOADED
// =========================================================

document.addEventListener(
    "DOMContentLoaded",
    () => {

        // =================================================
        // CART TOAST CLOSE BUTTON
        // =================================================

        const closeButton =
            document.getElementById(
                "closeCartToast"
            );


        if (closeButton) {

            closeButton.addEventListener(
                "click",
                () => {

                    closeCartToast();

                }
            );
        }


        // =================================================
        // ESC TO CLOSE CART TOAST
        // =================================================

        document.addEventListener(
            "keydown",
            event => {

                if (event.key === "Escape") {

                    closeCartToast();

                }
            }
        );


        // =================================================
        // INIT REMOVE CONFIRM
        // =================================================

        initRemoveConfirm();

    }
);