/* =========================================================
   ROSALIE COFFEE
   STAFF - PRODUCT EDIT
   File: wwwroot/js/staff-product-edit.js
========================================================= */

document.addEventListener("DOMContentLoaded", function () {

    const imageInput =
        document.getElementById("imageFile");

    const uploadBox =
        document.getElementById("imageUploadBox");

    const uploadEmpty =
        document.getElementById("uploadEmpty");

    const preview =
        document.getElementById("imagePreview");

    const previewImage =
        document.getElementById("previewImage");

    const selectedFileName =
        document.getElementById("selectedFileName");

    const currentImage =
        document.querySelector(".js-current-product-image");

    const currentFallback =
        document.querySelector(".js-current-image-fallback");

    const promotionToggle =
        document.getElementById("promotionToggle");

    const promotionFields =
        document.getElementById("promotionFields");

    const editForm =
        document.getElementById("staffProductEditForm");

    const saveButton =
        document.getElementById("saveProductButton");


    /* =====================================================
       CURRENT IMAGE FALLBACK
    ===================================================== */

    if (currentImage && currentFallback) {

        const showFallback = function () {

            currentImage.style.display = "none";

            currentFallback.classList.add("visible");

        };


        currentImage.addEventListener(
            "error",
            showFallback
        );


        if (
            currentImage.complete &&
            currentImage.naturalWidth === 0
        ) {

            showFallback();

        }

    }


    /* =====================================================
       IMAGE PREVIEW
    ===================================================== */

    function resetPreview() {

        if (preview) {
            preview.classList.remove("show");
        }

        if (uploadEmpty) {
            uploadEmpty.style.display = "flex";
        }

        if (previewImage) {
            previewImage.src = "";
        }

        if (selectedFileName) {
            selectedFileName.textContent =
                "JPG, JPEG, PNG hoặc WEBP · tối đa 5MB";
        }

    }


    function showImageError(message) {

        window.alert(message);

        if (imageInput) {
            imageInput.value = "";
        }

        resetPreview();

    }


    function handleImageFile(file) {

        if (!file) {

            resetPreview();

            return;

        }


        const allowedTypes = [
            "image/jpeg",
            "image/png",
            "image/webp"
        ];


        if (!allowedTypes.includes(file.type)) {

            showImageError(
                "Chỉ chấp nhận ảnh JPG, JPEG, PNG hoặc WEBP."
            );

            return;

        }


        const maxFileSize =
            5 * 1024 * 1024;


        if (file.size > maxFileSize) {

            showImageError(
                "Ảnh sản phẩm không được vượt quá 5MB."
            );

            return;

        }


        if (previewImage) {

            previewImage.src =
                URL.createObjectURL(file);

        }


        if (uploadEmpty) {
            uploadEmpty.style.display = "none";
        }


        if (preview) {
            preview.classList.add("show");
        }


        if (selectedFileName) {

            selectedFileName.textContent =
                file.name;

        }

    }


    imageInput?.addEventListener(
        "change",
        function () {

            handleImageFile(
                this.files?.[0]
            );

        }
    );


    /* =====================================================
       DRAG & DROP IMAGE
    ===================================================== */

    if (uploadBox && imageInput) {

        [
            "dragenter",
            "dragover"
        ].forEach(eventName => {

            uploadBox.addEventListener(
                eventName,
                function (event) {

                    event.preventDefault();

                    uploadBox.classList.add(
                        "dragging"
                    );

                }
            );

        });


        [
            "dragleave",
            "drop"
        ].forEach(eventName => {

            uploadBox.addEventListener(
                eventName,
                function (event) {

                    event.preventDefault();

                    uploadBox.classList.remove(
                        "dragging"
                    );

                }
            );

        });


        uploadBox.addEventListener(
            "drop",
            function (event) {

                const file =
                    event.dataTransfer?.files?.[0];

                if (!file) {
                    return;
                }


                const transfer =
                    new DataTransfer();

                transfer.items.add(file);

                imageInput.files =
                    transfer.files;


                handleImageFile(file);

            }
        );

    }


    /* =====================================================
       PROMOTION TOGGLE
    ===================================================== */

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


    updatePromotionFields();


    /* =====================================================
       SUBMIT LOADING
    ===================================================== */

    editForm?.addEventListener(
        "submit",
        function () {

            if (!saveButton) {
                return;
            }


            window.setTimeout(
                function () {

                    if (
                        typeof editForm.checkValidity === "function" &&
                        !editForm.checkValidity()
                    ) {

                        return;

                    }


                    saveButton.disabled =
                        true;

                    saveButton.innerHTML = `
                        <i class="fa-solid fa-spinner fa-spin"></i>
                        <span>Đang lưu...</span>
                    `;

                },
                0
            );

        }
    );

});