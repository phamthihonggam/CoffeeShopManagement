document.addEventListener("DOMContentLoaded", function () {

    const hero = document.querySelector(".hero-section");
    const dotsContainer = document.querySelector(".hero-dots");

    if (!hero || !dotsContainer) return;

    const images = [
        "/images/home/banner1.jpg",
        "/images/home/banner2.jpg",
        "/images/home/banner3.jpg",
        "/images/home/banner4.jpg",
        "/images/home/banner5.jpg"
    ];

    let current = 0;
    let timer;

    // Tạo chấm tròn
    images.forEach((image, index) => {

        const dot = document.createElement("span");
        dot.classList.add("hero-dot");

        dot.addEventListener("click", function () {

            current = index;

            showSlide();

            restartSlider();

        });

        dotsContainer.appendChild(dot);

    });

    const dots = document.querySelectorAll(".hero-dot");

    function showSlide() {

        hero.style.backgroundImage = `
            linear-gradient(rgba(0,0,0,.45), rgba(0,0,0,.45)),
            url('${images[current]}')
        `;

        dots.forEach(dot => dot.classList.remove("active"));

        dots[current].classList.add("active");

    }

    function nextSlide() {

        current++;

        if (current >= images.length) {
            current = 0;
        }

        showSlide();

    }

    function restartSlider() {

        clearInterval(timer);

        timer = setInterval(nextSlide, 5000);

    }

    showSlide();

    restartSlider();

});