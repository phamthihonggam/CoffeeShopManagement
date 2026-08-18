/* =========================================================
   ROSALIE COFFEE
   STAFF - CUSTOMER
   File: wwwroot/js/staff-customer.js
========================================================= */

document.addEventListener("DOMContentLoaded", function () {

    const pageSizeSelect =
        document.querySelector(
            ".js-customer-page-size"
        );


    /* =====================================================
       PAGE SIZE
       Đổi 10 / 20 / 50 dòng thì submit form luôn
    ===================================================== */

    pageSizeSelect?.addEventListener(
        "change",
        function () {

            this.form?.submit();

        }
    );

});