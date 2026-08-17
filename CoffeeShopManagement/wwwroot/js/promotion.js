// =========================================================
// ROSALIE COFFEE
// PROMOTION.JS
// =========================================================


// =========================================================
// LOCALIZATION
// =========================================================

function rosalieText(
    key,
    fallback = ""
) {

    return window
        .rosaliePromotionI18n?.[key]
        ?? fallback;
}


function rosalieFormat(
    key,
    fallback,
    ...values
) {

    let text =
        rosalieText(
            key,
            fallback
        );


    values.forEach(
        (value, index) => {

            text =
                text.replaceAll(
                    `{${index}}`,
                    value
                );

        }
    );


    return text;
}


// =========================================================
// FORMAT MONEY
// =========================================================

function promotionFormatMoney(
    value
) {

    return Number(
        value || 0
    ).toLocaleString(
        "vi-VN"
    ) + "đ";
}


// =========================================================
// PROMOTION TOAST
// Voucher + Combo + Product + Error
// =========================================================

window.promotionShowToast =
    function (
        message,
        success = true,
        type = "voucher"
    ) {

        const toast =
            document.getElementById(
                "promotionToast"
            );


        if (!toast) {
            return;
        }


        let title = "";

        let headerIcon = "";

        let bodyIcon = "";


        // =================================================
        // ERROR
        // =================================================

        if (!success) {

            title =
                rosalieText(
                    "ToastErrorTitle",
                    "Có lỗi xảy ra"
                );


            headerIcon =
                "bi-x-lg";


            bodyIcon =
                "bi-exclamation-circle-fill";
        }

        // =================================================
        // COMBO
        // =================================================

        else if (
            type === "combo"
        ) {

            title =
                rosalieText(
                    "ToastAddedToCartTitle",
                    "Đã thêm vào giỏ"
                );


            headerIcon =
                "bi-check-lg";


            bodyIcon =
                "bi-cart-check-fill";
        }

        // =================================================
        // PRODUCT
        // =================================================

        else if (
            type === "product"
        ) {

            title =
                rosalieText(
                    "ToastAddedToCartTitle",
                    "Đã thêm vào giỏ"
                );


            headerIcon =
                "bi-check-lg";


            bodyIcon =
                "bi-cart-check-fill";
        }

        // =================================================
        // VOUCHER
        // =================================================

        else {

            title =
                rosalieText(
                    "ToastVoucherSavedTitle",
                    "Đã lưu voucher"
                );


            headerIcon =
                "bi-check-lg";


            bodyIcon =
                "bi-ticket-perforated-fill";
        }


        // =================================================
        // HTML
        // =================================================

        toast.innerHTML = `

            <div class="promotion-toast-header">

                <div class="promotion-toast-icon">

                    <i class="bi ${headerIcon}">
                    </i>

                </div>

                <span>
                    ${title}
                </span>

                <button type="button"
                        class="promotion-toast-close"
                        aria-label="${rosalieText(
            "Close",
            "Đóng"
        )}">

                    <i class="bi bi-x-lg">
                    </i>

                </button>

            </div>


            <div class="promotion-toast-body">

                <i class="bi ${bodyIcon}">
                </i>

                <span>
                    ${message}
                </span>

            </div>
        `;


        // =================================================
        // SHOW
        // =================================================

        toast
            .classList
            .remove(
                "show"
            );


        void toast.offsetWidth;


        toast
            .classList
            .add(
                "show"
            );


        // =================================================
        // CLOSE BUTTON
        // =================================================

        const closeButton =
            toast.querySelector(
                ".promotion-toast-close"
            );


        closeButton
            ?.addEventListener(
                "click",
                () => {

                    toast
                        .classList
                        .remove(
                            "show"
                        );

                }
            );


        // =================================================
        // AUTO CLOSE
        // =================================================

        clearTimeout(
            window.promotionToastTimer
        );


        window.promotionToastTimer =
            setTimeout(
                () => {

                    toast
                        .classList
                        .remove(
                            "show"
                        );

                },
                3000
            );
    };


// =========================================================
// UPDATE CART BADGE
// =========================================================

window.promotionUpdateCart =
    function (
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

    };


// =========================================================
// DOM READY
// =========================================================

document.addEventListener(
    "DOMContentLoaded",
    function () {


        // =================================================
        // HERO SLIDER
        // =================================================

        const slides =
            document.querySelectorAll(
                ".promotion-slide"
            );


        const dots =
            document.querySelectorAll(
                ".promotion-dot"
            );


        let currentSlide =
            0;


        function showSlide(
            index
        ) {

            if (
                slides.length === 0
            ) {
                return;
            }


            slides.forEach(
                slide => {

                    slide
                        .classList
                        .remove(
                            "active"
                        );

                }
            );


            dots.forEach(
                dot => {

                    dot
                        .classList
                        .remove(
                            "active"
                        );

                }
            );


            currentSlide =
                (
                    index
                    + slides.length
                )
                %
                slides.length;


            slides[
                currentSlide
            ]
                ?.classList
                .add(
                    "active"
                );


            dots[
                currentSlide
            ]
                ?.classList
                .add(
                    "active"
                );

        }


        function nextSlide() {

            showSlide(
                currentSlide + 1
            );

        }


        dots.forEach(
            (
                dot,
                index
            ) => {

                dot.addEventListener(
                    "click",
                    () => {

                        showSlide(
                            index
                        );

                    }
                );

            }
        );


        if (
            slides.length > 1
        ) {

            setInterval(
                nextSlide,
                5000
            );

        }


        // =================================================
        // FLASH SALE COUNTDOWN
        // =================================================

        const flashHour =
            document.getElementById(
                "flashHour"
            );


        const flashMinute =
            document.getElementById(
                "flashMinute"
            );


        const flashSecond =
            document.getElementById(
                "flashSecond"
            );


        const flashEnd =
            new Date();


        // Flash Sale 3 giờ
        flashEnd.setHours(
            flashEnd.getHours() + 3
        );


        function updateFlashCountdown() {

            if (
                !flashHour ||
                !flashMinute ||
                !flashSecond
            ) {
                return;
            }


            const now =
                new Date();


            let distance =
                flashEnd.getTime()
                -
                now.getTime();


            if (
                distance <= 0
            ) {

                flashHour.textContent =
                    "00";


                flashMinute.textContent =
                    "00";


                flashSecond.textContent =
                    "00";


                return;
            }


            const hours =
                Math.floor(
                    distance
                    /
                    (
                        1000
                        *
                        60
                        *
                        60
                    )
                );


            const minutes =
                Math.floor(
                    (
                        distance
                        %
                        (
                            1000
                            *
                            60
                            *
                            60
                        )
                    )
                    /
                    (
                        1000
                        *
                        60
                    )
                );


            const seconds =
                Math.floor(
                    (
                        distance
                        %
                        (
                            1000
                            *
                            60
                        )
                    )
                    /
                    1000
                );


            flashHour.textContent =
                String(
                    hours
                )
                    .padStart(
                        2,
                        "0"
                    );


            flashMinute.textContent =
                String(
                    minutes
                )
                    .padStart(
                        2,
                        "0"
                    );


            flashSecond.textContent =
                String(
                    seconds
                )
                    .padStart(
                        2,
                        "0"
                    );

        }


        updateFlashCountdown();


        setInterval(
            updateFlashCountdown,
            1000
        );


        // =================================================
        // BACK TO TOP
        // =================================================

        const backTop =
            document.querySelector(
                ".promotion-backtop"
            );


        function toggleBackTop() {

            if (!backTop) {
                return;
            }


            if (
                window.scrollY > 350
            ) {

                backTop.style.display =
                    "flex";

            }
            else {

                backTop.style.display =
                    "none";

            }

        }


        window.addEventListener(
            "scroll",
            toggleBackTop
        );


        toggleBackTop();


        backTop
            ?.addEventListener(
                "click",
                () => {

                    window.scrollTo(
                        {
                            top: 0,

                            behavior:
                                "smooth"
                        }
                    );

                }
            );


        // =================================================
        // LOADER
        // =================================================

        const loader =
            document.querySelector(
                ".promotion-loader"
            );


        if (loader) {

            setTimeout(
                () => {

                    loader
                        .classList
                        .add(
                            "hide"
                        );

                },
                250
            );

        }


        // =================================================
        // SMOOTH ANCHOR
        // =================================================

        document
            .querySelectorAll(
                'a[href^="#"]'
            )
            .forEach(
                link => {

                    link.addEventListener(
                        "click",
                        function (
                            event
                        ) {

                            const href =
                                this.getAttribute(
                                    "href"
                                );


                            if (
                                !href ||
                                href === "#"
                            ) {
                                return;
                            }


                            const target =
                                document
                                    .querySelector(
                                        href
                                    );


                            if (!target) {
                                return;
                            }


                            event.preventDefault();


                            target.scrollIntoView(
                                {
                                    behavior:
                                        "smooth",

                                    block:
                                        "start"
                                }
                            );

                        }
                    );

                }
            );


        // =================================================
        // SCROLL REVEAL
        // =================================================

        function revealOnScroll() {

            const trigger =
                window.innerHeight
                *
                0.92;


            document
                .querySelectorAll(
                    ".promotion-card, " +
                    ".combo-card, " +
                    ".promotion-voucher-card"
                )
                .forEach(
                    item => {

                        // Card đang bị ẩn thì bỏ qua
                        if (
                            getComputedStyle(
                                item
                            ).display === "none"
                        ) {
                            return;
                        }


                        const top =
                            item
                                .getBoundingClientRect()
                                .top;


                        if (
                            top < trigger
                        ) {

                            item
                                .classList
                                .add(
                                    "show"
                                );

                        }

                    }
                );

        }


        window.addEventListener(
            "scroll",
            revealOnScroll
        );


        revealOnScroll();


        // =================================================
        // PRODUCT CARD HOVER
        // =================================================

        document
            .querySelectorAll(
                ".promotion-card"
            )
            .forEach(
                card => {

                    card.addEventListener(
                        "mouseenter",
                        function () {

                            this
                                .classList
                                .add(
                                    "hover"
                                );

                        }
                    );


                    card.addEventListener(
                        "mouseleave",
                        function () {

                            this
                                .classList
                                .remove(
                                    "hover"
                                );

                        }
                    );

                }
            );


        // =================================================
        // VOUCHERS
        // =================================================

        let savedVouchers =
            [];


        try {

            savedVouchers =
                JSON.parse(
                    localStorage
                        .getItem(
                            "savedVouchers"
                        )
                )
                ||
                [];

        }
        catch {

            savedVouchers =
                [];

        }


        const voucherButtons =
            document.querySelectorAll(
                ".promotion-voucher-button"
            );


        // =================================================
        // RESTORE VOUCHERS
        // =================================================

        voucherButtons.forEach(
            button => {

                const code =
                    (
                        button.dataset.code
                        ||
                        ""
                    )
                        .trim();


                if (!code) {
                    return;
                }


                const exists =
                    savedVouchers.some(
                        voucher =>
                            voucher.code
                            ===
                            code
                    );


                if (exists) {

                    button.disabled =
                        true;


                    button.classList.add(
                        "saved"
                    );


                    button.innerHTML = `

                        <i class="bi bi-check-circle-fill">
                        </i>

                        ${rosalieText(
                        "VoucherSavedButton",
                        "Đã lưu"
                    )}
                    `;

                }

            }
        );


        // =================================================
        // SAVE VOUCHER
        // =================================================

        voucherButtons.forEach(
            button => {

                button.addEventListener(
                    "click",
                    function () {

                        if (
                            this.disabled
                        ) {
                            return;
                        }


                        const card =
                            this.closest(
                                ".promotion-voucher-card"
                            );


                        if (!card) {
                            return;
                        }


                        const code =
                            (
                                this.dataset.code
                                ||
                                card
                                    .querySelector(
                                        ".promotion-voucher-code"
                                    )
                                    ?.textContent
                                ||
                                ""
                            )
                                .trim();


                        const discount =
                            Number(
                                this.dataset.discount
                            )
                            ||
                            0;


                        const minOrder =
                            Number(
                                this.dataset.min
                            )
                            ||
                            0;


                        // =========================================
                        // CODE NOT FOUND
                        // =========================================

                        if (!code) {

                            window
                                .promotionShowToast(
                                    rosalieText(
                                        "VoucherCodeNotFound",
                                        "Không tìm thấy mã voucher."
                                    ),

                                    false,

                                    "voucher"
                                );


                            return;
                        }


                        // =========================================
                        // CHECK EXIST
                        // =========================================

                        const exists =
                            savedVouchers.some(
                                voucher =>
                                    voucher.code
                                    ===
                                    code
                            );


                        if (exists) {

                            this.disabled =
                                true;


                            this
                                .classList
                                .add(
                                    "saved"
                                );


                            this.innerHTML = `

                                <i class="bi bi-check-circle-fill">
                                </i>

                                ${rosalieText(
                                "VoucherSavedButton",
                                "Đã lưu"
                            )}
                            `;


                            return;
                        }


                        // =========================================
                        // SAVE
                        // =========================================

                        savedVouchers.push(
                            {
                                code:
                                    code,

                                discount:
                                    discount,

                                minOrder:
                                    minOrder
                            }
                        );


                        localStorage.setItem(
                            "savedVouchers",

                            JSON.stringify(
                                savedVouchers
                            )
                        );


                        // =========================================
                        // BUTTON STATE
                        // =========================================

                        this.disabled =
                            true;


                        this
                            .classList
                            .add(
                                "saved"
                            );


                        this.innerHTML = `

                            <i class="bi bi-check-circle-fill">
                            </i>

                            ${rosalieText(
                            "VoucherSavedButton",
                            "Đã lưu"
                        )}
                        `;


                        // =========================================
                        // TOAST
                        // =========================================

                        window
                            .promotionShowToast(
                                rosalieFormat(
                                    "VoucherSavedSuccess",

                                    "Voucher {0} đã được lưu thành công.",

                                    code
                                ),

                                true,

                                "voucher"
                            );

                    }
                );

            }
        );


        // =================================================
        // VOUCHER RIPPLE
        // =================================================

        document
            .querySelectorAll(
                ".promotion-voucher-button"
            )
            .forEach(
                button => {

                    button.addEventListener(
                        "click",
                        function (
                            event
                        ) {

                            if (
                                this.disabled
                            ) {
                                return;
                            }


                            const ripple =
                                document
                                    .createElement(
                                        "span"
                                    );


                            ripple.className =
                                "promotion-ripple";


                            const rect =
                                this
                                    .getBoundingClientRect();


                            ripple.style.left =
                                (
                                    event.clientX
                                    -
                                    rect.left
                                )
                                +
                                "px";


                            ripple.style.top =
                                (
                                    event.clientY
                                    -
                                    rect.top
                                )
                                +
                                "px";


                            this.appendChild(
                                ripple
                            );


                            setTimeout(
                                () => {

                                    ripple.remove();

                                },
                                600
                            );

                        }
                    );

                }
            );


        // =========================================================
        // SHOW ALL PRODUCTS
        //
        // Ban đầu: 4 sản phẩm
        // Bấm xem tất cả: hiện toàn bộ
        // =========================================================

        const showAllProductButton =
            document.getElementById(
                "showAllProduct"
            );


        const extraProducts =
            Array.from(
                document.querySelectorAll(
                    ".extra-product"
                )
            );


        let productsExpanded =
            false;


        function updateProductVisibility() {

            extraProducts.forEach(
                item => {

                    if (
                        productsExpanded
                    ) {

                        // =====================================
                        // HIỆN SẢN PHẨM
                        // =====================================

                        item
                            .classList
                            .remove(
                                "extra-product"
                            );


                        item.style
                            .removeProperty(
                                "display"
                            );


                        item
                            .classList
                            .add(
                                "show"
                            );

                    }
                    else {

                        // =====================================
                        // ẨN SẢN PHẨM
                        // =====================================

                        item
                            .classList
                            .add(
                                "extra-product"
                            );


                        item.style
                            .setProperty(
                                "display",
                                "none",
                                "important"
                            );

                    }

                }
            );


            // =============================================
            // BUTTON TEXT
            // =============================================

            if (
                showAllProductButton
            ) {

                showAllProductButton
                    .innerHTML =

                    productsExpanded

                        ? `${rosalieText(
                            "Collapse",
                            "Thu gọn"
                        )} ▲`

                        : `${rosalieText(
                            "ViewAllProducts",
                            "Xem tất cả sản phẩm"
                        )} ▼`;

            }

        }


        // =================================================
        // PRODUCT INITIAL
        // =================================================

        updateProductVisibility();


        // =================================================
        // PRODUCT CLICK
        // =================================================

        showAllProductButton
            ?.addEventListener(
                "click",
                function () {

                    productsExpanded =
                        !productsExpanded;


                    updateProductVisibility();


                    // Mở xong gọi reveal lại
                    if (
                        productsExpanded
                    ) {

                        setTimeout(
                            revealOnScroll,
                            30
                        );

                    }


                    // Thu gọn => trở về đầu section
                    if (
                        !productsExpanded
                    ) {

                        document
                            .querySelector(
                                ".promotion-header"
                            )
                            ?.scrollIntoView(
                                {
                                    behavior:
                                        "smooth",

                                    block:
                                        "start"
                                }
                            );

                    }

                }
            );


        // =========================================================
        // SHOW ALL COMBO
        //
        // Ban đầu: 3 combo
        // Bấm xem tất cả: hiện toàn bộ
        // =========================================================

        const showAllComboButton =
            document.getElementById(
                "showAllCombo"
            );


        const extraCombos =
            Array.from(
                document.querySelectorAll(
                    ".extra-combo"
                )
            );


        let combosExpanded =
            false;


        function updateComboVisibility() {

            extraCombos.forEach(
                item => {

                    if (
                        combosExpanded
                    ) {

                        // =====================================
                        // HIỆN COMBO
                        // =====================================

                        item
                            .classList
                            .remove(
                                "extra-combo"
                            );


                        item.style
                            .removeProperty(
                                "display"
                            );


                        item
                            .classList
                            .add(
                                "show"
                            );

                    }
                    else {

                        // =====================================
                        // ẨN COMBO
                        // =====================================

                        item
                            .classList
                            .add(
                                "extra-combo"
                            );


                        item.style
                            .setProperty(
                                "display",
                                "none",
                                "important"
                            );

                    }

                }
            );


            // =============================================
            // BUTTON TEXT
            // =============================================

            if (
                showAllComboButton
            ) {

                showAllComboButton
                    .innerHTML =

                    combosExpanded

                        ? `${rosalieText(
                            "Collapse",
                            "Thu gọn"
                        )} ▲`

                        : `${rosalieText(
                            "ViewAllCombos",
                            "Xem tất cả Combo"
                        )} ▼`;

            }

        }


        // =================================================
        // COMBO INITIAL
        // =================================================

        updateComboVisibility();


        // =================================================
        // COMBO CLICK
        // =================================================

        showAllComboButton
            ?.addEventListener(
                "click",
                function () {

                    combosExpanded =
                        !combosExpanded;


                    updateComboVisibility();


                    // Mở xong gọi reveal
                    if (
                        combosExpanded
                    ) {

                        setTimeout(
                            revealOnScroll,
                            30
                        );

                    }


                    // Thu gọn => trở lại section combo
                    if (
                        !combosExpanded
                    ) {

                        document
                            .querySelector(
                                ".combo-section"
                            )
                            ?.scrollIntoView(
                                {
                                    behavior:
                                        "smooth",

                                    block:
                                        "start"
                                }
                            );

                    }

                }
            );


        // =================================================
        // ESC CLOSE PROMOTION TOAST
        // =================================================

        document.addEventListener(
            "keydown",
            function (event) {

                if (event.key !== "Escape") {
                    return;
                }

                document
                    .getElementById("promotionToast")
                    ?.classList
                    .remove("show");
            }
        );


        // =================================================
        // IMAGE LOAD EFFECT
        // =================================================

        document
            .querySelectorAll(
                ".promotion-image img, " +
                ".combo-image"
            )
            .forEach(
                image => {

                    if (
                        image.complete
                    ) {

                        image
                            .classList
                            .add(
                                "show"
                            );

                    }


                    image.addEventListener(
                        "load",
                        function () {

                            this
                                .classList
                                .add(
                                    "show"
                                );

                        }
                    );

                }
            );


        // =================================================
        // INITIAL REVEAL
        // =================================================

        revealOnScroll();


        // =================================================
        // PAGE READY
        // =================================================

        document
            .body
            .classList
            .add(
                "promotion-loaded"
            );


        console.log(
            "%cRosalie Coffee Promotion Ready",
            "color:#6F4E37;font-size:15px;font-weight:bold;"
        );

    }
);