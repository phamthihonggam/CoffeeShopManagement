// ======================================================
// ROSALIE COFFEE
// ADMIN CATEGORY
// TOAST + DELETE CONFIRM MODAL
// ======================================================

document.addEventListener("DOMContentLoaded", function () {

    // ==================================================
    // TOAST
    // ==================================================

    const toast =
        document.getElementById("adminToast");

    const closeButton =
        document.getElementById("adminToastClose");


    if (toast) {

        // Hiện toast
        requestAnimationFrame(function () {

            toast.classList.add("show");

        });


        function hideToast() {

            toast.classList.remove("show");

            setTimeout(function () {

                if (toast) {
                    toast.remove();
                }

            }, 300);

        }


        // Nút đóng
        closeButton?.addEventListener(
            "click",
            hideToast
        );


        // Tự đóng sau 3 giây
        setTimeout(
            hideToast,
            3000
        );

    }


    // ==================================================
    // DELETE CATEGORY MODAL
    // ==================================================

    const deleteModal =
        document.getElementById("categoryDeleteModal");

    const deleteBackdrop =
        document.getElementById("categoryDeleteBackdrop");

    const deleteCancel =
        document.getElementById("categoryDeleteCancel");

    const deleteConfirm =
        document.getElementById("categoryDeleteConfirm");

    const deleteName =
        document.getElementById("categoryDeleteName");

    const deleteButtons =
        document.querySelectorAll(".js-delete-category");


    // Form đang chờ xóa
    let currentDeleteForm = null;


    // ==================================================
    // OPEN MODAL
    // ==================================================

    function openDeleteModal(button) {

        if (!deleteModal) {
            return;
        }


        const formId =
            button.dataset.formId;

        const categoryName =
            button.dataset.categoryName;


        currentDeleteForm =
            document.getElementById(formId);


        // Hiện tên danh mục
        if (deleteName) {

            deleteName.textContent =
                `"${categoryName}"`;

        }


        // Hiện modal
        deleteModal.classList.add("show");


        // Khóa scroll
        document.body.classList.add(
            "category-modal-open"
        );

    }


    // ==================================================
    // CLOSE MODAL
    // ==================================================

    function closeDeleteModal() {

        if (!deleteModal) {
            return;
        }


        deleteModal.classList.remove("show");


        document.body.classList.remove(
            "category-modal-open"
        );


        currentDeleteForm = null;

    }


    // ==================================================
    // CLICK DELETE BUTTON
    // ==================================================

    deleteButtons.forEach(function (button) {

        button.addEventListener(
            "click",
            function () {

                openDeleteModal(button);

            }
        );

    });


    // ==================================================
    // CANCEL
    // ==================================================

    deleteCancel?.addEventListener(
        "click",
        closeDeleteModal
    );


    // ==================================================
    // CLICK BACKDROP
    // ==================================================

    deleteBackdrop?.addEventListener(
        "click",
        closeDeleteModal
    );


    // ==================================================
    // ESC TO CLOSE
    // ==================================================

    document.addEventListener(
        "keydown",
        function (event) {

            if (event.key === "Escape" &&
                deleteModal?.classList.contains("show")) {

                closeDeleteModal();

            }

        }
    );


    // ==================================================
    // CONFIRM DELETE
    // ==================================================

    deleteConfirm?.addEventListener(
        "click",
        function () {

            if (!currentDeleteForm) {
                return;
            }


            // Chống bấm xóa nhiều lần
            deleteConfirm.disabled = true;

            deleteConfirm.innerHTML = `
                <i class="fa-solid fa-spinner fa-spin"></i>
                Đang xóa...
            `;


            // Submit form xóa
            currentDeleteForm.submit();

        }
    );

});