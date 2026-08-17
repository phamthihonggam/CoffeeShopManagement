// ======================================================
// ROSALIE COFFEE
// MENU.JS
//
// CORE:
// - CATEGORY
// - SEARCH STATE
// - PRICE FILTER
// - SORT
// - PAGINATION
// - LOAD MORE
// ======================================================

document.addEventListener("DOMContentLoaded", function () {

    "use strict";


    // ==================================================
    // ELEMENTS
    // ==================================================

    const menuGrid =
        document.getElementById("menuGrid");

    if (!menuGrid) {
        return;
    }


    const pagination =
        document.getElementById("pagination");

    const loadMoreContainer =
        document.getElementById("loadMoreContainer");

    const loadMoreBtn =
        document.getElementById("loadMoreBtn");

    const switchTrack =
        document.querySelector(".switch-track");

    const paginationText =
        document.getElementById("paginationText");

    const loadMoreText =
        document.getElementById("loadMoreText");

    const menuSearchInput =
        document.getElementById("menuSearchInput");


    // ==================================================
    // CATEGORY MAP
    // ==================================================

    const categoryMap = {

        all: 0,

        coffee: 1,

        tea: 2,

        matcha: 3,

        soda: 4,

        cake: 5,

        juice: 6,

        yogurt: 7,

        blended: 8

    };


    const reverseCategoryMap = {

        0: "all",

        1: "coffee",

        2: "tea",

        3: "matcha",

        4: "soda",

        5: "cake",

        6: "juice",

        7: "yogurt",

        8: "blended"

    };


    // ==================================================
    // URL PARAMS
    // ==================================================

    const urlParams =
        new URLSearchParams(
            window.location.search
        );


    // ==================================================
    // INITIAL CATEGORY
    // ==================================================

    let initialCategory = 0;


    const categoryFromUrl =
        Number(
            urlParams.get("category")
        );


    if (
        categoryFromUrl >= 1
        &&
        categoryFromUrl <= 8
    ) {

        initialCategory =
            categoryFromUrl;

    }
    else {

        const defaultFilter =
            window.defaultFilter;


        if (
            defaultFilter
            &&
            categoryMap[defaultFilter] !== undefined
        ) {

            initialCategory =
                categoryMap[defaultFilter];

        }

    }


    // ==================================================
    // INITIAL KEYWORD
    // ==================================================

    let initialKeyword = "";


    const keywordFromUrl =
        urlParams.get("keyword");


    if (
        keywordFromUrl
        &&
        keywordFromUrl.trim() !== ""
    ) {

        initialKeyword =
            keywordFromUrl.trim();

    }
    else if (
        typeof window.defaultKeyword === "string"
    ) {

        initialKeyword =
            window.defaultKeyword.trim();

    }


    /*
        Khi tìm bằng keyword:
        ưu tiên tìm toàn bộ sản phẩm,
        không khóa trong category.
    */

    if (initialKeyword !== "") {

        initialCategory = 0;

    }


    // ==================================================
    // SHARED STATE
    // ==================================================

    const state = {

        currentPage: 1,

        currentCategory:
            initialCategory,

        currentKeyword:
            initialKeyword,

        currentMinPrice: "",

        currentMaxPrice: "",

        currentSort: "",

        viewMode: "pagination",

        pageSize: 12

    };


    let productAbortController = null;

    let paginationAbortController = null;


    // ==================================================
    // SYNC SEARCH INPUT
    // ==================================================

    if (menuSearchInput) {

        menuSearchInput.value =
            state.currentKeyword;

    }


    // ==================================================
    // ACTIVE CATEGORY
    // ==================================================

    function setActiveCategory(category) {

        const name =
            reverseCategoryMap[category]
            ||
            "all";


        document
            .querySelectorAll(
                ".menu-filter button"
            )
            .forEach(button => {

                button.classList.toggle(
                    "active",
                    button.dataset.filter === name
                );

            });

    }


    // ==================================================
    // BUILD PRODUCT QUERY
    // ==================================================

    function buildProductQuery() {

        const params =
            new URLSearchParams();


        params.set(
            "page",
            state.currentPage
        );


        params.set(
            "category",
            state.currentCategory
        );


        if (
            state.currentKeyword !== ""
        ) {

            params.set(
                "keyword",
                state.currentKeyword
            );

        }


        if (
            state.currentMinPrice !== ""
        ) {

            params.set(
                "minPrice",
                state.currentMinPrice
            );

        }


        if (
            state.currentMaxPrice !== ""
        ) {

            params.set(
                "maxPrice",
                state.currentMaxPrice
            );

        }


        if (
            state.currentSort !== ""
        ) {

            params.set(
                "sort",
                state.currentSort
            );

        }


        params.set(
            "pageSize",
            state.pageSize
        );


        return params.toString();

    }


    // ==================================================
    // BUILD PAGINATION QUERY
    // ==================================================

    function buildPaginationQuery() {

        const params =
            new URLSearchParams();


        params.set(
            "category",
            state.currentCategory
        );


        if (
            state.currentKeyword !== ""
        ) {

            params.set(
                "keyword",
                state.currentKeyword
            );

        }


        if (
            state.currentMinPrice !== ""
        ) {

            params.set(
                "minPrice",
                state.currentMinPrice
            );

        }


        if (
            state.currentMaxPrice !== ""
        ) {

            params.set(
                "maxPrice",
                state.currentMaxPrice
            );

        }


        params.set(
            "pageSize",
            state.pageSize
        );


        return params.toString();

    }


    // ==================================================
    // EMPTY RESULT
    // ==================================================

    function showEmptyResult() {

        const keyword =
            state.currentKeyword;


        let message = `
            Hãy thử từ khóa,
            danh mục hoặc khoảng giá khác.
        `;


        if (keyword !== "") {

            message = `
                Không có sản phẩm nào phù hợp với
                "<strong>${escapeHtml(keyword)}</strong>".
            `;

        }


        menuGrid.innerHTML = `

            <div class="menu-no-result">

                <div class="menu-no-result-icon">

                    <i class="fa-solid fa-mug-hot"></i>

                </div>

                <h3>
                    Không tìm thấy món phù hợp
                </h3>

                <p>
                    ${message}
                </p>

                <button type="button"
                        class="menu-no-result-reset"
                        id="emptyResultReset">

                    <i class="fa-solid fa-rotate-left"></i>

                    Xem tất cả sản phẩm

                </button>

            </div>
        `;


        const resetBtn =
            document.getElementById(
                "emptyResultReset"
            );


        resetBtn?.addEventListener(
            "click",
            function () {

                const advancedReset =
                    document.getElementById(
                        "resetAdvancedBtn"
                    );


                if (advancedReset) {

                    advancedReset.click();

                    return;

                }


                state.currentPage = 1;

                state.currentCategory = 0;

                state.currentKeyword = "";

                state.currentMinPrice = "";

                state.currentMaxPrice = "";

                state.currentSort = "";


                if (menuSearchInput) {

                    menuSearchInput.value = "";

                }


                setActiveCategory(0);

                loadProducts(false);

            }
        );

    }


    // ==================================================
    // ESCAPE HTML
    // ==================================================

    function escapeHtml(value) {

        return String(value)
            .replaceAll("&", "&amp;")
            .replaceAll("<", "&lt;")
            .replaceAll(">", "&gt;")
            .replaceAll('"', "&quot;")
            .replaceAll("'", "&#039;");

    }


    // ==================================================
    // LOAD PRODUCTS
    // ==================================================

    async function loadProducts(
        append = false
    ) {

        if (productAbortController) {

            productAbortController.abort();

        }


        productAbortController =
            new AbortController();


        const url =
            "/Menu/GetProducts?"
            +
            buildProductQuery();


        try {

            const response =
                await fetch(
                    url,
                    {
                        cache: "no-store",

                        signal:
                            productAbortController.signal
                    }
                );


            if (!response.ok) {

                throw new Error(
                    "Không thể tải sản phẩm."
                );

            }


            const html =
                await response.text();


            // ==========================================
            // LOAD MORE
            // ==========================================

            if (
                append
                &&
                state.viewMode === "loadmore"
            ) {

                if (
                    html.trim() !== ""
                ) {

                    menuGrid.insertAdjacentHTML(
                        "beforeend",
                        html
                    );

                }

            }


            // ==========================================
            // NORMAL LOAD
            // ==========================================

            else {

                if (
                    html.trim() === ""
                ) {

                    showEmptyResult();

                }
                else {

                    menuGrid.innerHTML =
                        html;

                }

            }


            initFavorite();


            await loadPagination();

        }
        catch (error) {

            if (
                error.name ===
                "AbortError"
            ) {

                return;

            }


            console.error(
                "LOAD PRODUCTS:",
                error
            );

        }

    }


    // ==================================================
    // LOAD PAGINATION
    // ==================================================

    async function loadPagination() {

        if (!pagination) {
            return;
        }


        if (
            paginationAbortController
        ) {

            paginationAbortController.abort();

        }


        paginationAbortController =
            new AbortController();


        try {

            const response =
                await fetch(
                    "/Menu/GetTotalPages?"
                    +
                    buildPaginationQuery(),
                    {
                        cache: "no-store",

                        signal:
                            paginationAbortController.signal
                    }
                );


            if (!response.ok) {

                throw new Error(
                    "Không thể tải phân trang."
                );

            }


            const totalPages =
                await response.json();


            pagination.innerHTML = "";


            // ==========================================
            // LOAD MORE MODE
            // ==========================================

            if (
                state.viewMode ===
                "loadmore"
            ) {

                pagination.style.display =
                    "none";


                if (loadMoreContainer) {

                    loadMoreContainer.style.display =
                        state.currentPage < totalPages
                            ? "flex"
                            : "none";

                }


                return;

            }


            // ==========================================
            // PAGINATION MODE
            // ==========================================

            if (loadMoreContainer) {

                loadMoreContainer.style.display =
                    "none";

            }


            if (
                totalPages <= 1
            ) {

                pagination.style.display =
                    "none";

                return;

            }


            pagination.style.display =
                "flex";


            for (
                let page = 1;
                page <= totalPages;
                page++
            ) {

                const button =
                    document.createElement(
                        "button"
                    );


                button.type =
                    "button";


                button.className =
                    "page-btn"
                    +
                    (
                        page === state.currentPage
                            ? " active"
                            : ""
                    );


                button.textContent =
                    page;


                button.addEventListener(
                    "click",
                    function () {

                        state.currentPage =
                            page;


                        loadProducts(false);


                        document
                            .querySelector(
                                ".menu-toolbar"
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
                );


                pagination.appendChild(
                    button
                );

            }

        }
        catch (error) {

            if (
                error.name ===
                "AbortError"
            ) {

                return;

            }


            console.error(
                "PAGINATION:",
                error
            );

        }

    }


    // ==================================================
    // CATEGORY FILTER
    // ==================================================

    document
        .querySelectorAll(
            ".menu-filter button"
        )
        .forEach(button => {

            button.addEventListener(
                "click",
                function () {

                    state.currentCategory =
                        categoryMap[
                        this.dataset.filter
                        ]
                        ??
                        0;


                    // ==========================================
                    // BẤM CATEGORY THÌ XÓA SEARCH
                    // ==========================================

                    state.currentKeyword =
                        "";


                    /*
                        QUAN TRỌNG:
                        ID đúng là menuSearchInput,
                        không phải searchInput.
                    */

                    if (menuSearchInput) {

                        menuSearchInput.value =
                            "";

                    }


                    setActiveCategory(
                        state.currentCategory
                    );


                    state.currentPage =
                        1;


                    loadProducts(false);

                }
            );

        });


    // ==================================================
    // LOAD MORE
    // ==================================================

    loadMoreBtn
        ?.addEventListener(
            "click",
            function () {

                state.currentPage++;


                loadProducts(true);

            }
        );


    // ==================================================
    // VIEW MODE
    // ==================================================

    switchTrack
        ?.addEventListener(
            "click",
            function () {

                if (
                    state.viewMode ===
                    "pagination"
                ) {

                    state.viewMode =
                        "loadmore";


                    switchTrack.classList.add(
                        "active"
                    );


                    paginationText
                        ?.classList
                        .remove(
                            "active"
                        );


                    loadMoreText
                        ?.classList
                        .add(
                            "active"
                        );

                }
                else {

                    state.viewMode =
                        "pagination";


                    switchTrack.classList.remove(
                        "active"
                    );


                    paginationText
                        ?.classList
                        .add(
                            "active"
                        );


                    loadMoreText
                        ?.classList
                        .remove(
                            "active"
                        );

                }


                state.currentPage =
                    1;


                menuGrid.innerHTML =
                    "";


                loadProducts(false);

            }
        );


    // ==================================================
    // FAVORITE
    // ==================================================

    function initFavorite() {

        document
            .querySelectorAll(
                ".favorite-btn"
            )
            .forEach(button => {

                if (
                    button.dataset.favoriteInit ===
                    "1"
                ) {

                    return;

                }


                button.dataset.favoriteInit =
                    "1";


                button.addEventListener(
                    "click",
                    function (event) {

                        event.preventDefault();

                        event.stopPropagation();


                        this.classList.toggle(
                            "active"
                        );


                        const icon =
                            this.querySelector("i");


                        if (!icon) {
                            return;
                        }


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

                    }
                );

            });

    }


    // ==================================================
    // INITIAL ACTIVE CATEGORY
    // ==================================================

    setActiveCategory(
        state.currentCategory
    );


    // ==================================================
    // SHARE API CHO MENU-SEARCH.JS
    // ==================================================

    window.RosalieMenu = {

        state,

        loadProducts,

        setActiveCategory

    };


    // ==================================================
    // BÁO SEARCH FILE ĐÃ SẴN SÀNG
    // ==================================================

    document.dispatchEvent(
        new CustomEvent(
            "rosalieMenuReady"
        )
    );


    // ==================================================
    // FIRST LOAD
    // ==================================================

    initFavorite();


    /*
        Lần load AJAX đầu tiên bây giờ đã có:
        - category từ URL
        - keyword từ URL
        nên không làm mất kết quả tìm kiếm.
    */

    loadProducts(false);

});