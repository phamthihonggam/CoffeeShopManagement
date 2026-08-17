// ======================================================
// ROSALIE COFFEE
// CONTACT / BRANCH
// ======================================================

document.addEventListener("DOMContentLoaded", function () {

    // ==================================================
    // ELEMENTS
    // ==================================================

    const modal =
        document.getElementById("branchModal");

    const contactButton =
        document.getElementById("btnContactBranch");

    const searchInput =
        document.getElementById("searchBranch");

    const branchCards =
        document.querySelectorAll(".branch-page-card");


    // ==================================================
    // CONTACT BUTTON
    // CUỘN XUỐNG DANH SÁCH CHI NHÁNH
    // ==================================================

    if (contactButton) {

        contactButton.addEventListener(
            "click",
            function (event) {

                event.preventDefault();

                const branchList =
                    document.getElementById("branch-list");

                if (!branchList) {
                    return;
                }

                branchList.scrollIntoView({
                    behavior: "smooth",
                    block: "start"
                });

            }
        );

    }


    // ==================================================
    // SEARCH BRANCH
    // ==================================================

    if (
        searchInput &&
        branchCards.length > 0
    ) {

        searchInput.addEventListener(
            "input",
            function () {

                const keyword =
                    this.value
                        .toLowerCase()
                        .trim();


                branchCards.forEach(
                    function (card) {

                        const cardText =
                            card.textContent
                                .toLowerCase()
                                .trim();


                        if (
                            cardText.includes(keyword)
                        ) {

                            card.style.display = "";

                        }
                        else {

                            card.style.display = "none";

                        }

                    }
                );

            }
        );

    }


    // ==================================================
    // MODAL
    // ==================================================

    if (!modal) {
        return;
    }


    const overlay =
        modal.querySelector(
            ".branch-page-modal-overlay"
        );

    const closeBtn =
        modal.querySelector(
            ".branch-page-close"
        );

    const detailButtons =
        document.querySelectorAll(
            ".branch-btn-detail"
        );


    // ==================================================
    // MODAL ELEMENTS
    // ==================================================

    const modalImage =
        document.getElementById("modalImage");

    const modalName =
        document.getElementById("modalName");

    const modalDescription =
        document.getElementById("modalDescription");

    const modalAddress =
        document.getElementById("modalAddress");

    const modalPhone =
        document.getElementById("modalPhone");

    const modalTime =
        document.getElementById("modalTime");

    const modalCity =
        document.getElementById("modalCity");

    const modalRating =
        document.getElementById("modalRating");

    const modalMap =
        document.getElementById("modalMap");

    const modalFeature =
        document.getElementById("modalFeature");


    // ==================================================
    // OPEN MODAL
    // ==================================================

    detailButtons.forEach(
        function (button) {

            button.addEventListener(
                "click",
                function () {

                    const card =
                        this.closest(
                            ".branch-page-card"
                        );

                    if (!card) {
                        return;
                    }


                    // ==============================
                    // IMAGE
                    // ==============================

                    const image =
                        card.querySelector(
                            ".branch-page-image img"
                        );

                    if (
                        image &&
                        modalImage
                    ) {

                        modalImage.src =
                            image.src;

                        modalImage.alt =
                            image.alt || "Rosalie Coffee";
                    }


                    // ==============================
                    // NAME
                    // ==============================

                    const name =
                        card.querySelector(
                            ".branch-page-body h2"
                        );

                    if (
                        name &&
                        modalName
                    ) {

                        modalName.textContent =
                            name.textContent.trim();
                    }


                    // ==============================
                    // DESCRIPTION
                    // ==============================

                    const description =
                        card.querySelector(
                            ".branch-page-description"
                        );

                    if (
                        description &&
                        modalDescription
                    ) {

                        modalDescription.textContent =
                            description.textContent.trim();
                    }


                    // ==============================
                    // ADDRESS
                    // ==============================

                    const address =
                        card.querySelector(
                            ".branch-page-address"
                        );

                    if (
                        address &&
                        modalAddress
                    ) {

                        modalAddress.textContent =
                            address.textContent.trim();
                    }


                    // ==============================
                    // PHONE / TIME / CITY
                    // ==============================

                    const paragraphs =
                        card.querySelectorAll(
                            ".branch-page-body > p"
                        );


                    if (
                        paragraphs.length > 1 &&
                        modalPhone
                    ) {

                        modalPhone.textContent =
                            paragraphs[1]
                                .textContent
                                .trim();
                    }


                    if (
                        paragraphs.length > 2 &&
                        modalTime
                    ) {

                        modalTime.textContent =
                            paragraphs[2]
                                .textContent
                                .trim();
                    }


                    if (
                        paragraphs.length > 3 &&
                        modalCity
                    ) {

                        modalCity.textContent =
                            paragraphs[3]
                                .textContent
                                .trim();
                    }


                    // ==============================
                    // RATING
                    // ==============================

                    const rating =
                        card.querySelector(
                            ".branch-page-rating"
                        );


                    if (modalRating) {

                        if (rating) {

                            modalRating.innerHTML = `
                                <i class="fa-solid fa-star"></i>
                                ${rating.textContent.trim()}
                            `;

                            modalRating.style.display = "";

                        }
                        else {

                            modalRating.innerHTML = "";
                            modalRating.style.display = "none";

                        }

                    }


                    // ==============================
                    // GOOGLE MAP
                    // ==============================

                    const mapButton =
                        card.querySelector(
                            ".branch-btn-map"
                        );

                    if (
                        mapButton &&
                        modalMap
                    ) {

                        modalMap.href =
                            mapButton.href;
                    }


                    // ==============================
                    // FEATURES
                    // Wifi / Parking / Air / Outlet
                    //
                    // Nội dung được copy từ card,
                    // nên sẽ tự đổi VN / US theo Razor.
                    // ==============================

                    const features =
                        card.querySelectorAll(
                            ".branch-page-feature span"
                        );


                    if (modalFeature) {

                        modalFeature.innerHTML = "";

                        features.forEach(
                            function (feature) {

                                modalFeature.insertAdjacentHTML(
                                    "beforeend",
                                    feature.outerHTML
                                );

                            }
                        );

                    }


                    // ==============================
                    // SHOW MODAL
                    // ==============================

                    modal.classList.add(
                        "active"
                    );

                    document.body.style.overflow =
                        "hidden";

                }
            );

        }
    );


    // ==================================================
    // CLOSE MODAL
    // ==================================================

    function closeModal() {

        modal.classList.remove(
            "active"
        );

        document.body.style.overflow =
            "";

    }


    // CLOSE BUTTON
    closeBtn?.addEventListener(
        "click",
        closeModal
    );


    // CLICK OVERLAY
    overlay?.addEventListener(
        "click",
        closeModal
    );


    // ESC
    document.addEventListener(
        "keydown",
        function (event) {

            if (
                event.key === "Escape" &&
                modal.classList.contains("active")
            ) {

                closeModal();

            }

        }
    );

});