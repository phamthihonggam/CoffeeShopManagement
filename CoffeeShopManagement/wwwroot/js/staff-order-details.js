/* =====================================================
   ROSALIE COFFEE
   STAFF ORDER DETAILS
===================================================== */


/* =====================================================
   PRINT INVOICE
===================================================== */

function printOrderInvoice() {

    const invoice =
        document.getElementById("orderInvoicePrint");

    if (!invoice) {

        alert("Không tìm thấy hóa đơn.");

        return;
    }


    const printWindow =
        window.open(
            "",
            "_blank",
            "width=900,height=900"
        );


    if (!printWindow) {

        alert("Trình duyệt đang chặn cửa sổ in.");

        return;
    }


    const styles =
        Array.from(
            document.querySelectorAll(
                'link[rel="stylesheet"], style'
            )
        )
            .map(function (element) {

                return element.outerHTML;

            })
            .join("");


    printWindow.document.open();


    printWindow.document.write(
        '<!DOCTYPE html>' +

        '<html lang="vi">' +

        '<head>' +

        '<meta charset="utf-8">' +

        '<title>Hóa đơn Rosalie Coffee</title>' +

        styles +

        '<style>' +

        'html, body {' +
        'margin:0 !important;' +
        'padding:0 !important;' +
        'background:#ffffff !important;' +
        '}' +

        'body {' +
        'font-family:Arial,Helvetica,sans-serif !important;' +
        '}' +

        '.admin-invoice-document {' +
        'display:block !important;' +
        'position:static !important;' +
        'width:100% !important;' +
        'max-width:none !important;' +
        'margin:0 !important;' +
        'padding:10mm !important;' +
        'box-sizing:border-box !important;' +
        'background:#ffffff !important;' +
        'box-shadow:none !important;' +
        '}' +

        '.admin-invoice-table tr {' +
        'break-inside:avoid !important;' +
        'page-break-inside:avoid !important;' +
        '}' +

        '</style>' +

        '</head>' +

        '<body>' +

        invoice.outerHTML +

        '</body>' +

        '</html>'
    );


    printWindow.document.close();


    setTimeout(function () {

        printWindow.focus();

        printWindow.print();


        setTimeout(function () {

            printWindow.close();

        }, 500);

    }, 500);
}


/* =====================================================
   DOWNLOAD PDF
===================================================== */

function downloadOrderPdf() {

    const invoice =
        document.getElementById("orderInvoicePrint");


    if (!invoice) {

        alert("Không tìm thấy hóa đơn.");

        return;
    }


    if (typeof html2pdf === "undefined") {

        alert(
            "Không tải được thư viện tạo PDF."
        );

        return;
    }


    /* =================================================
       GET ORDER CODE
    ================================================= */

    const orderCodeElement =
        invoice.querySelector(
            ".admin-invoice-code strong"
        );


    let orderCode =
        orderCodeElement
            ? orderCodeElement.textContent.trim()
            : "HoaDon";


    orderCode =
        orderCode.replace("#", "");


    /* =================================================
       CLONE INVOICE
    ================================================= */

    const clone =
        invoice.cloneNode(true);


    clone.removeAttribute("id");


    clone.style.display =
        "block";

    clone.style.position =
        "fixed";

    clone.style.left =
        "-10000px";

    clone.style.top =
        "0";

    clone.style.width =
        "794px";

    clone.style.maxWidth =
        "794px";

    clone.style.margin =
        "0";

    clone.style.padding =
        "36px";

    clone.style.boxSizing =
        "border-box";

    clone.style.backgroundColor =
        "#ffffff";

    clone.style.opacity =
        "1";

    clone.style.visibility =
        "visible";


    document.body.appendChild(
        clone
    );


    /* =================================================
       PDF OPTIONS
    ================================================= */

    const options = {

        margin: [
            5,
            5,
            5,
            5
        ],

        filename:
            "RosalieCoffee-" +
            orderCode +
            ".pdf",

        image: {

            type:
                "jpeg",

            quality:
                0.98
        },

        html2canvas: {

            scale:
                2,

            useCORS:
                true,

            allowTaint:
                true,

            backgroundColor:
                "#ffffff",

            scrollX:
                0,

            scrollY:
                0,

            windowWidth:
                794
        },

        jsPDF: {

            unit:
                "mm",

            format:
                "a4",

            orientation:
                "portrait"
        },

        pagebreak: {

            mode: [
                "css",
                "legacy"
            ],

            avoid: [
                ".admin-invoice-header",
                ".admin-invoice-meta",
                ".admin-invoice-customer-grid",
                ".admin-invoice-summary",
                ".admin-invoice-footer"
            ]
        }
    };


    /* =================================================
       CREATE PDF
    ================================================= */

    html2pdf()

        .set(options)

        .from(clone)

        .save()

        .then(function () {

            if (clone.parentNode) {

                clone.parentNode.removeChild(
                    clone
                );
            }

        })

        .catch(function (error) {

            console.error(
                "PDF ERROR:",
                error
            );


            if (clone.parentNode) {

                clone.parentNode.removeChild(
                    clone
                );
            }


            alert(
                "Không thể tạo PDF. Vui lòng thử lại."
            );
        });
}