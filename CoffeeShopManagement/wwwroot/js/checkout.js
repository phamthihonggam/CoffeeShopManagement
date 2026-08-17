// =========================================================
// ROSALIE COFFEE
// CHECKOUT - PROVINCE / DISTRICT
// =========================================================

document.addEventListener("DOMContentLoaded", function () {

    // =====================================================
    // TRANSLATION
    // =====================================================

    const t = window.checkoutTranslations || {};


    // =====================================================
    // PROVINCE / DISTRICT DATA
    // =====================================================

    const data = {

        "TP.HCM": [
            "Quận 1",
            "Quận 3",
            "Quận 4",
            "Quận 5",
            "Quận 6",
            "Quận 7",
            "Quận 8",
            "Quận 10",
            "Quận 11",
            "Quận 12",
            "Bình Thạnh",
            "Gò Vấp",
            "Phú Nhuận",
            "Tân Bình",
            "Tân Phú",
            "Bình Tân",
            "Thủ Đức",
            "Hóc Môn",
            "Củ Chi",
            "Nhà Bè",
            "Cần Giờ"
        ],

        "Hà Nội": [
            "Ba Đình",
            "Hoàn Kiếm",
            "Đống Đa",
            "Hai Bà Trưng",
            "Thanh Xuân",
            "Cầu Giấy",
            "Long Biên",
            "Hoàng Mai",
            "Hà Đông",
            "Nam Từ Liêm",
            "Bắc Từ Liêm"
        ],

        "Đà Nẵng": [
            "Hải Châu",
            "Thanh Khê",
            "Sơn Trà",
            "Ngũ Hành Sơn",
            "Liên Chiểu",
            "Cẩm Lệ",
            "Hòa Vang"
        ],

        "Cần Thơ": [
            "Ninh Kiều",
            "Bình Thủy",
            "Cái Răng",
            "Ô Môn",
            "Thốt Nốt"
        ],

        "Bình Dương": [
            "Thủ Dầu Một",
            "Thuận An",
            "Dĩ An",
            "Bến Cát",
            "Tân Uyên"
        ]

    };


    // =====================================================
    // ELEMENTS
    // =====================================================

    const province =
        document.getElementById("province");

    const district =
        document.getElementById("district");


    if (!province || !district) {
        return;
    }


    // =====================================================
    // LOAD PROVINCES
    // =====================================================

    Object.keys(data).forEach(function (p) {

        const option =
            document.createElement("option");

        option.value = p;
        option.textContent = p;

        province.appendChild(option);

    });


    // =====================================================
    // PROVINCE CHANGE
    // =====================================================

    province.addEventListener(
        "change",
        function () {

            // =============================================
            // RESET DISTRICT
            // =============================================

            district.innerHTML = "";


            const defaultOption =
                document.createElement("option");

            defaultOption.value = "";

            defaultOption.textContent =
                t.selectDistrict ||
                "-- Chọn quận / huyện --";

            district.appendChild(defaultOption);


            // =============================================
            // NO PROVINCE
            // =============================================

            if (!data[this.value]) {
                return;
            }


            // =============================================
            // LOAD DISTRICTS
            // =============================================

            data[this.value].forEach(
                function (d) {

                    const option =
                        document.createElement("option");

                    option.value = d;
                    option.textContent = d;

                    district.appendChild(option);

                }
            );

        }
    );

});