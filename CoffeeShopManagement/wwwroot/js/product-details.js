/* =====================================================
   ROSALIE COFFEE
   STAFF PRODUCT DETAILS
===================================================== */

document.addEventListener(
    "DOMContentLoaded",
    function () {

        setupProductImage();

        setupDeleteProduct();

    }
);


/* =====================================================
   PRODUCT IMAGE FALLBACK
===================================================== */

function setupProductImage() {

    const image =
        document.querySelector(
            ".js-product-detail-image"
        );

    const fallback =
        document.querySelector(
            ".js-product-image-fallback"
        );


    if (
        !image
        ||
        !fallback
    ) {

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


    /*
       Trường hợp ảnh đã error trước khi
       event listener được gắn.
    */

    if (
        image.complete
        &&
        image.naturalWidth === 0
    ) {

        showFallback();

    }

}


/* =====================================================
   DELETE PRODUCT
===================================================== */

function setupDeleteProduct() {

    const deleteForm =
        document.querySelector(
            ".staff-product-delete-form"
        );


    if (!deleteForm) {

        return;

    }


    deleteForm.addEventListener(
        "submit",
        function (event) {

            event.preventDefault();


            const productName =
                deleteForm.dataset.productName
                ||
                "sản phẩm này";


            showDeleteConfirmation(
                deleteForm,
                productName
            );

        }
    );

}


/* =====================================================
   SWEETALERT DELETE
===================================================== */

function showDeleteConfirmation(
    deleteForm,
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

                const productNameElement =
                    popup.querySelector(
                        ".js-delete-product-name"
                    );


                if (productNameElement) {

                    productNameElement.textContent =
                        productName;

                }

            }

    }).then(
        function (result) {

            if (!result.isConfirmed) {

                return;

            }


            /*
               submit() native để không kích hoạt
               event confirmation lần thứ 2.
            */

            deleteForm.submit();

        }
    );

}