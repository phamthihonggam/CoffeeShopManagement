using CoffeeShopManagement.Data;
using CoffeeShopManagement.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO.Compression;
using System.Security;
using System.Text;

namespace CoffeeShopManagement.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize("STATISTICS_VIEW")]
    public class StatisticsController : Controller
    {
        private readonly CoffeeShopDbContext _context;


        public StatisticsController(
            CoffeeShopDbContext context)
        {
            _context = context;
        }


        // =====================================================
        // INDEX
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> Index(
            string? period,
            int? month,
            int? year,
            DateTime? fromDate,
            DateTime? toDate)
        {
            var model =
                await BuildStatisticsModel(
                    period,
                    month,
                    year,
                    fromDate,
                    toDate
                );


            return View(model);
        }


        // =====================================================
        // EXPORT EXCEL
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> ExportExcel(
            string? period,
            int? month,
            int? year,
            DateTime? fromDate,
            DateTime? toDate)
        {
            var model =
                await BuildStatisticsModel(
                    period,
                    month,
                    year,
                    fromDate,
                    toDate
                );


            var excelBytes =
                BuildExcelFile(model);


            var fileName =
                $"RosalieCoffee-ThongKe-" +
                $"{model.FromDate:yyyyMMdd}-" +
                $"{model.ToDate:yyyyMMdd}.xlsx";


            return File(
                excelBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName
            );
        }


        // =====================================================
        // PRINT / PDF REPORT
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> PrintReport(
            string? period,
            int? month,
            int? year,
            DateTime? fromDate,
            DateTime? toDate)
        {
            var model =
                await BuildStatisticsModel(
                    period,
                    month,
                    year,
                    fromDate,
                    toDate
                );


            return View(model);
        }


        // =====================================================
        // BUILD STATISTICS MODEL
        // =====================================================

        private async Task<StatisticsViewModel>
            BuildStatisticsModel(
                string? period,
                int? month,
                int? year,
                DateTime? fromDate,
                DateTime? toDate)
        {
            var today =
                DateTime.Today;


            // =================================================
            // PERIOD
            // =================================================

            period =
                string.IsNullOrWhiteSpace(period)
                    ? "7days"
                    : period.Trim().ToLower();


            var allowedPeriods =
                new[]
                {
                    "7days",
                    "month",
                    "year",
                    "custom"
                };


            if (!allowedPeriods.Contains(period))
            {
                period =
                    "7days";
            }


            // =================================================
            // AVAILABLE YEARS
            // =================================================

            var availableYears =
                await _context.HoaDons

                    .Where(x =>
                        x.NgayDat.HasValue
                    )

                    .Select(x =>
                        x.NgayDat!.Value.Year
                    )

                    .Distinct()

                    .OrderByDescending(x =>
                        x
                    )

                    .ToListAsync();


            if (!availableYears.Contains(today.Year))
            {
                availableYears.Add(
                    today.Year
                );
            }


            // =================================================
            // SELECTED MONTH / YEAR
            // =================================================

            var selectedMonth =
                month ?? today.Month;


            var selectedYear =
                year ?? today.Year;


            if (
                selectedMonth < 1
                ||
                selectedMonth > 12
            )
            {
                selectedMonth =
                    today.Month;
            }


            if (
                selectedYear < 2000
                ||
                selectedYear > 2100
            )
            {
                selectedYear =
                    today.Year;
            }


            if (!availableYears.Contains(selectedYear))
            {
                availableYears.Add(
                    selectedYear
                );
            }


            availableYears =
                availableYears

                    .Distinct()

                    .OrderByDescending(x =>
                        x
                    )

                    .ToList();


            // =================================================
            // DATE RANGE
            // =================================================

            DateTime selectedFromDate;

            DateTime selectedToDate;

            string periodTitle;


            // =================================================
            // 7 DAYS
            // =================================================

            if (period == "7days")
            {
                selectedFromDate =
                    today.AddDays(-6);


                selectedToDate =
                    today;


                periodTitle =
                    "7 ngày gần nhất";
            }

            // =================================================
            // MONTH
            // =================================================

            else if (period == "month")
            {
                selectedFromDate =
                    new DateTime(
                        selectedYear,
                        selectedMonth,
                        1
                    );


                selectedToDate =
                    new DateTime(
                        selectedYear,
                        selectedMonth,
                        DateTime.DaysInMonth(
                            selectedYear,
                            selectedMonth
                        )
                    );


                periodTitle =
                    $"Tháng {selectedMonth}/{selectedYear}";
            }

            // =================================================
            // YEAR
            // =================================================

            else if (period == "year")
            {
                selectedFromDate =
                    new DateTime(
                        selectedYear,
                        1,
                        1
                    );


                selectedToDate =
                    new DateTime(
                        selectedYear,
                        12,
                        31
                    );


                periodTitle =
                    $"Năm {selectedYear}";
            }

            // =================================================
            // CUSTOM
            // =================================================

            else
            {
                selectedFromDate =
                    (
                        fromDate
                        ??
                        today.AddDays(-6)
                    ).Date;


                selectedToDate =
                    (
                        toDate
                        ??
                        today
                    ).Date;


                if (
                    selectedFromDate
                    >
                    selectedToDate
                )
                {
                    var temp =
                        selectedFromDate;


                    selectedFromDate =
                        selectedToDate;


                    selectedToDate =
                        temp;
                }


                periodTitle =
                    "Khoảng thời gian tùy chọn";
            }


            var endExclusive =
                selectedToDate.AddDays(1);


            // =================================================
            // LOAD ORDERS
            // =================================================

            var periodOrders =
                await _context.HoaDons

                    .Where(x =>
                        x.NgayDat.HasValue
                        &&
                        x.NgayDat.Value >= selectedFromDate
                        &&
                        x.NgayDat.Value < endExclusive
                    )

                    .Include(x =>
                        x.MaKhNavigation
                    )

                    .Include(x =>
                        x.ChiTietHoaDons
                    )

                        .ThenInclude(x =>
                            x.MaSpNavigation
                        )

                    .Include(x =>
                        x.ChiTietHoaDons
                    )

                        .ThenInclude(x =>
                            x.MaComboNavigation
                        )

                    .AsSplitQuery()

                    .AsNoTracking()

                    .ToListAsync();


            // =================================================
            // SYSTEM TOTALS
            // =================================================

            var totalCustomers =
                await _context.KhachHangs

                    .AsNoTracking()

                    .CountAsync();


            var totalProducts =
                await _context.SanPhams

                    .AsNoTracking()

                    .CountAsync();


            // =================================================
            // PERIOD TOTALS
            // =================================================

            var totalOrders =
                periodOrders.Count;


            var completedOrderList =
                periodOrders

                    .Where(x =>
                        x.TrangThai == "Hoàn thành"
                    )

                    .ToList();


            var completedOrders =
                completedOrderList.Count;


            var totalRevenue =
                completedOrderList

                    .Sum(x =>
                        x.TongTien ?? 0
                    );


            var averageOrderValue =
                completedOrders > 0

                    ? totalRevenue / completedOrders

                    : 0;


            var uniqueCustomers =
                periodOrders

                    .Where(x =>
                        x.MaKhNavigation != null
                    )

                    .Select(x =>
                        x.MaKh
                    )

                    .Distinct()

                    .Count();


            // =================================================
            // STATUS GROUPS
            // =================================================

            var pendingStatuses =
                new[]
                {
                    "Chờ xác nhận",
                    "Chờ xử lý",
                    "Chờ thanh toán",
                    "Đã thanh toán",
                    "Yêu cầu hủy",
                    "Yêu cầu hủy khi đang giao"
                };


            var processingStatuses =
                new[]
                {
                    "Đã xác nhận",
                    "Đang xử lý",
                    "Đang chuẩn bị"
                };


            var shippingStatuses =
                new[]
                {
                    "Chờ giao hàng",
                    "Đang giao hàng"
                };


            var pendingOrders =
                periodOrders.Count(x =>
                    pendingStatuses.Contains(
                        x.TrangThai ?? ""
                    )
                );


            var processingOrders =
                periodOrders.Count(x =>
                    processingStatuses.Contains(
                        x.TrangThai ?? ""
                    )
                );


            var shippingOrders =
                periodOrders.Count(x =>
                    shippingStatuses.Contains(
                        x.TrangThai ?? ""
                    )
                );


            var cancelledOrders =
                periodOrders.Count(x =>
                    !string.IsNullOrWhiteSpace(
                        x.TrangThai
                    )
                    &&
                    (
                        x.TrangThai.StartsWith(
                            "Đã hủy"
                        )
                        ||
                        x.TrangThai ==
                        "Giao thất bại"
                    )
                );


            // =================================================
            // REVENUE CHART DATA
            // =================================================

            var revenueLabels =
                new List<string>();


            var revenueValues =
                new List<decimal>();


            string revenuePeriodTitle;


            // =================================================
            // YEAR -> 12 MONTHS
            // =================================================

            if (period == "year")
            {
                revenuePeriodTitle =
                    $"Doanh thu 12 tháng năm {selectedYear}";


                for (
                    int currentMonth = 1;
                    currentMonth <= 12;
                    currentMonth++
                )
                {
                    revenueLabels.Add(
                        $"T{currentMonth}"
                    );


                    var revenue =
                        completedOrderList

                            .Where(x =>
                                x.NgayDat.HasValue
                                &&
                                x.NgayDat.Value.Year ==
                                selectedYear
                                &&
                                x.NgayDat.Value.Month ==
                                currentMonth
                            )

                            .Sum(x =>
                                x.TongTien ?? 0
                            );


                    revenueValues.Add(
                        revenue
                    );
                }
            }

            // =================================================
            // MONTH -> DAYS
            // =================================================

            else if (period == "month")
            {
                revenuePeriodTitle =
                    $"Doanh thu tháng {selectedMonth}/{selectedYear}";


                var daysInMonth =
                    DateTime.DaysInMonth(
                        selectedYear,
                        selectedMonth
                    );


                for (
                    int day = 1;
                    day <= daysInMonth;
                    day++
                )
                {
                    var date =
                        new DateTime(
                            selectedYear,
                            selectedMonth,
                            day
                        );


                    revenueLabels.Add(
                        day.ToString("00")
                    );


                    var revenue =
                        completedOrderList

                            .Where(x =>
                                x.NgayDat.HasValue
                                &&
                                x.NgayDat.Value.Date ==
                                date
                            )

                            .Sum(x =>
                                x.TongTien ?? 0
                            );


                    revenueValues.Add(
                        revenue
                    );
                }
            }

            // =================================================
            // 7 DAYS
            // =================================================

            else if (period == "7days")
            {
                revenuePeriodTitle =
                    "Doanh thu 7 ngày gần nhất";


                for (
                    int i = 0;
                    i < 7;
                    i++
                )
                {
                    var date =
                        selectedFromDate
                            .AddDays(i);


                    revenueLabels.Add(
                        date.ToString("dd/MM")
                    );


                    var revenue =
                        completedOrderList

                            .Where(x =>
                                x.NgayDat.HasValue
                                &&
                                x.NgayDat.Value.Date ==
                                date
                            )

                            .Sum(x =>
                                x.TongTien ?? 0
                            );


                    revenueValues.Add(
                        revenue
                    );
                }
            }

            // =================================================
            // CUSTOM
            // =================================================

            else
            {
                var totalDays =
                    (
                        selectedToDate
                        -
                        selectedFromDate
                    ).Days + 1;


                if (totalDays <= 31)
                {
                    revenuePeriodTitle =
                        "Doanh thu theo ngày";


                    for (
                        int i = 0;
                        i < totalDays;
                        i++
                    )
                    {
                        var date =
                            selectedFromDate
                                .AddDays(i);


                        revenueLabels.Add(
                            date.ToString("dd/MM")
                        );


                        var revenue =
                            completedOrderList

                                .Where(x =>
                                    x.NgayDat.HasValue
                                    &&
                                    x.NgayDat.Value.Date ==
                                    date
                                )

                                .Sum(x =>
                                    x.TongTien ?? 0
                                );


                        revenueValues.Add(
                            revenue
                        );
                    }
                }
                else
                {
                    revenuePeriodTitle =
                        "Doanh thu theo tháng";


                    var currentMonth =
                        new DateTime(
                            selectedFromDate.Year,
                            selectedFromDate.Month,
                            1
                        );


                    var lastMonth =
                        new DateTime(
                            selectedToDate.Year,
                            selectedToDate.Month,
                            1
                        );


                    while (
                        currentMonth
                        <=
                        lastMonth
                    )
                    {
                        revenueLabels.Add(
                            currentMonth
                                .ToString("MM/yyyy")
                        );


                        var monthValue =
                            currentMonth.Month;


                        var yearValue =
                            currentMonth.Year;


                        var revenue =
                            completedOrderList

                                .Where(x =>
                                    x.NgayDat.HasValue
                                    &&
                                    x.NgayDat.Value.Year ==
                                    yearValue
                                    &&
                                    x.NgayDat.Value.Month ==
                                    monthValue
                                )

                                .Sum(x =>
                                    x.TongTien ?? 0
                                );


                        revenueValues.Add(
                            revenue
                        );


                        currentMonth =
                            currentMonth
                                .AddMonths(1);
                    }
                }
            }


            // =================================================
            // STATUS CHART
            // =================================================

            var statusLabels =
                new List<string>
                {
                    "Chờ xử lý",
                    "Đang xử lý",
                    "Đang giao",
                    "Hoàn thành",
                    "Đã hủy"
                };


            var statusValues =
                new List<int>
                {
                    pendingOrders,
                    processingOrders,
                    shippingOrders,
                    completedOrders,
                    cancelledOrders
                };


            // =================================================
            // TOP 5 PRODUCTS
            // =================================================

            var topProducts =
                completedOrderList

                    .SelectMany(x =>
                        x.ChiTietHoaDons
                    )

                    .Select(x =>
                        new
                        {
                            Name =
                                x.MaSpNavigation?.TenSp
                                ??
                                x.MaComboNavigation?.TenCombo
                                ??
                                "Không xác định",

                            Type =
                                x.MaCombo.HasValue
                                    ? "Combo"
                                    : "Sản phẩm",

                            Quantity =
                                x.SoLuong,

                            Revenue =
                                x.DonGia
                                *
                                x.SoLuong
                        }
                    )

                    .GroupBy(x =>
                        new
                        {
                            x.Name,
                            x.Type
                        }
                    )

                    .Select(x =>
                        new StatisticsTopProductItem
                        {
                            Name =
                                x.Key.Name,

                            Type =
                                x.Key.Type,

                            Quantity =
                                x.Sum(y =>
                                    y.Quantity
                                ),

                            Revenue =
                                x.Sum(y =>
                                    y.Revenue
                                )
                        }
                    )

                    .OrderByDescending(x =>
                        x.Quantity
                    )

                    .ThenByDescending(x =>
                        x.Revenue
                    )

                    .Take(5)

                    .ToList();


            // =================================================
            // MODEL
            // =================================================

            return new StatisticsViewModel
            {
                Period =
                    period,

                PeriodTitle =
                    periodTitle,

                SelectedMonth =
                    selectedMonth,

                SelectedYear =
                    selectedYear,

                AvailableYears =
                    availableYears,

                FromDate =
                    selectedFromDate,

                ToDate =
                    selectedToDate,

                TotalRevenue =
                    totalRevenue,

                TotalOrders =
                    totalOrders,

                UniqueCustomers =
                    uniqueCustomers,

                AverageOrderValue =
                    averageOrderValue,

                TotalCustomers =
                    totalCustomers,

                TotalProducts =
                    totalProducts,

                PendingOrders =
                    pendingOrders,

                ProcessingOrders =
                    processingOrders,

                ShippingOrders =
                    shippingOrders,

                CompletedOrders =
                    completedOrders,

                CancelledOrders =
                    cancelledOrders,

                RevenuePeriodTitle =
                    revenuePeriodTitle,

                RevenueLabels =
                    revenueLabels,

                RevenueValues =
                    revenueValues,

                StatusLabels =
                    statusLabels,

                StatusValues =
                    statusValues,

                TopProducts =
                    topProducts
            };
        }


        // =====================================================
        // BUILD XLSX
        // KHÔNG CẦN THƯ VIỆN NGOÀI
        // =====================================================

        private static byte[] BuildExcelFile(
            StatisticsViewModel model)
        {
            using var memoryStream =
                new MemoryStream();


            using (
                var archive =
                    new ZipArchive(
                        memoryStream,
                        ZipArchiveMode.Create,
                        true
                    )
            )
            {
                AddZipEntry(
                    archive,
                    "[Content_Types].xml",
                    BuildContentTypesXml()
                );


                AddZipEntry(
                    archive,
                    "_rels/.rels",
                    BuildRootRelationshipsXml()
                );


                AddZipEntry(
                    archive,
                    "xl/workbook.xml",
                    BuildWorkbookXml()
                );


                AddZipEntry(
                    archive,
                    "xl/_rels/workbook.xml.rels",
                    BuildWorkbookRelationshipsXml()
                );


                AddZipEntry(
                    archive,
                    "xl/styles.xml",
                    BuildStylesXml()
                );


                AddZipEntry(
                    archive,
                    "xl/worksheets/sheet1.xml",
                    BuildWorksheetXml(model)
                );
            }


            return memoryStream.ToArray();
        }


        // =====================================================
        // ADD ZIP ENTRY
        // =====================================================

        private static void AddZipEntry(
            ZipArchive archive,
            string path,
            string content)
        {
            var entry =
                archive.CreateEntry(
                    path,
                    CompressionLevel.Fastest
                );


            using var stream =
                entry.Open();


            using var writer =
                new StreamWriter(
                    stream,
                    new UTF8Encoding(false)
                );


            writer.Write(content);
        }


        // =====================================================
        // XLSX XML
        // =====================================================

        private static string BuildContentTypesXml()
        {
            return """
<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<Types xmlns="http://schemas.openxmlformats.org/package/2006/content-types">
    <Default Extension="rels"
             ContentType="application/vnd.openxmlformats-package.relationships+xml"/>
    <Default Extension="xml"
             ContentType="application/xml"/>
    <Override PartName="/xl/workbook.xml"
              ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml"/>
    <Override PartName="/xl/worksheets/sheet1.xml"
              ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml"/>
    <Override PartName="/xl/styles.xml"
              ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.styles+xml"/>
</Types>
""";
        }


        private static string BuildRootRelationshipsXml()
        {
            return """
<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships">
    <Relationship Id="rId1"
                  Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument"
                  Target="xl/workbook.xml"/>
</Relationships>
""";
        }


        private static string BuildWorkbookXml()
        {
            return """
<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<workbook xmlns="http://schemas.openxmlformats.org/spreadsheetml/2006/main"
          xmlns:r="http://schemas.openxmlformats.org/officeDocument/2006/relationships">
    <sheets>
        <sheet name="Thống kê Rosalie"
               sheetId="1"
               r:id="rId1"/>
    </sheets>
</workbook>
""";
        }


        private static string BuildWorkbookRelationshipsXml()
        {
            return """
<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships">
    <Relationship Id="rId1"
                  Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet"
                  Target="worksheets/sheet1.xml"/>
    <Relationship Id="rId2"
                  Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles"
                  Target="styles.xml"/>
</Relationships>
""";
        }


        private static string BuildStylesXml()
        {
            return """
<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<styleSheet xmlns="http://schemas.openxmlformats.org/spreadsheetml/2006/main">

    <fonts count="3">

        <font>
            <sz val="11"/>
            <name val="Calibri"/>
        </font>

        <font>
            <b/>
            <sz val="11"/>
            <name val="Calibri"/>
        </font>

        <font>
            <b/>
            <sz val="16"/>
            <color rgb="FF4C2D25"/>
            <name val="Calibri"/>
        </font>

    </fonts>


    <fills count="3">

        <fill>
            <patternFill patternType="none"/>
        </fill>

        <fill>
            <patternFill patternType="gray125"/>
        </fill>

        <fill>
            <patternFill patternType="solid">
                <fgColor rgb="FFF4E8E0"/>
                <bgColor indexed="64"/>
            </patternFill>
        </fill>

    </fills>


    <borders count="2">

        <border>
            <left/>
            <right/>
            <top/>
            <bottom/>
            <diagonal/>
        </border>

        <border>

            <left style="thin">
                <color rgb="FFE5D8D0"/>
            </left>

            <right style="thin">
                <color rgb="FFE5D8D0"/>
            </right>

            <top style="thin">
                <color rgb="FFE5D8D0"/>
            </top>

            <bottom style="thin">
                <color rgb="FFE5D8D0"/>
            </bottom>

            <diagonal/>

        </border>

    </borders>


    <cellStyleXfs count="1">
        <xf numFmtId="0"
            fontId="0"
            fillId="0"
            borderId="0"/>
    </cellStyleXfs>


    <cellXfs count="4">

        <xf numFmtId="0"
            fontId="0"
            fillId="0"
            borderId="0"
            xfId="0"/>

        <xf numFmtId="0"
            fontId="2"
            fillId="0"
            borderId="0"
            xfId="0"
            applyAlignment="1">
            <alignment horizontal="center"/>
        </xf>

        <xf numFmtId="0"
            fontId="1"
            fillId="2"
            borderId="1"
            xfId="0"
            applyAlignment="1">
            <alignment horizontal="center"/>
        </xf>

        <xf numFmtId="0"
            fontId="1"
            fillId="0"
            borderId="0"
            xfId="0"/>

    </cellXfs>


    <cellStyles count="1">

        <cellStyle name="Normal"
                   xfId="0"
                   builtinId="0"/>

    </cellStyles>

</styleSheet>
""";
        }


        // =====================================================
        // SHEET
        // =====================================================

        private static string BuildWorksheetXml(
            StatisticsViewModel model)
        {
            var sb =
                new StringBuilder();


            sb.Append("""
<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<worksheet xmlns="http://schemas.openxmlformats.org/spreadsheetml/2006/main">

    <cols>
        <col min="1" max="1" width="24" customWidth="1"/>
        <col min="2" max="2" width="34" customWidth="1"/>
        <col min="3" max="3" width="18" customWidth="1"/>
        <col min="4" max="4" width="18" customWidth="1"/>
        <col min="5" max="5" width="22" customWidth="1"/>
    </cols>

    <sheetData>
""");


            sb.Append(
                ExcelRow(
                    1,
                    ExcelCell(
                        "A1",
                        "ROSALIE COFFEE - BÁO CÁO THỐNG KÊ",
                        1
                    )
                )
            );


            sb.Append(
                ExcelRow(
                    2,
                    ExcelCell(
                        "A2",
                        $"Kỳ: {model.PeriodTitle} | " +
                        $"{model.FromDate:dd/MM/yyyy} - " +
                        $"{model.ToDate:dd/MM/yyyy}",
                        3
                    )
                )
            );


            sb.Append(
                ExcelRow(
                    4,
                    ExcelCell(
                        "A4",
                        "CHỈ TIÊU",
                        2
                    ),
                    ExcelCell(
                        "B4",
                        "GIÁ TRỊ",
                        2
                    )
                )
            );


            sb.Append(
                ExcelRow(
                    5,
                    ExcelCell(
                        "A5",
                        "Doanh thu trong kỳ"
                    ),
                    ExcelCell(
                        "B5",
                        $"{model.TotalRevenue:#,##0}đ"
                    )
                )
            );


            sb.Append(
                ExcelRow(
                    6,
                    ExcelCell(
                        "A6",
                        "Tổng đơn hàng"
                    ),
                    ExcelCell(
                        "B6",
                        model.TotalOrders.ToString()
                    )
                )
            );


            sb.Append(
                ExcelRow(
                    7,
                    ExcelCell(
                        "A7",
                        "Khách phát sinh đơn"
                    ),
                    ExcelCell(
                        "B7",
                        model.UniqueCustomers.ToString()
                    )
                )
            );


            sb.Append(
                ExcelRow(
                    8,
                    ExcelCell(
                        "A8",
                        "Giá trị trung bình / đơn"
                    ),
                    ExcelCell(
                        "B8",
                        $"{model.AverageOrderValue:#,##0}đ"
                    )
                )
            );


            sb.Append(
                ExcelRow(
                    10,
                    ExcelCell(
                        "A10",
                        "TRẠNG THÁI ĐƠN",
                        2
                    ),
                    ExcelCell(
                        "B10",
                        "SỐ LƯỢNG",
                        2
                    )
                )
            );


            sb.Append(
                ExcelRow(
                    11,
                    ExcelCell("A11", "Chờ xử lý"),
                    ExcelCell(
                        "B11",
                        model.PendingOrders.ToString()
                    )
                )
            );


            sb.Append(
                ExcelRow(
                    12,
                    ExcelCell("A12", "Đang xử lý"),
                    ExcelCell(
                        "B12",
                        model.ProcessingOrders.ToString()
                    )
                )
            );


            sb.Append(
                ExcelRow(
                    13,
                    ExcelCell("A13", "Đang giao"),
                    ExcelCell(
                        "B13",
                        model.ShippingOrders.ToString()
                    )
                )
            );


            sb.Append(
                ExcelRow(
                    14,
                    ExcelCell("A14", "Hoàn thành"),
                    ExcelCell(
                        "B14",
                        model.CompletedOrders.ToString()
                    )
                )
            );


            sb.Append(
                ExcelRow(
                    15,
                    ExcelCell("A15", "Đã hủy"),
                    ExcelCell(
                        "B15",
                        model.CancelledOrders.ToString()
                    )
                )
            );


            sb.Append(
                ExcelRow(
                    17,
                    ExcelCell("A17", "HẠNG", 2),
                    ExcelCell("B17", "TÊN MÓN", 2),
                    ExcelCell("C17", "LOẠI", 2),
                    ExcelCell("D17", "SỐ LƯỢNG", 2),
                    ExcelCell("E17", "DOANH THU", 2)
                )
            );


            var row =
                18;


            var rank =
                1;


            foreach (var item in model.TopProducts)
            {
                sb.Append(
                    ExcelRow(
                        row,
                        ExcelCell(
                            $"A{row}",
                            rank.ToString()
                        ),
                        ExcelCell(
                            $"B{row}",
                            item.Name
                        ),
                        ExcelCell(
                            $"C{row}",
                            item.Type
                        ),
                        ExcelCell(
                            $"D{row}",
                            item.Quantity.ToString()
                        ),
                        ExcelCell(
                            $"E{row}",
                            $"{item.Revenue:#,##0}đ"
                        )
                    )
                );


                row++;

                rank++;
            }


            if (!model.TopProducts.Any())
            {
                sb.Append(
                    ExcelRow(
                        row,
                        ExcelCell(
                            $"A{row}",
                            "Không có dữ liệu sản phẩm."
                        )
                    )
                );
            }


            sb.Append("""
    </sheetData>

    <mergeCells count="2">
        <mergeCell ref="A1:E1"/>
        <mergeCell ref="A2:E2"/>
    </mergeCells>

</worksheet>
""");


            return sb.ToString();
        }


        // =====================================================
        // EXCEL ROW
        // =====================================================

        private static string ExcelRow(
            int rowNumber,
            params string[] cells)
        {
            return
                $"<row r=\"{rowNumber}\">" +
                string.Join(
                    "",
                    cells
                ) +
                "</row>";
        }


        // =====================================================
        // EXCEL CELL
        // =====================================================

        private static string ExcelCell(
            string reference,
            string value,
            int style = 0)
        {
            var safeValue =
                SecurityElement.Escape(
                    value ?? ""
                )
                ??
                "";


            var styleText =
                style > 0

                    ? $" s=\"{style}\""

                    : "";


            return
                $"<c r=\"{reference}\" " +
                $"t=\"inlineStr\"{styleText}>" +
                $"<is><t>{safeValue}</t></is>" +
                "</c>";
        }
    }


    // =========================================================
    // VIEW MODEL
    // =========================================================

    public sealed class StatisticsViewModel
    {
        public string Period { get; set; } =
            "7days";


        public string PeriodTitle { get; set; } =
            "";


        public int SelectedMonth { get; set; }

        public int SelectedYear { get; set; }


        public List<int> AvailableYears { get; set; } =
            new();


        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }


        public decimal TotalRevenue { get; set; }

        public int TotalOrders { get; set; }

        public int UniqueCustomers { get; set; }

        public decimal AverageOrderValue { get; set; }


        public int TotalCustomers { get; set; }

        public int TotalProducts { get; set; }


        public int PendingOrders { get; set; }

        public int ProcessingOrders { get; set; }

        public int ShippingOrders { get; set; }

        public int CompletedOrders { get; set; }

        public int CancelledOrders { get; set; }


        public string RevenuePeriodTitle { get; set; } =
            "";


        public List<string> RevenueLabels { get; set; } =
            new();


        public List<decimal> RevenueValues { get; set; } =
            new();


        public List<string> StatusLabels { get; set; } =
            new();


        public List<int> StatusValues { get; set; } =
            new();


        public List<StatisticsTopProductItem> TopProducts
        {
            get;
            set;
        } = new();
    }


    // =========================================================
    // TOP PRODUCT
    // =========================================================

    public sealed class StatisticsTopProductItem
    {
        public string Name { get; set; } =
            "";

        public string Type { get; set; } =
            "";

        public int Quantity { get; set; }

        public decimal Revenue { get; set; }
    }
}