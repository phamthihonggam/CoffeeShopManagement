// ==========================
// LANGUAGE DROPDOWN
// ==========================

document.addEventListener("DOMContentLoaded", () => {

    const langBtn = document.getElementById("langBtn");
    const langMenu = document.getElementById("langMenu");
    const arrow = document.querySelector(".arrow");

    if (!langBtn || !langMenu) return;

    function closeMenu() {
        langMenu.classList.remove("show");

        if (arrow) {
            arrow.style.transform = "rotate(0deg)";
        }
    }

    langBtn.addEventListener("click", function (e) {

        e.stopPropagation();

        const isOpen = langMenu.classList.toggle("show");

        if (arrow) {
            arrow.style.transform = isOpen
                ? "rotate(180deg)"
                : "rotate(0deg)";
        }

    });

    langMenu.addEventListener("click", function (e) {
        e.stopPropagation();
    });

    document.addEventListener("click", function () {
        closeMenu();
    });

});