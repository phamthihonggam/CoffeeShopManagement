using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoffeeShopManagement.Migrations
{
    /// <inheritdoc />
    public partial class FixRegister : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Combo",
                columns: table => new
                {
                    MaCombo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenCombo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GiaGoc = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GiaBan = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HinhAnh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhanTramGiam = table.Column<int>(type: "int", nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Combo", x => x.MaCombo);
                });

            migrationBuilder.CreateTable(
                name: "IceLevel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDa = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__IceLevel__3214EC07B8328FFD", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KhachHang",
                columns: table => new
                {
                    MaKH = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DienThoai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    DiaChi = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    MatKhau = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__KhachHan__2725CF1E040423DE", x => x.MaKH);
                });

            migrationBuilder.CreateTable(
                name: "LoaiSanPham",
                columns: table => new
                {
                    MaLoai = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenLoai = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__LoaiSanP__730A57592E50CAAC", x => x.MaLoai);
                });

            migrationBuilder.CreateTable(
                name: "NhanVien",
                columns: table => new
                {
                    MaNV = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DienThoai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    TenDangNhap = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MatKhau = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ChucVu = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__NhanVien__2725D70A71DD95D8", x => x.MaNV);
                });

            migrationBuilder.CreateTable(
                name: "ProductTopping",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenTopping = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    GiaThem = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    HinhAnh = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ProductT__3214EC0794F0657D", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SugarLevel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDuong = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SugarLev__3214EC074600FA23", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TuyChonMon",
                columns: table => new
                {
                    MaTuyChon = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenTuyChon = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BatBuoc = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    ChonToiDa = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    ThuTu = table.Column<int>(type: "int", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TuyChonM__32F262619597F393", x => x.MaTuyChon);
                });

            migrationBuilder.CreateTable(
                name: "Voucher",
                columns: table => new
                {
                    MaVoucher = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    TenVoucher = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LoaiGiam = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    GiaTriGiam = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DonToiThieu = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NgayHetHan = table.Column<DateTime>(type: "datetime", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Voucher", x => x.MaVoucher);
                });

            migrationBuilder.CreateTable(
                name: "HoaDon",
                columns: table => new
                {
                    MaHD = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaKH = table.Column<int>(type: "int", nullable: false),
                    NgayDat = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    TongTien = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__HoaDon__2725A6E0D01897CE", x => x.MaHD);
                    table.ForeignKey(
                        name: "FK_HoaDon_KhachHang",
                        column: x => x.MaKH,
                        principalTable: "KhachHang",
                        principalColumn: "MaKH");
                });

            migrationBuilder.CreateTable(
                name: "ThanhVien",
                columns: table => new
                {
                    MaTV = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaKH = table.Column<int>(type: "int", nullable: false),
                    MaThanhVien = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Diem = table.Column<int>(type: "int", nullable: false),
                    HangThanhVien = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NgayThamGia = table.Column<DateTime>(type: "datetime", nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThanhVien", x => x.MaTV);
                    table.ForeignKey(
                        name: "FK_ThanhVien_KhachHang_MaKH",
                        column: x => x.MaKH,
                        principalTable: "KhachHang",
                        principalColumn: "MaKH");
                });

            migrationBuilder.CreateTable(
                name: "SanPham",
                columns: table => new
                {
                    MaSP = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenSP = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DonGia = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HinhAnh = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaLoai = table.Column<int>(type: "int", nullable: false),
                    GiaGoc = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    GiaKhuyenMai = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PhanTramGiam = table.Column<int>(type: "int", nullable: true),
                    DangKhuyenMai = table.Column<bool>(type: "bit", nullable: false),
                    NgayBatDau = table.Column<DateTime>(type: "datetime", nullable: true),
                    NgayKetThuc = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SanPham__2725081C8E8DB98B", x => x.MaSP);
                    table.ForeignKey(
                        name: "FK_SanPham_LoaiSanPham",
                        column: x => x.MaLoai,
                        principalTable: "LoaiSanPham",
                        principalColumn: "MaLoai");
                });

            migrationBuilder.CreateTable(
                name: "LoaiSanPham_TuyChonMon",
                columns: table => new
                {
                    MaLoai = table.Column<int>(type: "int", nullable: false),
                    MaTuyChon = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__LoaiSanP__7025717FBDE273E6", x => new { x.MaLoai, x.MaTuyChon });
                    table.ForeignKey(
                        name: "FK_LSP_TCM_LoaiSanPham",
                        column: x => x.MaLoai,
                        principalTable: "LoaiSanPham",
                        principalColumn: "MaLoai");
                    table.ForeignKey(
                        name: "FK_LSP_TCM_TuyChonMon",
                        column: x => x.MaTuyChon,
                        principalTable: "TuyChonMon",
                        principalColumn: "MaTuyChon");
                });

            migrationBuilder.CreateTable(
                name: "LuaChonMon",
                columns: table => new
                {
                    MaLuaChon = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaTuyChon = table.Column<int>(type: "int", nullable: false),
                    TenLuaChon = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    GiaThem = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    ThuTu = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__LuaChonM__99B0F9F8B9DAF7FE", x => x.MaLuaChon);
                    table.ForeignKey(
                        name: "FK_LuaChonMon_TuyChonMon",
                        column: x => x.MaTuyChon,
                        principalTable: "TuyChonMon",
                        principalColumn: "MaTuyChon");
                });

            migrationBuilder.CreateTable(
                name: "KhachHangVoucher",
                columns: table => new
                {
                    MaKHVoucher = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaKH = table.Column<int>(type: "int", nullable: false),
                    MaVoucher = table.Column<int>(type: "int", nullable: false),
                    NgayNhan = table.Column<DateTime>(type: "datetime", nullable: false),
                    NgaySuDung = table.Column<DateTime>(type: "datetime", nullable: true),
                    DaSuDung = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KhachHangVoucher", x => x.MaKHVoucher);
                    table.ForeignKey(
                        name: "FK_KhachHangVoucher_KhachHang_MaKH",
                        column: x => x.MaKH,
                        principalTable: "KhachHang",
                        principalColumn: "MaKH");
                    table.ForeignKey(
                        name: "FK_KhachHangVoucher_Voucher_MaVoucher",
                        column: x => x.MaVoucher,
                        principalTable: "Voucher",
                        principalColumn: "MaVoucher");
                });

            migrationBuilder.CreateTable(
                name: "ChiTietCombo",
                columns: table => new
                {
                    MaCombo = table.Column<int>(type: "int", nullable: false),
                    MaSanPham = table.Column<int>(type: "int", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietCombo", x => new { x.MaCombo, x.MaSanPham });
                    table.ForeignKey(
                        name: "FK_ChiTietCombo_Combo_MaCombo",
                        column: x => x.MaCombo,
                        principalTable: "Combo",
                        principalColumn: "MaCombo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChiTietCombo_SanPham_MaSanPham",
                        column: x => x.MaSanPham,
                        principalTable: "SanPham",
                        principalColumn: "MaSP",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChiTietHoaDon",
                columns: table => new
                {
                    MaCTHD = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaHD = table.Column<int>(type: "int", nullable: false),
                    MaSP = table.Column<int>(type: "int", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    DonGia = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ChiTietH__1E4FA7716BC7F702", x => x.MaCTHD);
                    table.ForeignKey(
                        name: "FK_CTHD_HoaDon",
                        column: x => x.MaHD,
                        principalTable: "HoaDon",
                        principalColumn: "MaHD");
                    table.ForeignKey(
                        name: "FK_CTHD_SanPham",
                        column: x => x.MaSP,
                        principalTable: "SanPham",
                        principalColumn: "MaSP");
                });

            migrationBuilder.CreateTable(
                name: "ProductOption",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaSP = table.Column<int>(type: "int", nullable: false),
                    AllowSize = table.Column<bool>(type: "bit", nullable: false),
                    AllowIce = table.Column<bool>(type: "bit", nullable: false),
                    AllowSugar = table.Column<bool>(type: "bit", nullable: false),
                    AllowTopping = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ProductO__3214EC079A2771EA", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductOption_SanPham",
                        column: x => x.MaSP,
                        principalTable: "SanPham",
                        principalColumn: "MaSP");
                });

            migrationBuilder.CreateTable(
                name: "ProductProductTopping",
                columns: table => new
                {
                    MaSP = table.Column<int>(type: "int", nullable: false),
                    ToppingId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ProductP__69C524D4DF9D211F", x => new { x.MaSP, x.ToppingId });
                    table.ForeignKey(
                        name: "FK_ProductProductTopping_Product",
                        column: x => x.MaSP,
                        principalTable: "SanPham",
                        principalColumn: "MaSP");
                    table.ForeignKey(
                        name: "FK_ProductProductTopping_Topping",
                        column: x => x.ToppingId,
                        principalTable: "ProductTopping",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProductSize",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaSP = table.Column<int>(type: "int", nullable: false),
                    TenSize = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    GiaThem = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    ThuTu = table.Column<int>(type: "int", nullable: true, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ProductS__3214EC0765051DA0", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductSize_SanPham",
                        column: x => x.MaSP,
                        principalTable: "SanPham",
                        principalColumn: "MaSP");
                });

            migrationBuilder.CreateTable(
                name: "ChiTietHoaDon_LuaChon",
                columns: table => new
                {
                    MaCTHD = table.Column<int>(type: "int", nullable: false),
                    MaLuaChon = table.Column<int>(type: "int", nullable: false),
                    GiaThem = table.Column<decimal>(type: "decimal(18,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ChiTietH__87D4A8EE2901ACB1", x => new { x.MaCTHD, x.MaLuaChon });
                    table.ForeignKey(
                        name: "FK_CTHDLC_CTHD",
                        column: x => x.MaCTHD,
                        principalTable: "ChiTietHoaDon",
                        principalColumn: "MaCTHD");
                    table.ForeignKey(
                        name: "FK_CTHDLC_LCM",
                        column: x => x.MaLuaChon,
                        principalTable: "LuaChonMon",
                        principalColumn: "MaLuaChon");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietCombo_MaSanPham",
                table: "ChiTietCombo",
                column: "MaSanPham");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietHoaDon_MaHD",
                table: "ChiTietHoaDon",
                column: "MaHD");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietHoaDon_MaSP",
                table: "ChiTietHoaDon",
                column: "MaSP");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietHoaDon_LuaChon_MaLuaChon",
                table: "ChiTietHoaDon_LuaChon",
                column: "MaLuaChon");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDon_MaKH",
                table: "HoaDon",
                column: "MaKH");

            migrationBuilder.CreateIndex(
                name: "IX_KhachHangVoucher_MaKH",
                table: "KhachHangVoucher",
                column: "MaKH");

            migrationBuilder.CreateIndex(
                name: "IX_KhachHangVoucher_MaVoucher",
                table: "KhachHangVoucher",
                column: "MaVoucher");

            migrationBuilder.CreateIndex(
                name: "IX_LoaiSanPham_TuyChonMon_MaTuyChon",
                table: "LoaiSanPham_TuyChonMon",
                column: "MaTuyChon");

            migrationBuilder.CreateIndex(
                name: "IX_LuaChonMon_MaTuyChon",
                table: "LuaChonMon",
                column: "MaTuyChon");

            migrationBuilder.CreateIndex(
                name: "IX_ProductOption_MaSP",
                table: "ProductOption",
                column: "MaSP");

            migrationBuilder.CreateIndex(
                name: "IX_ProductProductTopping_ToppingId",
                table: "ProductProductTopping",
                column: "ToppingId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSize_MaSP",
                table: "ProductSize",
                column: "MaSP");

            migrationBuilder.CreateIndex(
                name: "IX_SanPham_MaLoai",
                table: "SanPham",
                column: "MaLoai");

            migrationBuilder.CreateIndex(
                name: "IX_ThanhVien_MaKH",
                table: "ThanhVien",
                column: "MaKH");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChiTietCombo");

            migrationBuilder.DropTable(
                name: "ChiTietHoaDon_LuaChon");

            migrationBuilder.DropTable(
                name: "IceLevel");

            migrationBuilder.DropTable(
                name: "KhachHangVoucher");

            migrationBuilder.DropTable(
                name: "LoaiSanPham_TuyChonMon");

            migrationBuilder.DropTable(
                name: "NhanVien");

            migrationBuilder.DropTable(
                name: "ProductOption");

            migrationBuilder.DropTable(
                name: "ProductProductTopping");

            migrationBuilder.DropTable(
                name: "ProductSize");

            migrationBuilder.DropTable(
                name: "SugarLevel");

            migrationBuilder.DropTable(
                name: "ThanhVien");

            migrationBuilder.DropTable(
                name: "Combo");

            migrationBuilder.DropTable(
                name: "ChiTietHoaDon");

            migrationBuilder.DropTable(
                name: "LuaChonMon");

            migrationBuilder.DropTable(
                name: "Voucher");

            migrationBuilder.DropTable(
                name: "ProductTopping");

            migrationBuilder.DropTable(
                name: "HoaDon");

            migrationBuilder.DropTable(
                name: "SanPham");

            migrationBuilder.DropTable(
                name: "TuyChonMon");

            migrationBuilder.DropTable(
                name: "KhachHang");

            migrationBuilder.DropTable(
                name: "LoaiSanPham");
        }
    }
}
