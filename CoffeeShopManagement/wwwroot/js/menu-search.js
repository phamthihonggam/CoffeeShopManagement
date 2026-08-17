// ======================================================
// ROSALIE COFFEE
// MENU-SEARCH.JS
//
// SEARCH RIÊNG CHO TRANG MENU
// KHÔNG DÙNG CHUNG ID VỚI HEADER
//
// - Enter tìm kiếm
// - Click kính lúp tìm kiếm
// - Xóa tìm kiếm
// - Giá từ
// - Giá đến
// - Sắp xếp
// - Reset
// ======================================================

(function () {

    "use strict";


    // ==================================================
    // INIT
    // ==================================================

    function initMenuSearch() {

        // ----------------------------------------------
        // Không cho bind event 2 lần
        // ----------------------------------------------

        if (window.rosalieMenuSearchInitialized) {
            return;
        }


        // ----------------------------------------------
        // Lấy API được public từ menu.js
        // ----------------------------------------------

        const menu = window.RosalieMenu;


        if (!menu) {

            console.warn(
                "RosalieMenu chưa sẵn sàng."
            );

            return;
        }


        window.rosalieMenuSearchInitialized = true;


        const state = menu.state;


        // ==================================================
        // ELEMENTS
        // ==================================================

        // QUAN TRỌNG:
        // Các ID này chỉ thuộc SEARCH Ở TRANG MENU.
        // Không dùng searchInput/searchBtn của Header.

        const searchInput =
            document.getElementById(
                "menuSearchInput"
            );


        const searchBtn =
            document.getElementById(
                "menuSearchBtn"
            );


        const clearSearchBtn =
            document.getElementById(
                "menuClearSearchBtn"
            );


        const minPriceInput =
            document.getElementById(
                "minPrice"
            );


        const maxPriceInput =
            document.getElementById(
                "maxPrice"
            );


        const sortSelect =
            document.getElementById(
                "sortSelect"
            );


        const resetButton =
            document.getElementById(
                "resetAdvancedBtn"
            );


        // ==================================================
        // DEBUG
        // ==================================================

        console.log(
            "Menu search initialized:",
            {
                searchInput,
                searchBtn,
                clearSearchBtn
            }
        );


        // ==================================================
        // SEARCH
        // ==================================================

        function runSearch() {

            if (!searchInput) {
                return;
            }


            // Lấy đúng dữ liệu ở thanh SEARCH DƯỚI MENU

            state.currentKeyword =
                searchInput.value.trim();


            // Khi tìm bằng chữ:
            // tìm toàn bộ sản phẩm
            // không giới hạn danh mục trước đó.

            if (state.currentKeyword !== "") {

                state.currentCategory = 0;


                if (
                    typeof menu.setActiveCategory ===
                    "function"
                ) {

                    menu.setActiveCategory(0);

                }

            }


            // Quay lại trang 1

            state.currentPage = 1;


            // Load lại sản phẩm

            menu.loadProducts(false);

        }


        // ==================================================
        // ENTER TRONG SEARCH
        // ==================================================

        if (searchInput) {

            searchInput.addEventListener(
                "keydown",
                function (event) {

                    if (event.key === "Enter") {

                        event.preventDefault();

                        runSearch();

                    }

                }
            );

        }


        // ==================================================
        // CLICK KÍNH LÚP
        // ==================================================

        if (searchBtn) {

            searchBtn.addEventListener(
                "click",
                function (event) {

                    event.preventDefault();

                    runSearch();

                }
            );

        }


        // ==================================================
        // CLEAR SEARCH
        // ==================================================

        if (clearSearchBtn) {

            clearSearchBtn.addEventListener(
                "click",
                function (event) {

                    event.preventDefault();


                    if (searchInput) {

                        searchInput.value = "";

                    }


                    state.currentKeyword = "";

                    state.currentPage = 1;


                    menu.loadProducts(false);


                    searchInput?.focus();

                }
            );

        }


        // ==================================================
        // PRICE FILTER
        // ==================================================

        function runPriceFilter() {

            state.currentMinPrice =
                minPriceInput
                    ? minPriceInput.value.trim()
                    : "";


            state.currentMaxPrice =
                maxPriceInput
                    ? maxPriceInput.value.trim()
                    : "";


            state.currentPage = 1;


            menu.loadProducts(false);

        }


        // ==================================================
        // MIN PRICE - ENTER
        // ==================================================

        if (minPriceInput) {

            minPriceInput.addEventListener(
                "keydown",
                function (event) {

                    if (event.key === "Enter") {

                        event.preventDefault();

                        runPriceFilter();

                    }

                }
            );


            minPriceInput.addEventListener(
                "change",
                runPriceFilter
            );

        }


        // ==================================================
        // MAX PRICE - ENTER
        // ==================================================

        if (maxPriceInput) {

            maxPriceInput.addEventListener(
                "keydown",
                function (event) {

                    if (event.key === "Enter") {

                        event.preventDefault();

                        runPriceFilter();

                    }

                }
            );


            maxPriceInput.addEventListener(
                "change",
                runPriceFilter
            );

        }


        // ==================================================
        // SORT
        // ==================================================

        if (sortSelect) {

            sortSelect.addEventListener(
                "change",
                function () {

                    state.currentSort =
                        this.value;


                    state.currentPage = 1;


                    menu.loadProducts(false);

                }
            );

        }


        // ==================================================
        // RESET
        // ==================================================

        if (resetButton) {

            resetButton.addEventListener(
                "click",
                function (event) {

                    event.preventDefault();


                    // -------------------------------
                    // RESET STATE
                    // -------------------------------

                    state.currentPage = 1;

                    state.currentCategory = 0;

                    state.currentKeyword = "";

                    state.currentMinPrice = "";

                    state.currentMaxPrice = "";

                    state.currentSort = "";


                    // -------------------------------
                    // RESET INPUT
                    // -------------------------------

                    if (searchInput) {

                        searchInput.value = "";

                    }


                    if (minPriceInput) {

                        minPriceInput.value = "";

                    }


                    if (maxPriceInput) {

                        maxPriceInput.value = "";

                    }


                    if (sortSelect) {

                        sortSelect.value = "";

                    }


                    // -------------------------------
                    // RESET CATEGORY
                    // -------------------------------

                    if (
                        typeof menu.setActiveCategory ===
                        "function"
                    ) {

                        menu.setActiveCategory(0);

                    }


                    // -------------------------------
                    // LOAD LẠI
                    // -------------------------------

                    menu.loadProducts(false);

                }
            );

        }

    }


    // ==================================================
    // TRƯỜNG HỢP MENU.JS ĐÃ LOAD
    // ==================================================

    if (window.RosalieMenu) {

        initMenuSearch();

    }


    // ==================================================
    // TRƯỜNG HỢP MENU.JS LOAD SAU
    // ==================================================

    document.addEventListener(
        "rosalieMenuReady",
        initMenuSearch
    );


    // ==================================================
    // DOM FALLBACK
    // ==================================================

    document.addEventListener(
        "DOMContentLoaded",
        function () {

            if (window.RosalieMenu) {

                initMenuSearch();

            }

        }
    );

})();