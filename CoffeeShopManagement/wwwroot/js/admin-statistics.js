// ======================================================
// ROSALIE COFFEE
// ADMIN STATISTICS
// ======================================================

document.addEventListener("DOMContentLoaded", function () {

    const dataElement =
        document.getElementById("statisticsData");

    const chartCanvas =
        document.getElementById("revenueChart");


    if (!dataElement ||
        !chartCanvas ||
        typeof Chart === "undefined") {

        return;
    }


    let statisticsData;


    try {

        statisticsData =
            JSON.parse(
                dataElement.textContent
            );

    }
    catch {

        return;

    }


    const labels =
        statisticsData.labels || [];

    const values =
        statisticsData.values || [];


    new Chart(
        chartCanvas,
        {
            type: "line",

            data: {

                labels: labels,

                datasets: [
                    {
                        label: "Doanh thu",

                        data: values,

                        borderColor:
                            "#875238",

                        backgroundColor:
                            "rgba(135, 82, 56, 0.12)",

                        borderWidth: 3,

                        fill: true,

                        tension: 0.35,

                        pointRadius: 4,

                        pointHoverRadius: 6
                    }
                ]
            },


            options: {

                responsive: true,

                maintainAspectRatio: false,


                interaction: {

                    mode: "index",

                    intersect: false
                },


                plugins: {

                    legend: {

                        display: false
                    },


                    tooltip: {

                        callbacks: {

                            label: function (context) {

                                const value =
                                    Number(
                                        context.raw || 0
                                    );


                                return (
                                    value.toLocaleString(
                                        "vi-VN"
                                    )
                                    +
                                    "đ"
                                );

                            }

                        }

                    }

                },


                scales: {

                    x: {

                        grid: {

                            display: false
                        }

                    },


                    y: {

                        beginAtZero: true,

                        ticks: {

                            callback: function (value) {

                                return Number(value)
                                    .toLocaleString(
                                        "vi-VN"
                                    )
                                    +
                                    "đ";

                            }

                        }

                    }

                }

            }
        }
    );

});