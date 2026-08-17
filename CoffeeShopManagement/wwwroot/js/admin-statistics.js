/* =====================================================
   ROSALIE COFFEE
   ADMIN STATISTICS
===================================================== */

document.addEventListener(
    "DOMContentLoaded",
    function () {

        const dataElement =
            document.getElementById(
                "statisticsData"
            );


        if (
            !dataElement
            ||
            typeof Chart === "undefined"
        ) {
            return;
        }


        let statisticsData;


        try {

            statisticsData =
                JSON.parse(
                    dataElement.textContent
                );

        }
        catch (error) {

            console.error(
                "Statistics JSON error:",
                error
            );

            return;
        }


        /* =================================================
           DATA
        ================================================= */

        const revenueLabels =
            statisticsData.revenueLabels || [];


        const revenueValues =
            statisticsData.revenueValues || [];


        const statusLabels =
            statisticsData.statusLabels || [];


        const statusValues =
            statisticsData.statusValues || [];


        /* =================================================
           CHART DEFAULT
        ================================================= */

        Chart.defaults.font.family =
            "'Segoe UI', Arial, sans-serif";


        Chart.defaults.color =
            "#74645c";


        /* =================================================
           FORMAT MONEY
        ================================================= */

        function formatMoney(value) {

            return (
                Number(value || 0)
                    .toLocaleString("vi-VN")
                +
                "đ"
            );

        }


        /* =================================================
           REVENUE
        ================================================= */

        const revenueCanvas =
            document.getElementById(
                "revenueChart"
            );


        if (revenueCanvas) {

            new Chart(
                revenueCanvas,
                {
                    type: "line",


                    data: {

                        labels:
                            revenueLabels,


                        datasets: [
                            {
                                label:
                                    "Doanh thu",

                                data:
                                    revenueValues,

                                borderColor:
                                    "#875238",

                                backgroundColor:
                                    "rgba(135, 82, 56, 0.10)",

                                borderWidth:
                                    3,

                                fill:
                                    true,

                                tension:
                                    0.35,

                                pointRadius:
                                    4,

                                pointHoverRadius:
                                    6,

                                pointBackgroundColor:
                                    "#875238",

                                pointBorderColor:
                                    "#ffffff",

                                pointBorderWidth:
                                    2
                            }
                        ]

                    },


                    options: {

                        responsive:
                            true,

                        maintainAspectRatio:
                            false,


                        interaction: {

                            mode:
                                "index",

                            intersect:
                                false

                        },


                        plugins: {

                            legend: {

                                display:
                                    false

                            },


                            tooltip: {

                                displayColors:
                                    false,


                                callbacks: {

                                    label:
                                        function (context) {

                                            return (
                                                "Doanh thu: "
                                                +
                                                formatMoney(
                                                    context.raw
                                                )
                                            );

                                        }

                                }

                            }

                        },


                        scales: {

                            x: {

                                border: {

                                    display:
                                        false

                                },


                                grid: {

                                    display:
                                        false

                                },


                                ticks: {

                                    color:
                                        "#8f7d74",

                                    autoSkip:
                                        true,

                                    maxTicksLimit:
                                        16,

                                    maxRotation:
                                        0

                                }

                            },


                            y: {

                                beginAtZero:
                                    true,


                                border: {

                                    display:
                                        false

                                },


                                grid: {

                                    color:
                                        "rgba(112, 91, 80, 0.08)"

                                },


                                ticks: {

                                    color:
                                        "#8f7d74",


                                    callback:
                                        function (value) {

                                            const number =
                                                Number(value);


                                            if (number >= 1000000000) {

                                                return (
                                                    (
                                                        number /
                                                        1000000000
                                                    )
                                                        .toLocaleString(
                                                            "vi-VN",
                                                            {
                                                                maximumFractionDigits: 1
                                                            }
                                                        )
                                                    +
                                                    " tỷ"
                                                );

                                            }


                                            if (number >= 1000000) {

                                                return (
                                                    (
                                                        number /
                                                        1000000
                                                    )
                                                        .toLocaleString(
                                                            "vi-VN",
                                                            {
                                                                maximumFractionDigits: 1
                                                            }
                                                        )
                                                    +
                                                    "tr"
                                                );

                                            }


                                            if (number >= 1000) {

                                                return (
                                                    (
                                                        number /
                                                        1000
                                                    )
                                                        .toLocaleString(
                                                            "vi-VN",
                                                            {
                                                                maximumFractionDigits: 0
                                                            }
                                                        )
                                                    +
                                                    "k"
                                                );

                                            }


                                            return number;

                                        }

                                }

                            }

                        }

                    }

                }
            );

        }


        /* =================================================
           STATUS
        ================================================= */

        const statusCanvas =
            document.getElementById(
                "statusChart"
            );


        if (statusCanvas) {

            new Chart(
                statusCanvas,
                {
                    type:
                        "doughnut",


                    data: {

                        labels:
                            statusLabels,


                        datasets: [
                            {
                                data:
                                    statusValues,

                                backgroundColor: [
                                    "#d8a84d",
                                    "#8a77b1",
                                    "#5c8eb5",
                                    "#5b9b6c",
                                    "#c85c5c"
                                ],

                                borderColor:
                                    "#ffffff",

                                borderWidth:
                                    4,

                                hoverOffset:
                                    5
                            }
                        ]

                    },


                    options: {

                        responsive:
                            true,

                        maintainAspectRatio:
                            false,

                        cutout:
                            "66%",


                        plugins: {

                            legend: {

                                position:
                                    "bottom",


                                labels: {

                                    usePointStyle:
                                        true,

                                    pointStyle:
                                        "circle",

                                    padding:
                                        17,

                                    boxWidth:
                                        8,

                                    boxHeight:
                                        8,

                                    color:
                                        "#74645c",

                                    font: {

                                        size:
                                            11,

                                        weight:
                                            "600"

                                    }

                                }

                            },


                            tooltip: {

                                callbacks: {

                                    label:
                                        function (context) {

                                            const value =
                                                Number(
                                                    context.raw || 0
                                                );


                                            const total =
                                                context.dataset.data
                                                    .reduce(
                                                        function (
                                                            sum,
                                                            item
                                                        ) {

                                                            return (
                                                                sum
                                                                +
                                                                Number(
                                                                    item || 0
                                                                )
                                                            );

                                                        },
                                                        0
                                                    );


                                            const percent =
                                                total > 0

                                                    ? (
                                                        value /
                                                        total *
                                                        100
                                                    ).toFixed(1)

                                                    : "0.0";


                                            return (
                                                context.label
                                                +
                                                ": "
                                                +
                                                value
                                                +
                                                " đơn ("
                                                +
                                                percent
                                                +
                                                "%)"
                                            );

                                        }

                                }

                            }

                        }

                    }

                }
            );

        }

    }
);