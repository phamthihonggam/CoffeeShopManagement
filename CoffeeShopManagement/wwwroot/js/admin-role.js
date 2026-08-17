document.addEventListener("DOMContentLoaded", function () {

    const toast = document.getElementById("roleSuccessToast");

    if (!toast) {
        return;
    }

    // Tự đóng sau 3 giây
    setTimeout(function () {
        closeRoleToast();
    }, 3000);
});


function closeRoleToast() {

    const toast = document.getElementById("roleSuccessToast");

    if (!toast) {
        return;
    }

    toast.classList.add("role-toast-hide");

    setTimeout(function () {
        toast.remove();
    }, 250);
}