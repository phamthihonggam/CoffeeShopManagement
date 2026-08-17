// ======================================================
// ROSALIE COFFEE
// ADMIN PRODUCT
// IMAGE PREVIEW + PROMOTION TOGGLE + TOAST
// ======================================================

document.addEventListener(
    "DOMContentLoaded",
    function () {

        // ==================================================
        // ELEMENTS
        // ==================================================

        const imageInput =
            document.getElementById("imageFile");

        const imagePreview =
            document.getElementById("imagePreview");

        const previewImage =
            document.getElementById("previewImage");

        const promotionToggle =
            document.getElementById("promotionToggle");

        const promotionFields =
            document.getElementById("promotionFields");

        const adminToast =
            document.getElementById("adminToast");

        const adminToastClose =
            document.getElementById("adminToastClose");


        // ==================================================
        // IMAGE PREVIEW
        // ==================================================

        function handleImagePreview() {

            if (
                !imageInput ||
                !imagePreview ||
                !previewImage
            ) {
                return;
            }


            const file =
                imageInput.files?.[0];


            // Không có file
            if (!file) {

                imagePreview.classList.remove(
                    "show"
                );

                previewImage.removeAttribute(
                    "src"
                );

                return;
            }


            // Chỉ nhận file ảnh
            if (!file.type.startsWith("image/")) {

                alert(
                    "Vui lòng chọn đúng file hình ảnh."
                );

                imageInput.value = "";

                imagePreview.classList.remove(
                    "show"
                );

                previewImage.removeAttribute(
                    "src"
                );

                return;
            }


            // ==================================================
            // TẠO PREVIEW
            // ==================================================

            const imageUrl =
                URL.createObjectURL(file);


            previewImage.src =
                imageUrl;


            imagePreview.classList.add(
                "show"
            );


            // Giải phóng URL sau khi load ảnh
            previewImage.onload =
                function () {

                    URL.revokeObjectURL(
                        imageUrl
                    );

                };

        }


        imageInput?.addEventListener(
            "change",
            handleImagePreview
        );


        // ==================================================
        // PROMOTION TOGGLE
        // ==================================================

        function updatePromotionFields() {

            if (
                !promotionToggle ||
                !promotionFields
            ) {
                return;
            }


            promotionFields.classList.toggle(
                "show",
                promotionToggle.checked
            );

        }


        promotionToggle?.addEventListener(
            "change",
            updatePromotionFields
        );


        // Chạy ngay khi mở Create/Edit
        updatePromotionFields();


        // ==================================================
        // ADMIN TOAST
        // ==================================================

        function hideAdminToast() {

            if (!adminToast) {
                return;
            }


            adminToast.classList.add(
                "hide"
            );


            setTimeout(
                function () {

                    adminToast.remove();

                },
                300
            );

        }


        // Nút X đóng toast
        adminToastClose?.addEventListener(
            "click",
            hideAdminToast
        );


        // Tự đóng sau 3 giây
        if (adminToast) {

            setTimeout(
                hideAdminToast,
                3000
            );

        }

    }
);