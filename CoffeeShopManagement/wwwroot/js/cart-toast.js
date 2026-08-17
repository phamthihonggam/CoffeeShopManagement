// ==========================
// CART TOAST
// ==========================

const cartToast = document.getElementById("cartToast");

const toastImage = document.getElementById("toastImage");
const toastName = document.getElementById("toastName");
const toastQty = document.getElementById("toastQty");
const toastPrice = document.getElementById("toastPrice");

const toastClose = document.getElementById("toastClose");

let toastTimer = null;

// ==========================
// SHOW TOAST
// ==========================

function showCartToast(product) {

    if (!cartToast) return;

    toastImage.src =
        product.image || "/images/no-image.png";

    toastName.textContent =
        product.name || "";

    toastQty.textContent =
        product.quantity || 1;

    toastPrice.textContent =
        (product.total || 0)
            .toLocaleString("vi-VN") + "đ";

    cartToast.classList.add("show");

    clearTimeout(toastTimer);

    toastTimer = setTimeout(function () {

        cartToast.classList.remove("show");

    }, 3000);

}

// ==========================
// CLOSE BUTTON
// ==========================

if (toastClose) {

    toastClose.onclick = function () {

        cartToast.classList.remove("show");

    };

}

// ==========================
// CLICK OUTSIDE
// ==========================

if (cartToast) {

    cartToast.addEventListener("click", function (e) {

        if (e.target === cartToast) {

            cartToast.classList.remove("show");

        }

    });

}

// ==========================
// GLOBAL
// ==========================

window.showCartToast = showCartToast;

