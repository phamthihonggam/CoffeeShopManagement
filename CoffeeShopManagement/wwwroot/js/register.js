// ===========================================
// ROSALIE COFFEE
// REGISTER PAGE
// ===========================================

document.addEventListener("DOMContentLoaded", function () {

    // ==========================================
    // TRANSLATION
    // ==========================================

    const t =
        window.registerTranslations || {};


    // ==========================================
    // ELEMENTS
    // ==========================================

    const password =
        document.getElementById("password");

    const confirmPassword =
        document.getElementById("confirmPassword");


    const togglePassword =
        document.getElementById("togglePassword");

    const toggleConfirmPassword =
        document.getElementById(
            "toggleConfirmPassword"
        );


    const strengthBar =
        document.getElementById("strengthBar");

    const strengthText =
        document.getElementById("strengthText");


    const registerBtn =
        document.getElementById("registerBtn");

    const btnText =
        document.getElementById("btnText");


    const form =
        document.querySelector(
            'form[action*="Register"]'
        );


    // ==========================================
    // SHOW / HIDE PASSWORD
    // ==========================================

    function toggle(input, button) {

        if (!input || !button) {
            return;
        }


        button.addEventListener(
            "click",
            function () {

                if (input.type === "password") {

                    input.type =
                        "text";

                    this.innerHTML =
                        '<i class="fa-solid fa-eye-slash"></i>';

                }
                else {

                    input.type =
                        "password";

                    this.innerHTML =
                        '<i class="fa-solid fa-eye"></i>';

                }

            }
        );

    }


    toggle(
        password,
        togglePassword
    );


    toggle(
        confirmPassword,
        toggleConfirmPassword
    );


    // ==========================================
    // PASSWORD RULE
    // ==========================================

    function updateRule(id, valid) {

        const item =
            document.getElementById(id);


        if (!item) {
            return;
        }


        const icon =
            item.querySelector("i");


        if (valid) {

            item.classList.remove(
                "invalid"
            );

            item.classList.add(
                "valid"
            );


            if (icon) {

                icon.className =
                    "fa-solid fa-circle-check";
            }

        }
        else {

            item.classList.remove(
                "valid"
            );

            item.classList.add(
                "invalid"
            );


            if (icon) {

                icon.className =
                    "fa-solid fa-circle-xmark";
            }

        }

    }


    // ==========================================
    // PASSWORD STRENGTH
    // ==========================================

    if (
        password &&
        strengthBar &&
        strengthText
    ) {

        password.addEventListener(
            "input",
            function () {

                const value =
                    this.value;


                let score = 0;


                const hasLength =
                    value.length >= 8;

                const hasUpper =
                    /[A-Z]/.test(value);

                const hasLower =
                    /[a-z]/.test(value);

                const hasNumber =
                    /\d/.test(value);

                const hasSpecial =
                    /[@$!%*?&#._\-]/.test(value);


                updateRule(
                    "rule-length",
                    hasLength
                );

                updateRule(
                    "rule-upper",
                    hasUpper
                );

                updateRule(
                    "rule-lower",
                    hasLower
                );

                updateRule(
                    "rule-number",
                    hasNumber
                );

                updateRule(
                    "rule-special",
                    hasSpecial
                );


                if (hasLength) {
                    score++;
                }

                if (hasUpper) {
                    score++;
                }

                if (hasLower) {
                    score++;
                }

                if (hasNumber) {
                    score++;
                }

                if (hasSpecial) {
                    score++;
                }


                switch (score) {

                    case 0:
                    case 1:

                        strengthBar.style.width =
                            "20%";

                        strengthBar.style.background =
                            "#dc3545";

                        strengthText.textContent =
                            t.passwordVeryWeak ||
                            "Mật khẩu rất yếu";

                        break;


                    case 2:

                        strengthBar.style.width =
                            "40%";

                        strengthBar.style.background =
                            "#fd7e14";

                        strengthText.textContent =
                            t.passwordWeak ||
                            "Mật khẩu yếu";

                        break;


                    case 3:

                        strengthBar.style.width =
                            "60%";

                        strengthBar.style.background =
                            "#ffc107";

                        strengthText.textContent =
                            t.passwordMedium ||
                            "Mật khẩu trung bình";

                        break;


                    case 4:

                        strengthBar.style.width =
                            "80%";

                        strengthBar.style.background =
                            "#20c997";

                        strengthText.textContent =
                            t.passwordQuiteStrong ||
                            "Mật khẩu khá mạnh";

                        break;


                    case 5:

                        strengthBar.style.width =
                            "100%";

                        strengthBar.style.background =
                            "#198754";

                        strengthText.textContent =
                            t.passwordStrong ||
                            "Mật khẩu mạnh";

                        break;

                }

            }
        );

    }


    // ==========================================
    // LOADING BUTTON
    // ==========================================

    if (
        form &&
        registerBtn &&
        btnText
    ) {

        form.addEventListener(
            "submit",
            function () {

                if (
                    typeof $ !== "undefined" &&
                    !$(this).valid()
                ) {

                    return;
                }


                registerBtn.disabled =
                    true;


                btnText.innerHTML = `

                    <i class="fa-solid fa-spinner fa-spin"></i>

                    ${t.creatingAccount ||
                    "Đang tạo tài khoản..."}

                `;

            }
        );

    }


    // ==========================================
    // AUTO FOCUS
    // ==========================================

    const nameInput =
        document.getElementById("HoTen");


    if (nameInput) {

        nameInput.focus();

    }

});