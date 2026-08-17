// ======================================================
// ROSALIE COFFEE
// PRODUCT DETAIL
// ======================================================

document.addEventListener(
    "DOMContentLoaded",
    function () {

        // ==================================================
        // PRODUCT DATA
        // ==================================================

        const dataElement =
            document.getElementById(
                "productDetailData"
            );


        if (!dataElement) {
            return;
        }


        const detailBasePrice =
            Number(
                dataElement.dataset.basePrice || 0
            );


        const detailProductName =
            dataElement.dataset.productName || "";


        const detailProductId =
            Number(
                dataElement.dataset.productId || 0
            );


        const detailProductType =
            Number(
                dataElement.dataset.productType || 0
            );


        const addCartUrl =
            dataElement.dataset.addCartUrl;


        // ==================================================
        // SOURCE
        //
        // MENU:
        //     source = menu
        //
        // PROMOTION:
        //     source = promotion
        // ==================================================

        const urlParams =
            new URLSearchParams(
                window.location.search
            );


        const detailSource =
            (
                dataElement.dataset.source
                ||
                urlParams.get("source")
                ||
                "menu"
            )
                .toLowerCase();


        // ==================================================
        // STATE
        // ==================================================

        let detailQuantity =
            1;


        let detailToastTimer =
            null;


        // ==================================================
        // ELEMENTS
        // ==================================================

        const qtyText =
            document.getElementById(
                "detailQty"
            );


        const minusButton =
            document.getElementById(
                "detailMinus"
            );


        const plusButton =
            document.getElementById(
                "detailPlus"
            );


        const priceElement =
            document.getElementById(
                "detailPrice"
            );


        const addCartButton =
            document.getElementById(
                "detailAddCart"
            );


        const sizeButtons =
            document.querySelectorAll(
                ".detail-size-btn"
            );


        const toppingInputs =
            document.querySelectorAll(
                ".detail-topping"
            );


        // ==================================================
        // SIZE
        // ==================================================

        sizeButtons.forEach(
            button => {

                button.addEventListener(
                    "click",
                    function () {

                        sizeButtons.forEach(
                            item => {

                                item.classList.remove(
                                    "active"
                                );

                            }
                        );


                        this.classList.add(
                            "active"
                        );


                        updateDetailPrice();

                    }
                );

            }
        );


        // ==================================================
        // TOPPING
        // ==================================================

        toppingInputs.forEach(
            input => {

                input.addEventListener(
                    "change",
                    function () {

                        updateDetailPrice();

                    }
                );

            }
        );


        // ==================================================
        // QUANTITY -
        // ==================================================

        minusButton?.addEventListener(
            "click",
            function () {

                if (detailQuantity <= 1) {
                    return;
                }


                detailQuantity--;


                if (qtyText) {

                    qtyText.textContent =
                        detailQuantity;

                }


                updateDetailPrice();

            }
        );


        // ==================================================
        // QUANTITY +
        // ==================================================

        plusButton?.addEventListener(
            "click",
            function () {

                detailQuantity++;


                if (qtyText) {

                    qtyText.textContent =
                        detailQuantity;

                }


                updateDetailPrice();

            }
        );


        // ==================================================
        // GET SIZE PRICE
        // ==================================================

        function getDetailSizePrice() {

            const selectedSize =
                document.querySelector(
                    ".detail-size-btn.active"
                );


            return Number(
                selectedSize?.dataset.plus || 0
            );

        }


        // ==================================================
        // GET TOPPING PRICE
        // ==================================================

        function getDetailToppingPrice() {

            let total =
                0;


            document
                .querySelectorAll(
                    ".detail-topping:checked"
                )
                .forEach(
                    item => {

                        total +=
                            Number(
                                item.dataset.price || 0
                            );

                    }
                );


            return total;

        }


        // ==================================================
        // UPDATE PRICE
        // ==================================================

        function updateDetailPrice() {

            const unitPrice =
                detailBasePrice
                +
                getDetailSizePrice()
                +
                getDetailToppingPrice();


            const total =
                unitPrice
                *
                detailQuantity;


            if (!priceElement) {
                return;
            }


            priceElement.textContent =
                total.toLocaleString(
                    "vi-VN"
                )
                +
                "đ";

        }


        // ==================================================
        // TOAST
        // ==================================================

        function showDetailToast(
            productName,
            size,
            quantity,
            total
        ) {

            const toast =
                document.getElementById(
                    "detailToast"
                );


            const title =
                document.getElementById(
                    "detailToastTitle"
                );


            const message =
                document.getElementById(
                    "detailToastMessage"
                );


            if (!toast) {
                return;
            }


            if (title) {

                title.textContent =
                    "Đã thêm vào giỏ hàng";

            }


            if (message) {

                let text =
                    `${productName} · SL ${quantity}`;


                if (size) {

                    text +=
                        ` · Size ${size}`;

                }


                text +=
                    ` · ${total.toLocaleString("vi-VN")}đ`;


                message.textContent =
                    text;

            }


            toast.classList.remove(
                "show"
            );


            void toast.offsetWidth;


            toast.classList.add(
                "show"
            );


            clearTimeout(
                detailToastTimer
            );


            detailToastTimer =
                setTimeout(
                    function () {

                        toast.classList.remove(
                            "show"
                        );

                    },
                    4000
                );

        }


        // ==================================================
        // RESET OPTIONS
        // ==================================================

        function resetDetailOptions() {

            // ==============================================
            // SIZE
            // ==============================================

            sizeButtons.forEach(
                button => {

                    button.classList.remove(
                        "active"
                    );

                }
            );


            if (sizeButtons.length > 0) {

                sizeButtons[0]
                    .classList.add(
                        "active"
                    );

            }


            // ==============================================
            // SUGAR
            // ==============================================

            const sugarOptions =
                document.querySelectorAll(
                    'input[name="detailSugar"]'
                );


            sugarOptions.forEach(
                (input, index) => {

                    input.checked =
                        index === 0;

                }
            );


            // ==============================================
            // ICE
            // ==============================================

            const iceOptions =
                document.querySelectorAll(
                    'input[name="detailIce"]'
                );


            iceOptions.forEach(
                (input, index) => {

                    input.checked =
                        index === 0;

                }
            );


            // ==============================================
            // TOPPING
            // ==============================================

            document
                .querySelectorAll(
                    ".detail-topping"
                )
                .forEach(
                    input => {

                        input.checked =
                            false;

                    }
                );


            // ==============================================
            // QUANTITY
            // ==============================================

            detailQuantity =
                1;


            if (qtyText) {

                qtyText.textContent =
                    "1";

            }


            // ==============================================
            // PRICE
            // ==============================================

            updateDetailPrice();

        }


        // ==================================================
        // UPDATE CART BADGE
        // ==================================================

        function updateCartBadge(
            count
        ) {

            document
                .querySelectorAll(
                    ".cart-count, #cartCount, .cart-badge"
                )
                .forEach(
                    badge => {

                        badge.textContent =
                            count;

                    }
                );

        }


        // ==================================================
        // ADD TO CART
        // ==================================================

        addCartButton?.addEventListener(
            "click",
            async function () {

                const button =
                    this;


                // ==========================================
                // SIZE
                // ==========================================

                const selectedSize =
                    document.querySelector(
                        ".detail-size-btn.active"
                    );


                const size =
                    selectedSize?.dataset.size || "";


                const giaSize =
                    Number(
                        selectedSize?.dataset.plus || 0
                    );


                // ==========================================
                // SUGAR
                // ==========================================

                const selectedSugar =
                    document.querySelector(
                        'input[name="detailSugar"]:checked'
                    );


                const doNgot =
                    selectedSugar?.value || "";


                // ==========================================
                // ICE
                // ==========================================

                const selectedIce =
                    document.querySelector(
                        'input[name="detailIce"]:checked'
                    );


                const mucDa =
                    selectedIce?.value || "";


                // ==========================================
                // TOPPING
                // ==========================================

                const selectedToppings =
                    Array.from(
                        document.querySelectorAll(
                            ".detail-topping:checked"
                        )
                    );


                const toppings =
                    selectedToppings.map(
                        item => item.value
                    );


                const giaTopping =
                    selectedToppings.reduce(
                        (total, item) => {

                            return total
                                +
                                Number(
                                    item.dataset.price || 0
                                );

                        },
                        0
                    );


                // ==========================================
                // LƯU THÔNG TIN TRƯỚC KHI RESET
                // ==========================================

                const quantityAdded =
                    detailQuantity;


                const totalAdded =
                    (
                        detailBasePrice
                        +
                        giaSize
                        +
                        giaTopping
                    )
                    *
                    quantityAdded;


                // ==========================================
                // FORM DATA
                // ==========================================

                const formData =
                    new URLSearchParams();


                formData.append(
                    "id",
                    detailProductId
                );


                // ==========================================
                // SOURCE
                // ==========================================

                formData.append(
                    "source",
                    detailSource
                );


                formData.append(
                    "size",
                    size
                );


                formData.append(
                    "mucDa",
                    mucDa
                );


                formData.append(
                    "doNgot",
                    doNgot
                );


                formData.append(
                    "ghiChu",
                    ""
                );


                formData.append(
                    "soLuong",
                    quantityAdded
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
                // BUTTON LOADING
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


                    // ======================================
                    // REQUEST
                    // ======================================

                    const response =
                        await fetch(
                            addCartUrl,
                            {
                                method:
                                    "POST",

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
                            "Không thể kết nối đến giỏ hàng."
                        );

                    }


                    const result =
                        await response.json();


                    if (!result.success) {

                        throw new Error(
                            result.message
                            ||
                            "Không thể thêm sản phẩm vào giỏ."
                        );

                    }


                    // ======================================
                    // UPDATE CART COUNT
                    // ======================================

                    updateCartBadge(
                        result.count
                    );


                    // ======================================
                    // SUCCESS BUTTON
                    // ======================================

                    button.innerHTML =
                        `
                            <i class="fa-solid fa-check"></i>
                            <span>Đã thêm vào giỏ</span>
                        `;


                    // ======================================
                    // SHOW TOAST
                    // ======================================

                    showDetailToast(
                        detailProductName,
                        size,
                        quantityAdded,
                        totalAdded
                    );


                    // ======================================
                    // RESET OPTIONS
                    // ======================================

                    resetDetailOptions();


                    // ======================================
                    // RESET BUTTON
                    // ======================================

                    setTimeout(
                        function () {

                            button.innerHTML =
                                oldHtml;


                            button.disabled =
                                false;

                        },
                        1200
                    );

                }
                catch (error) {

                    console.error(
                        "Add to cart error:",
                        error
                    );


                    button.innerHTML =
                        oldHtml;


                    button.disabled =
                        false;


                    alert(
                        error.message
                        ||
                        "Có lỗi khi thêm sản phẩm vào giỏ hàng."
                    );

                }

            }
        );


        // ==================================================
        // PRODUCT DESCRIPTION TABS
        // ==================================================

        const descriptionTabs =
            document.querySelectorAll(
                ".product-description-tab"
            );


        const descriptionPanel =
            document.getElementById(
                "productDescriptionTab"
            );


        const informationPanel =
            document.getElementById(
                "productInformationTab"
            );


        descriptionTabs.forEach(
            tab => {

                tab.addEventListener(
                    "click",
                    function () {

                        const selectedTab =
                            this.dataset.tab;


                        // ======================================
                        // ACTIVE TAB
                        // ======================================

                        descriptionTabs.forEach(
                            item => {

                                item.classList.remove(
                                    "active"
                                );

                            }
                        );


                        this.classList.add(
                            "active"
                        );


                        // ======================================
                        // DESCRIPTION
                        // ======================================

                        descriptionPanel
                            ?.classList
                            .toggle(
                                "active",
                                selectedTab ===
                                "description"
                            );


                        // ======================================
                        // INFORMATION
                        // ======================================

                        informationPanel
                            ?.classList
                            .toggle(
                                "active",
                                selectedTab ===
                                "information"
                            );

                    }
                );

            }
        );


        // ==================================================
        // INITIAL PRICE
        // ==================================================

        updateDetailPrice();

    }
);