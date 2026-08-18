/* =====================================================
   ROSALIE COFFEE
   STAFF PRODUCT INDEX
===================================================== */

document.addEventListener(
    "DOMContentLoaded",
    function () {

        setupProductImages();

        setupDeleteProducts();

        setupProductToasts();

        setupPageSize();

    }
);


/* =====================================================
   PRODUCT IMAGE FALLBACK
===================================================== */

function setupProductImages() {

    const images =
        document.querySelectorAll(
            ".js-product-image"
        );


    images.forEach(
        function (image) {

            const container =
                image.closest(
                    ".staff-product-image"
                );


            if (!container) {
                return;
            }


            const fallback =
                container.querySelector(
                    ".staff-product-image-fallback"
                );


            if (!fallback) {
                return;
            }


            function showFallback() {

                image.style.display =
                    "none";


                fallback.classList.add(
                    "visible"
                );

            }


            image.addEventListener(
                "error",
                showFallback
            );


            if (
                image.complete
                &&
                image.naturalWidth === 0
            ) {

                showFallback();

            }

        }
    );

}


/* =====================================================
   DELETE PRODUCT
===================================================== */

function setupDeleteProducts() {

    const forms =
        document.querySelectorAll(
            ".staff-product-delete-form"
        );


    forms.forEach(
        function (form) {

            form.addEventListener(
                "submit",
                function (event) {

                    event.preventDefault();


                    const productName =
                        form.dataset.productName
                        ||
                        "sản phẩm này";


                    showDeleteProductPopup(
                        form,
                        productName
                    );

                }
            );

        }
    );

}


/* =====================================================
   DELETE POPUP
===================================================== */

function showDeleteProductPopup(
    form,
    productName
) {

    if (
        typeof Swal ===
        "undefined"
    ) {

        return;

    }


    Swal.fire({

        icon:
            "warning",

        title:
            "Xóa sản phẩm?",

        html:
            `
                <div>
                    Bạn có chắc muốn xóa
                    <strong class="js-delete-product-name"></strong>
                    không?
                </div>

                <div style="
                    margin-top:5px;
                    font-size:13px;
                    color:#958178;
                ">
                    Hành động này không thể hoàn tác.
                </div>
            `,

        showCancelButton:
            true,

        reverseButtons:
            true,

        focusCancel:
            true,

        buttonsStyling:
            false,

        confirmButtonText:
            '<i class="fa-solid fa-trash"></i> Xóa sản phẩm',

        cancelButtonText:
            '<i class="fa-solid fa-xmark"></i> Hủy',

        customClass: {

            popup:
                "rosalie-delete-popup",

            title:
                "rosalie-delete-title",

            htmlContainer:
                "rosalie-delete-html",

            confirmButton:
                "rosalie-delete-confirm",

            cancelButton:
                "rosalie-delete-cancel"

        },

        didOpen:
            function (popup) {

                const nameElement =
                    popup.querySelector(
                        ".js-delete-product-name"
                    );


                if (nameElement) {

                    nameElement.textContent =
                        productName;

                }

            }

    }).then(
        function (result) {

            if (!result.isConfirmed) {
                return;
            }


            /*
                Native submit:
                không chạy event confirm lần 2.
            */

            form.submit();

        }
    );

}


/* =====================================================
   TOAST
===================================================== */

function setupProductToasts() {

    const toasts =
        document.querySelectorAll(
            ".js-product-toast"
        );


    toasts.forEach(
        function (toast) {

            const closeButton =
                toast.querySelector(
                    ".js-toast-close"
                );


            if (closeButton) {

                closeButton.addEventListener(
                    "click",
                    function () {

                        hideProductToast(
                            toast
                        );

                    }
                );

            }


            window.setTimeout(
                function () {

                    hideProductToast(
                        toast
                    );

                },
                4000
            );

        }
    );

}


/* =====================================================
   HIDE TOAST
===================================================== */

function hideProductToast(
    toast
) {

    if (!toast) {
        return;
    }


    toast.classList.add(
        "hide"
    );


    window.setTimeout(
        function () {

            if (toast.parentNode) {

                toast.parentNode.removeChild(
                    toast
                );

            }

        },
        300
    );

}


/* =====================================================
   PAGE SIZE
===================================================== */

function setupPageSize() {

    const pageSizeSelect =
        document.querySelector(
            ".js-page-size"
        );


    if (!pageSizeSelect) {
        return;
    }


    pageSizeSelect.addEventListener(
        "change",
        function () {

            const form =
                pageSizeSelect.closest(
                    "form"
                );


            if (form) {

                form.submit();

            }

        }
    );

}