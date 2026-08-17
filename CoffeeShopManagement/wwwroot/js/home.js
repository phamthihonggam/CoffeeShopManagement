document.addEventListener("DOMContentLoaded", function () {

    const pages = document.querySelectorAll(".product-page");
    const dots = document.querySelectorAll(".dot");

    if (pages.length === 0) return;

    let current = 0;

    function showPage(index) {

        pages.forEach(p => p.classList.remove("active"));
        dots.forEach(d => d.classList.remove("active"));

        pages[index].classList.add("active");
        dots[index].classList.add("active");

        current = index;
    }

    document.getElementById("nextBtn")?.addEventListener("click", () => {

        showPage((current + 1) % pages.length);

    });

    document.getElementById("prevBtn")?.addEventListener("click", () => {

        showPage((current - 1 + pages.length) % pages.length);

    });

    dots.forEach((dot, index) => {

        dot.onclick = () => showPage(index);

    });

});