// ======================================================
// ROSALIE COFFEE
// COMBO DETAIL
// ======================================================


// =========================================================
// LOCALIZATION
// =========================================================

function rosalieText(key, fallback = "") {
    return window.rosaliePromotionI18n?.[key] ?? fallback;
}

function rosalieFormat(key, fallback, ...values) {

    let text = rosalieText(key, fallback);

    values.forEach((value, index) => {
        text = text.replaceAll(`{${index}}`, value);
    });

    return text;
}


// ======================================================
// DOM READY
// ======================================================

document.addEventListener("DOMContentLoaded", function () {

    // ==================================================
    // ELEMENTS
    // ==================================================

    const modal =
        document.getElementById("comboModal");

    if (!modal) {
        return;
    }

    const modalImage =
        document.getElementById("comboModalImage");

    const modalName =
        document.getElementById("comboModalName");

    const modalDesc =
        document.getElementById("comboModalDesc");

    const modalDiscount =
        document.getElementById("comboDiscount");

    const modalOldPrice =
        document.getElementById("comboOldPrice");

    const modalNewPrice =
        document.getElementById("comboNewPrice");

    const productList =
        document.getElementById("comboProductList");

    const btnBuy =
        document.getElementById("comboBuyNow");

    const btnClose =
        document.querySelector(".combo-modal-close");


    let currentCombo = 0;


    // ==================================================
    // FORMAT MONEY
    // ==================================================

    function formatMoney(value) {

        const culture =
            document.documentElement.lang
                ?.toLowerCase()
                .startsWith("en")
                ? "en-US"
                : "vi-VN";


        const number =
            Number(value || 0)
                .toLocaleString(culture);


        return culture === "en-US"
            ? `${number} VND`
            : `${number}đ`;
    }


    // ==================================================
    // UPDATE CART BADGE
    // ==================================================

    function updateCartBadge(count) {

        document
            .querySelectorAll(
                ".cart-count, #cartCount, .cart-badge"
            )
            .forEach(badge => {

                badge.textContent = count;

            });
    }


    // ==================================================
    // SHOW COMBO TOAST
    // ==================================================

    function showComboToast(
        message,
        success = true
    ) {

        if (
            typeof promotionShowToast === "function"
        ) {

            promotionShowToast(
                message,
                success,
                success ? "combo" : "error"
            );

            return;
        }


        // fallback
        alert(message);
    }


    // ==================================================
    // OPEN COMBO DETAIL
    // ==================================================

    document
        .querySelectorAll(".btn-view-combo")
        .forEach(button => {

            button.addEventListener(
                "click",
                async function () {

                    currentCombo =
                        Number(this.dataset.id);


                    // ==============================
                    // CHECK ID
                    // ==============================

                    if (!currentCombo) {

                        showComboToast(

                            rosalieText(
                                "ComboInfoNotFound",
                                "Không tìm thấy thông tin combo."
                            ),

                            false
                        );

                        return;
                    }


                    try {

                        // ==============================
                        // FETCH COMBO
                        // ==============================

                        const response =
                            await fetch(
                                `/Promotion/GetComboDetail?id=${currentCombo}`
                            );


                        if (!response.ok) {

                            throw new Error(

                                rosalieText(
                                    "ComboLoadFailed",
                                    "Không tải được combo."
                                )

                            );
                        }


                        const data =
                            await response.json();


                        // ==============================
                        // IMAGE
                        // ==============================

                        if (modalImage) {

                            modalImage.src =
                                `/images/combo/${data.hinhAnh}`;

                            modalImage.alt =
                                data.tenCombo ||
                                "Combo Rosalie";
                        }


                        // ==============================
                        // NAME
                        // ==============================

                        if (modalName) {

                            modalName.textContent =
                                data.tenCombo || "";
                        }


                        // ==============================
                        // DESCRIPTION
                        // ==============================

                        if (modalDesc) {

                            modalDesc.textContent =
                                data.moTa ||

                                rosalieText(
                                    "DefaultComboDescription",
                                    "Combo Rosalie được kết hợp từ những món được yêu thích với mức giá tiết kiệm."
                                );
                        }


                        // ==============================
                        // DISCOUNT
                        // ==============================

                        if (modalDiscount) {

                            modalDiscount.textContent =
                                `-${data.phanTramGiam || 0}%`;
                        }


                        // ==============================
                        // OLD PRICE
                        // ==============================

                        if (modalOldPrice) {

                            modalOldPrice.textContent =
                                formatMoney(
                                    data.giaGoc
                                );
                        }


                        // ==============================
                        // NEW PRICE
                        // ==============================

                        if (modalNewPrice) {

                            modalNewPrice.textContent =
                                formatMoney(
                                    data.giaBan
                                );
                        }


                        // ==============================
                        // PRODUCTS
                        // ==============================

                        if (productList) {

                            productList.innerHTML = "";


                            if (
                                data.products &&
                                data.products.length > 0
                            ) {

                                data.products.forEach(item => {

                                    const row =
                                        document.createElement(
                                            "div"
                                        );


                                    row.className =
                                        "combo-item";


                                    row.innerHTML = `

                                        <span class="combo-item-name">

                                            <i class="bi bi-check-circle-fill"></i>

                                            ${item.ten}

                                        </span>

                                        <strong>

                                            x${item.soLuong}

                                        </strong>

                                    `;


                                    productList.appendChild(
                                        row
                                    );

                                });

                            }
                            else {

                                productList.innerHTML = `

                                    <div class="combo-empty">

                                        ${rosalieText(
                                    "ComboProductsEmpty",
                                    "Chưa có thông tin sản phẩm trong combo."
                                )}

                                    </div>

                                `;
                            }
                        }


                        // ==============================
                        // SHOW MODAL
                        // ==============================

                        modal.classList.add(
                            "show"
                        );

                        document.body.style.overflow =
                            "hidden";

                    }
                    catch (error) {

                        console.error(
                            "Get combo detail error:",
                            error
                        );


                        showComboToast(

                            rosalieText(
                                "ComboInfoLoadError",
                                "Không thể tải thông tin combo."
                            ),

                            false
                        );
                    }

                }
            );

        });


    // ==================================================
    // ADD COMBO BUTTON ON CARD
    // ==================================================

    document
        .querySelectorAll(".btn-combo")
        .forEach(button => {

            button.addEventListener(
                "click",
                async function () {

                    const comboId =
                        Number(
                            this.dataset.id
                        );


                    if (!comboId) {

                        showComboToast(

                            rosalieText(
                                "ComboNotFound",
                                "Không tìm thấy combo."
                            ),

                            false
                        );

                        return;
                    }


                    await addComboToCart(
                        comboId,
                        this
                    );

                }
            );

        });


    // ==================================================
    // ADD COMBO TO CART
    // ==================================================

    async function addComboToCart(
        comboId,
        button = null
    ) {

        if (!comboId) {
            return;
        }


        // ==============================
        // FORM
        // ==============================

        const form =
            new URLSearchParams();


        form.append(
            "comboId",
            comboId
        );


        let oldHtml = null;


        // ==============================
        // LOADING BUTTON
        // ==============================

        if (button) {

            oldHtml =
                button.innerHTML;


            button.disabled =
                true;


            button.innerHTML = `

                <i class="bi bi-arrow-repeat"></i>

                ${rosalieText(
                "Adding",
                "Đang thêm..."
            )}

            `;
        }


        try {

            // ==============================
            // REQUEST
            // ==============================

            const response =
                await fetch(
                    "/Cart/AddCombo",
                    {
                        method: "POST",

                        headers: {

                            "Content-Type":
                                "application/x-www-form-urlencoded; charset=UTF-8"

                        },

                        body:
                            form.toString()
                    }
                );


            if (!response.ok) {

                throw new Error(

                    rosalieText(
                        "ComboAddFailed",
                        "Không thể thêm combo."
                    )

                );
            }


            const data =
                await response.json();


            if (!data.success) {

                throw new Error(

                    rosalieText(
                        "ComboServerAddFailed",
                        "Server không thể thêm combo."
                    )

                );
            }


            // ==============================
            // UPDATE CART
            // ==============================

            updateCartBadge(
                data.count
            );


            // ==============================
            // SUCCESS BUTTON
            // ==============================

            if (button) {

                button.innerHTML = `

                    <i class="bi bi-check-lg"></i>

                    ${rosalieText(
                    "Added",
                    "Đã thêm"
                )}

                `;
            }


            // ==============================
            // GET COMBO NAME
            // ==============================

            let comboName =
                "Combo Rosalie";


            if (button) {

                const card =
                    button.closest(
                        ".combo-card"
                    );


                if (card) {

                    const nameElement =
                        card.querySelector(
                            ".combo-name, h3, h4"
                        );


                    if (nameElement) {

                        comboName =
                            nameElement
                                .textContent
                                .trim();
                    }
                }
            }


            // ==============================
            // SUCCESS TOAST
            // ==============================

            showComboToast(

                rosalieFormat(
                    "ComboAddedSuccess",
                    'Combo "{0}" đã được thêm vào giỏ hàng.',
                    comboName
                ),

                true
            );


            // ==============================
            // RESTORE BUTTON
            // ==============================

            setTimeout(
                function () {

                    if (
                        button &&
                        oldHtml !== null
                    ) {

                        button.innerHTML =
                            oldHtml;

                        button.disabled =
                            false;
                    }

                },
                1200
            );

        }
        catch (error) {

            console.error(
                "Add combo error:",
                error
            );


            // ==============================
            // RESTORE BUTTON
            // ==============================

            if (
                button &&
                oldHtml !== null
            ) {

                button.innerHTML =
                    oldHtml;

                button.disabled =
                    false;
            }


            // ==============================
            // ERROR TOAST
            // ==============================

            showComboToast(

                rosalieText(
                    "ComboAddCartError",
                    "Không thể thêm combo vào giỏ hàng. Vui lòng thử lại."
                ),

                false
            );
        }
    }


    // ==================================================
    // BUY COMBO INSIDE MODAL
    // ==================================================

    btnBuy?.addEventListener(
        "click",
        async function () {

            if (!currentCombo) {
                return;
            }


            const oldHtml =
                this.innerHTML;


            // ==============================
            // LOADING
            // ==============================

            this.disabled =
                true;


            this.innerHTML = `

                <i class="bi bi-arrow-repeat"></i>

                ${rosalieText(
                "Adding",
                "Đang thêm..."
            )}

            `;


            // ==============================
            // FORM
            // ==============================

            const form =
                new URLSearchParams();


            form.append(
                "comboId",
                currentCombo
            );


            try {

                // ==============================
                // REQUEST
                // ==============================

                const response =
                    await fetch(
                        "/Cart/AddCombo",
                        {
                            method: "POST",

                            headers: {

                                "Content-Type":
                                    "application/x-www-form-urlencoded; charset=UTF-8"

                            },

                            body:
                                form.toString()
                        }
                    );


                if (!response.ok) {

                    throw new Error(
                        "Add combo failed"
                    );
                }


                const data =
                    await response.json();


                if (!data.success) {

                    throw new Error(
                        "Add combo returned failure"
                    );
                }


                // ==============================
                // UPDATE CART
                // ==============================

                updateCartBadge(
                    data.count
                );


                // ==============================
                // SUCCESS BUTTON
                // ==============================

                this.innerHTML = `

                    <i class="bi bi-check-lg"></i>

                    ${rosalieText(
                    "AddedToCart",
                    "Đã thêm vào giỏ"
                )}

                `;


                // ==============================
                // COMBO NAME
                // ==============================

                const comboName =
                    modalName
                        ?.textContent
                        ?.trim()
                    ||
                    "Combo Rosalie";


                // ==============================
                // SUCCESS TOAST
                // ==============================

                showComboToast(

                    rosalieFormat(
                        "ComboAddedSuccess",
                        'Combo "{0}" đã được thêm vào giỏ hàng.',
                        comboName
                    ),

                    true
                );


                // ==============================
                // CLOSE MODAL
                // ==============================

                setTimeout(
                    () => {

                        modal.classList.remove(
                            "show"
                        );


                        document.body.style.overflow =
                            "";


                        this.innerHTML =
                            oldHtml;


                        this.disabled =
                            false;

                    },
                    900
                );

            }
            catch (error) {

                console.error(
                    "Buy combo error:",
                    error
                );


                // ==============================
                // RESTORE BUTTON
                // ==============================

                this.innerHTML =
                    oldHtml;


                this.disabled =
                    false;


                // ==============================
                // ERROR TOAST
                // ==============================

                showComboToast(

                    rosalieText(
                        "ComboAddCartError",
                        "Không thể thêm combo vào giỏ hàng. Vui lòng thử lại."
                    ),

                    false
                );
            }

        }
    );


    // ==================================================
    // CLOSE MODAL BUTTON
    // ==================================================

    btnClose?.addEventListener(
        "click",
        function () {

            modal.classList.remove(
                "show"
            );

            document.body.style.overflow =
                "";

        }
    );


    // ==================================================
    // CLICK OUTSIDE MODAL
    // ==================================================

    modal.addEventListener(
        "click",
        function (event) {

            if (
                event.target === modal
            ) {

                modal.classList.remove(
                    "show"
                );

                document.body.style.overflow =
                    "";
            }

        }
    );


    // ==================================================
    // ESC CLOSE
    // ==================================================

    document.addEventListener(
        "keydown",
        function (event) {

            if (
                event.key === "Escape" &&
                modal.classList.contains(
                    "show"
                )
            ) {

                modal.classList.remove(
                    "show"
                );

                document.body.style.overflow =
                    "";
            }

        }
    );

});