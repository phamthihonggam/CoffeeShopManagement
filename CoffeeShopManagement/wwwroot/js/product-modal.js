// ================================
// PRODUCT MODAL
// ================================

let basePrice = 0;
let currentPrice = 0;
let quantity = 1;

document.addEventListener("DOMContentLoaded", function () {

    const priceElement = document.querySelector(".product-price");

    if (priceElement) {

        basePrice = parsePrice(priceElement.innerText);
        currentPrice = basePrice;

    }

    initializeEvents();

    updateTotalPrice();

});


// ================================
// EVENT
// ================================

function initializeEvents() {

    document.querySelectorAll(".size-option")
        .forEach(radio => {

            radio.addEventListener("change", updateTotalPrice);

        });

    document.querySelectorAll(".topping-option")
        .forEach(item => {

            item.addEventListener("change", updateTotalPrice);

        });

    const plus = document.getElementById("btnPlus");

    if (plus) {

        plus.addEventListener("click", increaseQuantity);

    }

    const minus = document.getElementById("btnMinus");

    if (minus) {

        minus.addEventListener("click", decreaseQuantity);

    }

    const addCart = document.getElementById("btnAddCart");

    if (addCart) {

        addCart.addEventListener("click", addToCart);

    }

}