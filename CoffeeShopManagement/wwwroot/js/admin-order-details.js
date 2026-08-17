/* =====================================================
   ROSALIE COFFEE
   ADMIN ORDER DETAILS

   PRINT INVOICE
   EXPORT PDF THROUGH BROWSER
===================================================== */


document.addEventListener(
    "DOMContentLoaded",
    function () {

        const printButton =
            document.getElementById(
                "btnPrintInvoice"
            );


        const pdfButton =
            document.getElementById(
                "btnDownloadPdf"
            );


        /* =================================================
           PRINT
        ================================================= */

        if (printButton) {

            printButton.addEventListener(
                "click",
                function () {

                    openInvoiceWindow(
                        "print"
                    );

                }
            );

        }


        /* =================================================
           PDF
        ================================================= */

        if (pdfButton) {

            pdfButton.addEventListener(
                "click",
                function () {

                    openInvoiceWindow(
                        "pdf"
                    );

                }
            );

        }

    }
);


/* =====================================================
   GET INVOICE
===================================================== */

function getInvoiceElement() {

    return document.getElementById(
        "orderInvoicePrint"
    );

}


/* =====================================================
   OPEN CLEAN INVOICE WINDOW
===================================================== */

function openInvoiceWindow(mode) {

    const invoice =
        getInvoiceElement();


    /* =================================================
       CHECK
    ================================================= */

    if (!invoice) {

        alert(
            "Không tìm thấy hóa đơn."
        );

        return;

    }


    /* =================================================
       DATA
    ================================================= */

    const cssUrl =
        invoice.dataset.cssUrl
        ||
        "/css/admin-order.css";


    const orderCode =
        invoice.dataset.orderCode
        ||
        "Invoice";


    const isPdf =
        mode === "pdf";


    /* =================================================
       OPEN WINDOW

       Phải gọi window.open trực tiếp từ click
       để tránh popup blocker.
    ================================================= */

    const invoiceWindow =
        window.open(
            "",
            "_blank",
            "width=1000,height=900"
        );


    if (!invoiceWindow) {

        alert(
            "Trình duyệt đang chặn cửa sổ mới.\n" +
            "Hãy cho phép Popup cho localhost rồi thử lại."
        );

        return;

    }


    /* =================================================
       TITLE

       Khi Save as PDF, Chrome thường dùng title này
       làm tên file mặc định.
    ================================================= */

    const pageTitle =
        isPdf

            ? "RosalieCoffee-" + orderCode

            : "HoaDon-" + orderCode;


    /* =================================================
       CLONE INVOICE
    ================================================= */

    const invoiceClone =
        invoice.cloneNode(
            true
        );


    invoiceClone.removeAttribute(
        "id"
    );


    invoiceClone.style.display =
        "block";


    /* =================================================
       BUILD HTML
    ================================================= */

    const html =
        `
        <!DOCTYPE html>

        <html lang="vi">

        <head>

            <meta charset="UTF-8">

            <meta name="viewport"
                  content="width=device-width,
                           initial-scale=1.0">


            <title>${pageTitle}</title>


            <link rel="stylesheet"
                  href="${cssUrl}">


            <style>

                /* ==========================================
                   DOCUMENT
                ========================================== */

                html,
                body {

                    width: 100%;

                    margin: 0 !important;
                    padding: 0 !important;

                    background: #ffffff !important;

                }


                body {

                    color: #392720;

                    font-family:
                        Arial,
                        Helvetica,
                        sans-serif;

                }


                /* ==========================================
                   INVOICE
                ========================================== */

                .admin-invoice-document {

                    display: block !important;

                    position: static !important;

                    width: 210mm !important;
                    max-width: 210mm !important;

                    min-height: 0 !important;

                    margin: 0 auto !important;

                    padding:
                        12mm
                        12mm
                        10mm !important;

                    box-sizing: border-box !important;

                    background: #ffffff !important;

                    box-shadow: none !important;

                    visibility: visible !important;

                    opacity: 1 !important;

                }


                /* ==========================================
                   PAGE BREAK
                ========================================== */

                .admin-invoice-header,
                .admin-invoice-meta,
                .admin-invoice-customer-grid,
                .admin-invoice-summary,
                .admin-invoice-footer {

                    break-inside: avoid !important;

                    page-break-inside: avoid !important;

                }


                .admin-invoice-table {

                    width: 100% !important;

                    page-break-inside: auto !important;

                }


                .admin-invoice-table thead {

                    display:
                        table-header-group !important;

                }


                .admin-invoice-table tr {

                    break-inside:
                        avoid !important;

                    page-break-inside:
                        avoid !important;

                    page-break-after:
                        auto !important;

                }


                /* ==========================================
                   PRINT
                ========================================== */

                @media print {

                    html,
                    body {

                        width: 210mm !important;

                        margin: 0 !important;
                        padding: 0 !important;

                        background:
                            #ffffff !important;

                    }


                    body {

                        -webkit-print-color-adjust:
                            exact !important;

                        print-color-adjust:
                            exact !important;

                    }


                    .admin-invoice-document {

                        display:
                            block !important;

                        position:
                            static !important;

                        width:
                            210mm !important;

                        max-width:
                            210mm !important;

                        min-height:
                            0 !important;

                        margin:
                            0 !important;

                        padding:
                            10mm !important;

                        box-sizing:
                            border-box !important;

                        background:
                            #ffffff !important;

                    }


                    .admin-invoice-header,
                    .admin-invoice-meta,
                    .admin-invoice-customer-grid,
                    .admin-invoice-summary,
                    .admin-invoice-footer {

                        break-inside:
                            avoid !important;

                        page-break-inside:
                            avoid !important;

                    }


                    .admin-invoice-table tr {

                        break-inside:
                            avoid !important;

                        page-break-inside:
                            avoid !important;

                    }

                }


                /* ==========================================
                   A4
                ========================================== */

                @page {

                    size:
                        A4 portrait;

                    margin:
                        0;

                }

            </style>

        </head>


        <body>

            ${invoiceClone.outerHTML}

        </body>

        </html>
        `;


    /* =================================================
       WRITE WINDOW
    ================================================= */

    invoiceWindow.document.open();


    invoiceWindow.document.write(
        html
    );


    invoiceWindow.document.close();


    /* =================================================
       PRINT ONCE
    ================================================= */

    let printed =
        false;


    function startPrint() {

        if (printed) {

            return;

        }


        printed =
            true;


        setTimeout(
            function () {

                try {

                    invoiceWindow.focus();


                    invoiceWindow.print();

                }
                catch (error) {

                    console.error(
                        "Invoice print error:",
                        error
                    );


                    alert(
                        "Không thể mở hộp thoại in."
                    );

                }

            },
            350
        );

    }


    /* =================================================
       WAIT CSS LOAD
    ================================================= */

    invoiceWindow.onload =
        function () {

            startPrint();

        };


    /* =================================================
       FALLBACK
    ================================================= */

    setTimeout(
        function () {

            startPrint();

        },
        1200
    );


    /* =================================================
       CLOSE AFTER PRINT
    ================================================= */

    invoiceWindow.onafterprint =
        function () {

            setTimeout(
                function () {

                    try {

                        invoiceWindow.close();

                    }
                    catch (error) {

                        console.log(
                            error
                        );

                    }

                },
                200
            );

        };

}