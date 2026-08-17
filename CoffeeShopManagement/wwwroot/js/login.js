// ===========================
// LOGIN PAGE
// ===========================

document.addEventListener("DOMContentLoaded", function () {

    // ===========================
    // TRANSLATION
    // ===========================

    const t = window.loginTranslations || {};


    // ===========================
    // HIỆN / ẨN MẬT KHẨU
    // ===========================

    const password =
        document.getElementById("password");

    const togglePassword =
        document.getElementById("togglePassword");


    if (password && togglePassword) {

        togglePassword.addEventListener(
            "click",
            function () {

                if (password.type === "password") {

                    password.type = "text";

                    this.innerHTML =
                        '<i class="fa-solid fa-eye-slash"></i>';

                }
                else {

                    password.type = "password";

                    this.innerHTML =
                        '<i class="fa-solid fa-eye"></i>';

                }

            }
        );

    }


    // ===========================
    // LOADING BUTTON
    // ===========================

    const loginForm =
        document.querySelector(
            'form[action*="Login"]'
        );

    const loginBtn =
        document.getElementById("loginBtn");

    const btnText =
        document.getElementById("btnText");


    if (
        loginForm &&
        loginBtn &&
        btnText
    ) {

        loginForm.addEventListener(
            "submit",
            function () {

                if (
                    typeof $ !== "undefined" &&
                    !$(this).valid()
                ) {
                    return;
                }


                loginBtn.disabled = true;


                btnText.innerHTML = `

                    <i class="fa-solid fa-spinner fa-spin"></i>

                    ${t.loggingIn || "Đang đăng nhập..."}

                `;

            }
        );

    }


    // ===========================
    // ENTER ĐĂNG NHẬP
    // ===========================

    document.addEventListener(
        "keypress",
        function (e) {

            if (
                e.key === "Enter" &&
                loginForm
            ) {

                loginForm.requestSubmit();

            }

        }
    );


    // ===========================
    // AUTO FOCUS EMAIL
    // ===========================

    const email =
        document.getElementById("Email");


    if (
        email &&
        email.value === ""
    ) {

        email.focus();

    }


    // ===========================
    // XÓA LỖI KHI NHẬP LẠI
    // ===========================

    document
        .querySelectorAll(".form-control")
        .forEach(
            function (input) {

                input.addEventListener(
                    "input",
                    function () {

                        this.classList.remove(
                            "input-validation-error"
                        );

                    }
                );

            }
        );

});