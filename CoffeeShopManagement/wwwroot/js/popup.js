// ======================================================
// ROSALIE COFFEE
// POPUP.JS
// DATABASE CUSTOMIZE VERSION
// ======================================================


// ======================================================
// POPUP STATE
// ======================================================

let popupQuantity = 1;

let popupBasePrice = 0;

let popupType = 0;

let popupProductId = 0;


// ======================================================
// GET ELEMENT
// Không khai báo cứng NodeList vì option được render động
// ======================================================

function getModal() {
    return document.getElementById("productModal");
}

function getModalImg() {
    return document.getElementById("modalImg");
}

function getModalName() {
    return document.getElementById("modalName");
}

function getModalDesc() {
    return document.getElementById("modalDesc");
}

function getModalPrice() {
    return document.getElementById("modalPrice");
}


// ======================================================
// MENU CARD QUANTITY
// ======================================================

function initQuantity() {

    document
        .querySelectorAll(".quantity-box")
        .forEach(box => {

            const buttons =
                box.querySelectorAll(".qty-btn");

            if (buttons.length < 2)
                return;

            const minus = buttons[0];

            const plus = buttons[1];

            const number =
                box.querySelector("span");

            if (!number)
                return;


            // Tránh add event nhiều lần
            if (box.dataset.quantityInit === "1")
                return;

            box.dataset.quantityInit = "1";


            let qty =
                Number(number.textContent) || 1;


            plus.onclick = function (e) {

                e.preventDefault();

                e.stopPropagation();

                qty++;

                number.textContent =
                    qty;

            };


            minus.onclick = function (e) {

                e.preventDefault();

                e.stopPropagation();

                if (qty > 1) {

                    qty--;

                    number.textContent =
                        qty;

                }

            };

        });

}


// ======================================================
// FAVORITE
// ======================================================

function initFavorite() {

    document
        .querySelectorAll(".favorite-btn")
        .forEach(btn => {

            btn.onclick = function (e) {

                e.preventDefault();

                e.stopPropagation();


                this.classList.toggle(
                    "active"
                );


                const icon =
                    this.querySelector("i");


                if (!icon)
                    return;


                if (
                    this.classList.contains(
                        "active"
                    )
                ) {

                    icon.classList.remove(
                        "fa-regular"
                    );

                    icon.classList.add(
                        "fa-solid"
                    );

                }
                else {

                    icon.classList.remove(
                        "fa-solid"
                    );

                    icon.classList.add(
                        "fa-regular"
                    );

                }

            };

        });

}


// ======================================================
// RESET MODAL
// ======================================================

function resetPopup() {

    popupQuantity = 1;


    const qtyValue =
        document.getElementById(
            "qtyValue"
        );

    if (qtyValue)
        qtyValue.textContent = "1";


    const note =
        document.getElementById(
            "txtNote"
        );

    if (note)
        note.value = "";


    // Clear old option HTML

    const sizeGroup =
        document.getElementById(
            "sizeGroup"
        );

    const sugarGroup =
        document.getElementById(
            "sugarGroup"
        );

    const iceGroup =
        document.getElementById(
            "iceGroup"
        );

    const toppingList =
        document.getElementById(
            "toppingList"
        );


    if (sizeGroup)
        sizeGroup.innerHTML = "";

    if (sugarGroup)
        sugarGroup.innerHTML = "";

    if (iceGroup)
        iceGroup.innerHTML = "";

    if (toppingList)
        toppingList.innerHTML = "";


    hideCustomizeSections();

}


// ======================================================
// HIDE OPTIONS
// ======================================================

function hideCustomizeSections() {

    const sections = [

        document.getElementById(
            "sizeSection"
        ),

        document.getElementById(
            "sugarSection"
        ),

        document.getElementById(
            "iceSection"
        ),

        document.getElementById(
            "toppingSection"
        )

    ];


    sections.forEach(section => {

        if (section)
            section.style.display =
                "none";

    });

}


// ======================================================
// LOADING
// ======================================================

function showCustomizeLoading(show) {

    const loading =
        document.getElementById(
            "customizeLoading"
        );


    if (!loading)
        return;


    loading.style.display =
        show ? "flex" : "none";

}


// ======================================================
// OPEN POPUP
// ======================================================

async function openProductPopup(card) {

    const modal =
        getModal();


    if (!modal)
        return;


    popupProductId =
        Number(card.dataset.id || 0);


    popupType =
        Number(card.dataset.type || 0);


    popupBasePrice =
        Number(card.dataset.price || 0);


    resetPopup();


    modal.dataset.id =
        popupProductId;


    // ==========================================
    // BASIC PRODUCT INFO
    // ==========================================

    const modalImg =
        getModalImg();


    const modalName =
        getModalName();


    const modalDesc =
        getModalDesc();


    if (modalImg) {

        modalImg.src =
            card.dataset.image || "";

        modalImg.alt =
            card.dataset.name || "";

    }


    if (modalName) {

        modalName.textContent =
            card.dataset.name || "";

    }


    if (modalDesc) {

        modalDesc.textContent =
            card.dataset.desc || "";

    }


    updatePopupPrice();


    modal.style.display =
        "flex";


    // ==========================================
    // BÁNH
    // Không cần customize
    // ==========================================

    if (popupType === 5) {

        hideCustomizeSections();

        return;

    }


    // ==========================================
    // LOAD DATABASE OPTIONS
    // ==========================================

    showCustomizeLoading(true);


    try {

        const response =
            await fetch(
                `/Menu/GetCustomizeOptions?id=${popupProductId}`
            );


        if (!response.ok) {

            throw new Error(
                "Không thể tải tùy chọn sản phẩm."
            );

        }


        const data =
            await response.json();


        if (!data.success) {

            throw new Error(
                "Không tìm thấy sản phẩm."
            );

        }


        popupType =
            Number(
                data.productType ||
                popupType
            );


        renderCustomizeOptions(
            data
        );


        updatePopupPrice();

    }
    catch (error) {

        console.error(
            "Customize error:",
            error
        );


        hideCustomizeSections();

    }
    finally {

        showCustomizeLoading(false);

    }

}


// ======================================================
// INIT POPUP CARD
// ======================================================

function initPopup() {

    document
        .querySelectorAll(
            ".menu-card, .product-card"
        )
        .forEach(card => {


            // Chỉ nút Add Cart mở popup
            const buyButton =
                card.querySelector(
                    ".btn-buy"
                );


            if (buyButton) {

                buyButton.onclick =
                    function (e) {

                        e.preventDefault();

                        e.stopPropagation();

                        openProductPopup(
                            card
                        );

                    };

            }


            // Không cho click card mở popup
            // Ảnh / tên vẫn đi Details bình thường

            card.onclick =
                function (e) {

                    if (
                        e.target.closest(
                            ".favorite-btn"
                        )
                    ) {
                        return;
                    }


                    if (
                        e.target.closest(
                            ".btn-buy"
                        )
                    ) {
                        return;
                    }


                    if (
                        e.target.closest(
                            ".menu-product-image-link"
                        )
                    ) {
                        return;
                    }


                    if (
                        e.target.closest(
                            ".menu-product-name-link"
                        )
                    ) {
                        return;
                    }

                };

        });

}


// ======================================================
// RENDER CUSTOMIZE
// ======================================================

function renderCustomizeOptions(data) {

    renderSizes(
        data.sizes || []
    );


    renderSugar(
        data.sugarLevels || []
    );


    renderIce(
        data.iceLevels || []
    );


    renderToppings(
        data.toppings || []
    );


    applyCustomizeRules(
        Number(
            data.productType || 0
        )
    );

}


// ======================================================
// RENDER SIZE
// ======================================================

function renderSizes(sizes) {

    const section =
        document.getElementById(
            "sizeSection"
        );


    const group =
        document.getElementById(
            "sizeGroup"
        );


    if (!section || !group)
        return;


    group.innerHTML = "";


    if (!sizes.length) {

        section.style.display =
            "none";

        return;

    }


    sizes.forEach(
        (size, index) => {

            const button =
                document.createElement(
                    "button"
                );


            button.type =
                "button";


            button.className =
                "size-btn" +
                (
                    index === 0
                        ? " active"
                        : ""
                );


            button.dataset.size =
                size.name;


            button.dataset.plus =
                Number(
                    size.price || 0
                );


            const price =
                Number(
                    size.price || 0
                );


            button.innerHTML =
                `
                    <strong>
                        ${escapeHtml(size.name)}
                    </strong>

                    ${price > 0
                    ?
                    `<small>+${formatMoney(price)}</small>`
                    :
                    ""
                }
                `;


            button.onclick =
                function () {

                    document
                        .querySelectorAll(
                            "#sizeGroup .size-btn"
                        )
                        .forEach(x => {

                            x.classList.remove(
                                "active"
                            );

                        });


                    this.classList.add(
                        "active"
                    );


                    updatePopupPrice();

                };


            group.appendChild(
                button
            );

        }
    );


    section.style.display =
        "block";

}


// ======================================================
// RENDER SUGAR
// ======================================================

function renderSugar(items) {

    const section =
        document.getElementById(
            "sugarSection"
        );


    const group =
        document.getElementById(
            "sugarGroup"
        );


    if (!section || !group)
        return;


    group.innerHTML = "";


    if (!items.length) {

        section.style.display =
            "none";

        return;

    }


    items.forEach(
        (item, index) => {

            const label =
                document.createElement(
                    "label"
                );


            const input =
                document.createElement(
                    "input"
                );


            input.type =
                "radio";


            input.name =
                "sugar";


            input.value =
                item.name;


            input.checked =
                index === 0;


            const span =
                document.createElement(
                    "span"
                );


            span.textContent =
                item.name;


            label.appendChild(
                input
            );


            label.appendChild(
                span
            );


            group.appendChild(
                label
            );

        }
    );


    section.style.display =
        "block";

}


// ======================================================
// RENDER ICE
// ======================================================

function renderIce(items) {

    const section =
        document.getElementById(
            "iceSection"
        );


    const group =
        document.getElementById(
            "iceGroup"
        );


    if (!section || !group)
        return;


    group.innerHTML = "";


    if (!items.length) {

        section.style.display =
            "none";

        return;

    }


    items.forEach(
        (item, index) => {

            const label =
                document.createElement(
                    "label"
                );


            const input =
                document.createElement(
                    "input"
                );


            input.type =
                "radio";


            input.name =
                "ice";


            input.value =
                item.name;


            input.checked =
                index === 0;


            const span =
                document.createElement(
                    "span"
                );


            span.textContent =
                item.name;


            label.appendChild(
                input
            );


            label.appendChild(
                span
            );


            group.appendChild(
                label
            );

        }
    );


    section.style.display =
        "block";

}


// ======================================================
// RENDER TOPPING
// ======================================================

function renderToppings(items) {

    const section =
        document.getElementById(
            "toppingSection"
        );


    const list =
        document.getElementById(
            "toppingList"
        );


    if (!section || !list)
        return;


    list.innerHTML = "";


    if (!items.length) {

        section.style.display =
            "none";

        return;

    }


    items.forEach(item => {

        const label =
            document.createElement(
                "label"
            );


        label.className =
            "topping-item";


        const left =
            document.createElement(
                "span"
            );


        const input =
            document.createElement(
                "input"
            );


        input.className =
            "topping-option";


        input.type =
            "checkbox";


        input.value =
            item.name;


        input.dataset.price =
            Number(
                item.price || 0
            );


        input.addEventListener(
            "change",
            updatePopupPrice
        );


        const name =
            document.createElement(
                "span"
            );


        name.textContent =
            item.name;


        left.appendChild(
            input
        );


        left.appendChild(
            name
        );


        const price =
            document.createElement(
                "strong"
            );


        price.textContent =
            "+" +
            formatMoney(
                Number(
                    item.price || 0
                )
            );


        label.appendChild(
            left
        );


        label.appendChild(
            price
        );


        list.appendChild(
            label
        );

    });


    section.style.display =
        "block";

}


// ======================================================
// RULE THEO LOẠI SẢN PHẨM
//
// 1 = Coffee
// 2 = Tea
// 3 = Matcha
// 4 = Soda
// 5 = Cake
// 6 = Juice
// 7 = Yogurt
// 8 = Blended
// ======================================================

function applyCustomizeRules(type) {

    const sizeSection =
        document.getElementById(
            "sizeSection"
        );


    const sugarSection =
        document.getElementById(
            "sugarSection"
        );


    const iceSection =
        document.getElementById(
            "iceSection"
        );


    const toppingSection =
        document.getElementById(
            "toppingSection"
        );


    // ==========================================
    // BÁNH
    // ==========================================

    if (type === 5) {

        hideCustomizeSections();

        return;

    }


    // ==========================================
    // CÀ PHÊ
    // Size + Sugar + Ice
    // topping chỉ hiện nếu DB có
    // ==========================================

    if (type === 1) {

        setSectionByContent(
            sizeSection,
            "#sizeGroup .size-btn"
        );

        setSectionByContent(
            sugarSection,
            "#sugarGroup input"
        );

        setSectionByContent(
            iceSection,
            "#iceGroup input"
        );

        setSectionByContent(
            toppingSection,
            "#toppingList .topping-option"
        );

        return;

    }


    // ==========================================
    // TRÀ
    // Size + Sugar + Ice + Topping DB
    // ==========================================

    if (type === 2) {

        setSectionByContent(
            sizeSection,
            "#sizeGroup .size-btn"
        );

        setSectionByContent(
            sugarSection,
            "#sugarGroup input"
        );

        setSectionByContent(
            iceSection,
            "#iceGroup input"
        );

        setSectionByContent(
            toppingSection,
            "#toppingList .topping-option"
        );

        return;

    }


    // ==========================================
    // MATCHA
    // ==========================================

    if (type === 3) {

        setSectionByContent(
            sizeSection,
            "#sizeGroup .size-btn"
        );

        setSectionByContent(
            sugarSection,
            "#sugarGroup input"
        );

        setSectionByContent(
            iceSection,
            "#iceGroup input"
        );

        setSectionByContent(
            toppingSection,
            "#toppingList .topping-option"
        );

        return;

    }


    // ==========================================
    // SODA
    // Size + Ice
    // Không sugar, không topping nếu DB không có
    // ==========================================

    if (type === 4) {

        setSectionByContent(
            sizeSection,
            "#sizeGroup .size-btn"
        );


        if (sugarSection)
            sugarSection.style.display =
                "none";


        setSectionByContent(
            iceSection,
            "#iceGroup input"
        );


        setSectionByContent(
            toppingSection,
            "#toppingList .topping-option"
        );

        return;

    }


    // ==========================================
    // NƯỚC ÉP
    // Size + Ice
    // ==========================================

    if (type === 6) {

        setSectionByContent(
            sizeSection,
            "#sizeGroup .size-btn"
        );


        if (sugarSection)
            sugarSection.style.display =
                "none";


        setSectionByContent(
            iceSection,
            "#iceGroup input"
        );


        setSectionByContent(
            toppingSection,
            "#toppingList .topping-option"
        );

        return;

    }


    // ==========================================
    // SỮA CHUA
    // Size + Sugar
    // ==========================================

    if (type === 7) {

        setSectionByContent(
            sizeSection,
            "#sizeGroup .size-btn"
        );


        setSectionByContent(
            sugarSection,
            "#sugarGroup input"
        );


        if (iceSection)
            iceSection.style.display =
                "none";


        setSectionByContent(
            toppingSection,
            "#toppingList .topping-option"
        );

        return;

    }


    // ==========================================
    // ĐÁ XAY
    // Size + Sugar
    // ==========================================

    if (type === 8) {

        setSectionByContent(
            sizeSection,
            "#sizeGroup .size-btn"
        );


        setSectionByContent(
            sugarSection,
            "#sugarGroup input"
        );


        if (iceSection)
            iceSection.style.display =
                "none";


        setSectionByContent(
            toppingSection,
            "#toppingList .topping-option"
        );

        return;

    }

}


// ======================================================
// SHOW SECTION IF HAS DATA
// ======================================================

function setSectionByContent(
    section,
    selector
) {

    if (!section)
        return;


    const hasData =
        document.querySelector(
            selector
        );


    section.style.display =
        hasData
            ? "block"
            : "none";

}


// ======================================================
// UPDATE POPUP PRICE
// ======================================================

function updatePopupPrice() {

    const modalPrice =
        getModalPrice();


    if (!modalPrice)
        return;


    let unitPrice =
        popupBasePrice;


    // ==========================================
    // SIZE
    // ==========================================

    const activeSize =
        document.querySelector(
            "#sizeGroup .size-btn.active"
        );


    if (activeSize) {

        unitPrice +=
            Number(
                activeSize.dataset.plus || 0
            );

    }


    // ==========================================
    // TOPPING
    // ==========================================

    document
        .querySelectorAll(
            "#toppingList .topping-option:checked"
        )
        .forEach(item => {

            unitPrice +=
                Number(
                    item.dataset.price || 0
                );

        });


    // ==========================================
    // QUANTITY
    // ==========================================

    const total =
        unitPrice *
        popupQuantity;


    modalPrice.textContent =
        total.toLocaleString(
            "vi-VN"
        ) + "đ";

}


// ======================================================
// POPUP QUANTITY
// ======================================================

function initPopupQuantity() {

    const plusQty =
        document.getElementById(
            "plusQty"
        );


    const minusQty =
        document.getElementById(
            "minusQty"
        );


    const qtyValue =
        document.getElementById(
            "qtyValue"
        );


    if (plusQty) {

        plusQty.onclick =
            function () {

                popupQuantity++;

                if (qtyValue)
                    qtyValue.textContent =
                        popupQuantity;

                updatePopupPrice();

            };

    }


    if (minusQty) {

        minusQty.onclick =
            function () {

                if (popupQuantity > 1) {

                    popupQuantity--;

                    if (qtyValue)
                        qtyValue.textContent =
                            popupQuantity;

                    updatePopupPrice();

                }

            };

    }

}


// ======================================================
// CLOSE POPUP
// ======================================================

function initClosePopup() {

    const modal =
        getModal();


    const closeButton =
        document.querySelector(
            ".close-modal"
        );


    const overlay =
        document.querySelector(
            ".modal-overlay"
        );


    if (closeButton) {

        closeButton.onclick =
            function () {

                if (modal)
                    modal.style.display =
                        "none";

            };

    }


    if (overlay) {

        overlay.onclick =
            function () {

                if (modal)
                    modal.style.display =
                        "none";

            };

    }


    document.addEventListener(
        "keydown",
        function (e) {

            if (
                e.key === "Escape" &&
                modal &&
                modal.style.display === "flex"
            ) {

                modal.style.display =
                    "none";

            }

        }
    );

}


// ======================================================
// ADD TO CART
// ======================================================

function initAddToCart() {

    const button =
        document.querySelector(
            ".btn-buy-modal"
        );


    if (!button)
        return;


    button.onclick =
        async function () {


            const modal =
                getModal();


            if (!modal)
                return;


            const productId =
                Number(
                    modal.dataset.id || 0
                );


            if (!productId)
                return;


            // ==========================================
            // SIZE
            // ==========================================

            const activeSize =
                document.querySelector(
                    "#sizeGroup .size-btn.active"
                );


            const size =
                activeSize?.dataset.size
                || "";


            const giaSize =
                Number(
                    activeSize?.dataset.plus || 0
                );


            // ==========================================
            // ICE
            // ==========================================

            const selectedIce =
                document.querySelector(
                    "input[name='ice']:checked"
                );


            const mucDa =
                selectedIce?.value
                || "";


            // ==========================================
            // SUGAR
            // ==========================================

            const selectedSugar =
                document.querySelector(
                    "input[name='sugar']:checked"
                );


            const doNgot =
                selectedSugar?.value
                || "";


            // ==========================================
            // NOTE
            // ==========================================

            const ghiChu =
                document
                    .getElementById(
                        "txtNote"
                    )
                    ?.value
                    ?.trim()
                || "";


            // ==========================================
            // TOPPING
            // ==========================================

            const toppingInputs =
                document.querySelectorAll(
                    "#toppingList .topping-option:checked"
                );


            const toppings = [];


            let giaTopping = 0;


            toppingInputs.forEach(item => {

                toppings.push(
                    item.value
                );


                giaTopping +=
                    Number(
                        item.dataset.price || 0
                    );

            });


            // ==========================================
            // FORM DATA
            // ==========================================

            const formData =
                new URLSearchParams();


            formData.append(
                "id",
                productId
            );


            // CartController đang có default,
            // nhưng gửi giá trị hợp lý nếu section bị ẩn.

            formData.append(
                "size",
                size || "S"
            );


            formData.append(
                "mucDa",
                mucDa || "Đá vừa"
            );


            formData.append(
                "doNgot",
                doNgot || "100%"
            );


            formData.append(
                "ghiChu",
                ghiChu
            );


            formData.append(
                "soLuong",
                popupQuantity
            );


            formData.append(
                "giaSize",
                giaSize
            );


            formData.append(
                "giaTopping",
                giaTopping
            );


            toppings.forEach(
                topping => {

                    formData.append(
                        "toppings",
                        topping
                    );

                }
            );


            // ==========================================
            // SEND
            // ==========================================

            const oldHtml =
                button.innerHTML;


            try {

                button.disabled =
                    true;


                button.innerHTML =
                    `
                    <i class="fa-solid fa-spinner fa-spin"></i>
                    <span>Đang thêm...</span>
                    `;


                const response =
                    await fetch(
                        "/Cart/AddToCart",
                        {

                            method: "POST",

                            headers: {

                                "Content-Type":
                                    "application/x-www-form-urlencoded; charset=UTF-8"

                            },

                            body:
                                formData.toString()

                        }
                    );


                if (!response.ok) {

                    throw new Error(
                        "AddToCart failed"
                    );

                }


                const data =
                    await response.json();


                if (!data.success) {

                    throw new Error(
                        "Không thể thêm sản phẩm."
                    );

                }


                // ==========================================
                // CART BADGE
                // ==========================================

                document
                    .querySelectorAll(
                        ".cart-count, #cartCount"
                    )
                    .forEach(badge => {

                        badge.textContent =
                            data.count;

                    });


                // ==========================================
                // TOAST CŨ
                // ==========================================

                if (
                    typeof showCartToast ===
                    "function"
                ) {

                    const modalImg =
                        getModalImg();


                    const modalName =
                        getModalName();


                    const totalPrice =
                        (
                            popupBasePrice +
                            giaSize +
                            giaTopping
                        )
                        *
                        popupQuantity;


                    showCartToast({

                        image:
                            modalImg?.src || "",

                        name:
                            modalName?.textContent || "",

                        size:
                            popupType === 5
                                ? ""
                                : size,

                        quantity:
                            popupQuantity,

                        total:
                            totalPrice

                    });

                }


                // ==========================================
                // SUCCESS BUTTON
                // ==========================================

                button.innerHTML =
                    `
                    <i class="fa-solid fa-check"></i>
                    <span>Đã thêm vào giỏ</span>
                    `;


                setTimeout(() => {

                    button.innerHTML =
                        oldHtml;


                    button.disabled =
                        false;


                    const modal =
                        getModal();


                    if (modal)
                        modal.style.display =
                            "none";

                }, 900);

            }
            catch (error) {

                console.error(
                    error
                );


                button.innerHTML =
                    oldHtml;


                button.disabled =
                    false;


                alert(
                    "Có lỗi khi thêm sản phẩm vào giỏ."
                );

            }

        };

}


// ======================================================
// FORMAT MONEY
// ======================================================

function formatMoney(value) {

    return Number(value || 0)
        .toLocaleString(
            "vi-VN"
        ) + "đ";

}


// ======================================================
// ESCAPE HTML
// ======================================================

function escapeHtml(value) {

    return String(value ?? "")
        .replace(
            /&/g,
            "&amp;"
        )
        .replace(
            /</g,
            "&lt;"
        )
        .replace(
            />/g,
            "&gt;"
        )
        .replace(
            /"/g,
            "&quot;"
        )
        .replace(
            /'/g,
            "&#039;"
        );

}


// ======================================================
// START
// ======================================================

document.addEventListener(
    "DOMContentLoaded",
    function () {

        initPopup();

        initQuantity();

        initFavorite();

        initPopupQuantity();

        initClosePopup();

        initAddToCart();

    }
);