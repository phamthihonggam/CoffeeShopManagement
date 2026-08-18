/* =========================================================
   ROSALIE COFFEE
   STAFF - PRODUCT DETAILS
   File: wwwroot/js/staff-product-details.js
========================================================= */

document.addEventListener("DOMContentLoaded", function () {

    /* =====================================================
       1. PRODUCT IMAGE FALLBACK
    ===================================================== */

    const productImage =
        document.querySelector(".js-product-detail-image");

    const imageFallback =
        document.querySelector(".js-product-image-fallback");


    if (productImage && imageFallback) {

        /* Nếu ảnh tải lỗi */
        productImage.addEventListener("error", function () {

            productImage.style.display = "none";

            imageFallback.classList.add("visible");

        });


        /* Nếu ảnh tải thành công */
        productImage.addEventListener("load", function () {

            productImage.style.display = "block";

            imageFallback.classList.remove("visible");

        });


        /*
           Trường hợp trình duyệt đã cố load ảnh
           trước khi JS chạy và ảnh bị lỗi.
        */
        if (
            productImage.complete &&
            productImage.naturalWidth === 0
        ) {

            productImage.style.display = "none";

            imageFallback.classList.add("visible");

        }

    }



    /* =====================================================
       2. DELETE PRODUCT CONFIRMATION
    ===================================================== */

    const deleteForm =
        document.querySelector(".staff-product-delete-form");


    if (deleteForm) {

        deleteForm.addEventListener("submit", function (event) {

            /*
               Ngăn form submit ngay lập tức
            */
            event.preventDefault();


            const productName =
                deleteForm.dataset.productName
                || "sản phẩm này";


            /* =================================================
               Nếu SweetAlert2 tồn tại
            ================================================= */

            if (typeof Swal !== "undefined") {

                Swal.fire({

                    title: "Xóa sản phẩm?",

                    html: `
                        Bạn có chắc muốn xóa
                        <strong>${escapeHtml(productName)}</strong>
                        khỏi hệ thống không?
                        <br><br>
                        <span style="
                            color:#8a756d;
                            font-size:14px;
                        ">
                            Hành động này không thể hoàn tác.
                        </span>
                    `,

                    icon: "warning",

                    showCancelButton: true,

                    confirmButtonText:
                        '<i class="fa-solid fa-trash"></i> Xóa sản phẩm',

                    cancelButtonText:
                        '<i class="fa-solid fa-xmark"></i> Hủy',

                    reverseButtons: true,

                    focusCancel: true,

                    allowOutsideClick: false,

                    allowEscapeKey: true,

                    customClass: {

                        popup:
                            "rosalie-swal-popup",

                        title:
                            "rosalie-swal-title",

                        htmlContainer:
                            "rosalie-swal-content",

                        confirmButton:
                            "rosalie-swal-confirm",

                        cancelButton:
                            "rosalie-swal-cancel"

                    },

                    buttonsStyling: false

                }).then(function (result) {

                    if (result.isConfirmed) {

                        /*
                           Submit form thật sự.
                           Dùng native submit() để không kích hoạt
                           event submit lần thứ hai.
                        */
                        deleteForm.submit();

                    }

                });

            }

            /* =================================================
               Fallback nếu SweetAlert không load được
            ================================================= */

            else {

                const confirmed = window.confirm(
                    `Bạn có chắc muốn xóa "${productName}" không?`
                );

                if (confirmed) {

                    deleteForm.submit();

                }

            }

        });

    }



    /* =====================================================
       3. PREVENT DOUBLE CLICK
       Các link Edit / Back không cần xử lý gì đặc biệt.
       Riêng nút xóa sau khi xác nhận sẽ khóa.
    ===================================================== */

    const deleteButton =
        document.querySelector(".staff-product-delete-btn");


    if (deleteButton && deleteForm) {

        deleteForm.addEventListener("formdata", function () {

            deleteButton.disabled = true;

            deleteButton.innerHTML = `
                <i class="fa-solid fa-spinner fa-spin"></i>
                Đang xóa...
            `;

        });

    }

});


/* =========================================================
   ESCAPE HTML
   Tránh tên sản phẩm chứa ký tự HTML làm hỏng SweetAlert
========================================================= */

function escapeHtml(value) {

    if (!value) {
        return "";
    }


    return String(value)

        .replaceAll("&", "&amp;")

        .replaceAll("<", "&lt;")

        .replaceAll(">", "&gt;")

        .replaceAll('"', "&quot;")

        .replaceAll("'", "&#039;");

}