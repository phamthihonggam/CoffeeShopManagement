/* =====================================================
   ROSALIE COFFEE
   STAFF PRODUCT
===================================================== */

document.addEventListener(
    "DOMContentLoaded",
    function () {

        const deleteForms =
            document.querySelectorAll(
                ".staff-product-delete-form"
            );


        deleteForms.forEach(
            function (form) {

                form.addEventListener(
                    "submit",
                    function (event) {

                        event.preventDefault();


                        const productName =
                            form.dataset.productName
                            || "sản phẩm này";


                        Swal.fire({

                            title:
                                "Xóa sản phẩm?",

                            html:
                                `Bạn có chắc muốn xóa <strong>${escapeHtml(productName)}</strong> không?<br>
                                 <span style="font-size:13px;color:#8c7a72;">
                                     Hành động này không thể hoàn tác.
                                 </span>`,

                            icon:
                                "warning",

                            showCancelButton:
                                true,

                            confirmButtonText:
                                '<i class="fa-solid fa-trash"></i> Xóa sản phẩm',

                            cancelButtonText:
                                "Hủy",

                            reverseButtons:
                                true,

                            focusCancel:
                                true,

                            confirmButtonColor:
                                "#8b4438",

                            cancelButtonColor:
                                "#9a8b84",

                            background:
                                "#fffdfb",

                            color:
                                "#4f3028",

                            customClass: {

                                popup:
                                    "rosalie-confirm-popup",

                                title:
                                    "rosalie-confirm-title",

                                confirmButton:
                                    "rosalie-confirm-delete",

                                cancelButton:
                                    "rosalie-confirm-cancel"
                            }

                        }).then(
                            function (result) {

                                if (
                                    result.isConfirmed
                                ) {

                                    form.submit();

                                }

                            }
                        );

                    }
                );

            }
        );

    }
);


/* =====================================================
   ESCAPE HTML
===================================================== */

function escapeHtml(text) {

    const div =
        document.createElement(
            "div"
        );

    div.textContent =
        text;

    return div.innerHTML;
}