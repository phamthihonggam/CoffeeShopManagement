using System;
using System.Collections.Generic;
using CoffeeShopManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShopManagement.Data;

public partial class CoffeeShopDbContext : DbContext
{
    public CoffeeShopDbContext(
        DbContextOptions<CoffeeShopDbContext> options)
        : base(options)
    {
    }


    // =====================================================
    // DBSET
    // =====================================================

    public virtual DbSet<ChiTietHoaDon>
        ChiTietHoaDons
    { get; set; }

    public virtual DbSet<DanhGiaSanPham>
    DanhGiaSanPhams
    { get; set; }

    public virtual DbSet<DanhGiaCombo>
        DanhGiaCombos
    { get; set; }

    public virtual DbSet<ChiTietHoaDonLuaChon>
        ChiTietHoaDonLuaChons
    { get; set; }

    public virtual DbSet<HoaDon>
        HoaDons
    { get; set; }

    public virtual DbSet<IceLevel>
        IceLevels
    { get; set; }

    public virtual DbSet<KhachHang>
        KhachHangs
    { get; set; }

    public virtual DbSet<LoaiSanPham>
        LoaiSanPhams
    { get; set; }

    public virtual DbSet<LuaChonMon>
        LuaChonMons
    { get; set; }

    public virtual DbSet<NhanVien>
        NhanViens
    { get; set; }

    public virtual DbSet<ProductOption>
        ProductOptions
    { get; set; }

    public virtual DbSet<ProductSize>
        ProductSizes
    { get; set; }

    public virtual DbSet<ProductTopping>
        ProductToppings
    { get; set; }

    public virtual DbSet<SanPham>
        SanPhams
    { get; set; }

    public virtual DbSet<SugarLevel>
        SugarLevels
    { get; set; }

    public virtual DbSet<TuyChonMon>
        TuyChonMons
    { get; set; }

    public virtual DbSet<Combo>
        Combos
    { get; set; }

    public virtual DbSet<ChiTietCombo>
        ChiTietCombos
    { get; set; }

    public virtual DbSet<ThanhVien>
        ThanhViens
    { get; set; }

    public virtual DbSet<Voucher>
        Vouchers
    { get; set; }

    public virtual DbSet<KhachHangVoucher>
        KhachHangVouchers
    { get; set; }

    public virtual DbSet<ChiNhanh>
        ChiNhanhs
    { get; set; }

    public virtual DbSet<TaiKhoan> TaiKhoans { get; set; }

    public virtual DbSet<VaiTro> VaiTros { get; set; }

    public virtual DbSet<Quyen> Quyens { get; set; }

    // =====================================================
    // MODEL CONFIGURATION
    // =====================================================

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {

        // =================================================
        // CHI TIẾT HÓA ĐƠN
        // =================================================

        modelBuilder.Entity<ChiTietHoaDon>(entity =>
        {
            entity.HasKey(e => e.MaCthd)
                .HasName(
                    "PK__ChiTietH__1E4FA7716BC7F702"
                );

            entity.ToTable("ChiTietHoaDon");


            entity.Property(e => e.MaCthd)
                .HasColumnName("MaCTHD");

            entity.Property(e => e.MaHd)
                .HasColumnName("MaHD");

            entity.Property(e => e.MaSp)
                .HasColumnName("MaSP");

            entity.Property(e => e.DonGia)
                .HasColumnType("decimal(18, 2)");


            // HÓA ĐƠN

            entity.HasOne(
                    d => d.MaHdNavigation
                )
                .WithMany(
                    p => p.ChiTietHoaDons
                )
                .HasForeignKey(
                    d => d.MaHd
                )
                .OnDelete(
                    DeleteBehavior.ClientSetNull
                )
                .HasConstraintName(
                    "FK_CTHD_HoaDon"
                );


            // SẢN PHẨM

            entity.HasOne(
                    d => d.MaSpNavigation
                )
                .WithMany(
                    p => p.ChiTietHoaDons
                )
                .HasForeignKey(
                    d => d.MaSp
                )
                .HasConstraintName(
                    "FK_CTHD_SanPham"
                );


            // COMBO

            entity.HasOne(
                    d => d.MaComboNavigation
                )
                .WithMany()
                .HasForeignKey(
                    d => d.MaCombo
                )
                .HasConstraintName(
                    "FK_ChiTietHoaDon_Combo"
                );
        });


        // =================================================
        // CHI TIẾT HÓA ĐƠN - LỰA CHỌN
        // =================================================

        modelBuilder.Entity<ChiTietHoaDonLuaChon>(
            entity =>
            {
                entity.HasKey(
                    e => new
                    {
                        e.MaCthd,
                        e.MaLuaChon
                    }
                )
                .HasName(
                    "PK__ChiTietH__87D4A8EE2901ACB1"
                );


                entity.ToTable(
                    "ChiTietHoaDon_LuaChon"
                );


                entity.Property(e => e.MaCthd)
                    .HasColumnName("MaCTHD");

                entity.Property(e => e.GiaThem)
                    .HasColumnType("decimal(18, 0)");


                entity.HasOne(
                        d => d.MaCthdNavigation
                    )
                    .WithMany(
                        p => p.ChiTietHoaDonLuaChons
                    )
                    .HasForeignKey(
                        d => d.MaCthd
                    )
                    .OnDelete(
                        DeleteBehavior.ClientSetNull
                    )
                    .HasConstraintName(
                        "FK_CTHDLC_CTHD"
                    );


                entity.HasOne(
                        d => d.MaLuaChonNavigation
                    )
                    .WithMany(
                        p => p.ChiTietHoaDonLuaChons
                    )
                    .HasForeignKey(
                        d => d.MaLuaChon
                    )
                    .OnDelete(
                        DeleteBehavior.ClientSetNull
                    )
                    .HasConstraintName(
                        "FK_CTHDLC_LCM"
                    );
            });


        // =================================================
        // HÓA ĐƠN
        // =================================================

        modelBuilder.Entity<HoaDon>(
            entity =>
            {
                // =========================================
                // PRIMARY KEY
                // =========================================

                entity.HasKey(
                    e => e.MaHd
                )
                .HasName(
                    "PK__HoaDon__2725A6E0D01897CE"
                );


                entity.ToTable(
                    "HoaDon"
                );


                // =========================================
                // MÃ HÓA ĐƠN
                // =========================================

                entity.Property(
                    e => e.MaHd
                )
                .HasColumnName(
                    "MaHD"
                );


                // =========================================
                // KHÁCH HÀNG
                // =========================================

                entity.Property(
                    e => e.MaKh
                )
                .HasColumnName(
                    "MaKH"
                );


                // =========================================
                // NGÀY ĐẶT
                // =========================================

                entity.Property(
                    e => e.NgayDat
                )
                .HasDefaultValueSql(
                    "(getdate())"
                )
                .HasColumnType(
                    "datetime"
                );


                // =========================================
                // TIỀN
                // =========================================

                entity.Property(
                    e => e.TamTinh
                )
                .HasColumnType(
                    "decimal(18, 2)"
                );


                entity.Property(
                    e => e.GiamGia
                )
                .HasColumnType(
                    "decimal(18, 2)"
                );


                entity.Property(
                    e => e.PhiGiaoHang
                )
                .HasColumnType(
                    "decimal(18, 2)"
                );


                entity.Property(
                    e => e.TongTien
                )
                .HasColumnType(
                    "decimal(18, 2)"
                );


                // =========================================
                // TRẠNG THÁI ĐƠN HÀNG
                // =========================================

                entity.Property(
                    e => e.TrangThai
                )
                .HasMaxLength(
                    100
                );



                // =========================================
                // THÔNG TIN GIAO HÀNG CỦA ĐƠN
                // =========================================

                entity.Property(
                    e => e.HoTenNguoiNhan
                )
                .HasMaxLength(
                    150
                );


                entity.Property(
                    e => e.DienThoaiNguoiNhan
                )
                .HasMaxLength(
                    20
                );


                entity.Property(
                    e => e.DiaChiGiaoHang
                )
                .HasMaxLength(
                    500
                );


                entity.Property(
                    e => e.QuanHuyenGiaoHang
                )
                .HasMaxLength(
                    100
                );


                // =========================================
                // THANH TOÁN
                // =========================================

                entity.Property(
                    e => e.PhuongThucThanhToan
                )
                .HasMaxLength(
                    20
                );


                // =========================================
                // HỦY ĐƠN
                // =========================================

                entity.Property(
                    e => e.LyDoHuy
                )
                .HasMaxLength(
                    300
                );


                entity.Property(
                    e => e.GhiChuHuy
                )
                .HasMaxLength(
                    1000
                );


                entity.Property(
                    e => e.NgayYeuCauHuy
                )
                .HasColumnType(
                    "datetime"
                );


                entity.Property(
                    e => e.NgayXuLyHuy
                )
                .HasColumnType(
                    "datetime"
                );


                // =========================================
                // HOÀN TIỀN
                // =========================================

                entity.Property(
                    e => e.TrangThaiHoanTien
                )
                .HasMaxLength(
                    100
                );


                entity.Property(
                    e => e.NgayHoanTien
                )
                .HasColumnType(
                    "datetime"
                );


                entity.Property(
                    e => e.SoTienHoan
                )
                .HasColumnType(
                    "decimal(18, 2)"
                );



                // =========================================
                // GIAO HÀNG
                // =========================================

                entity.Property(
                    e => e.MaTaiKhoanGiao
                )
                .HasColumnName(
                    "MaTaiKhoanGiao"
                );


                entity.Property(
                    e => e.NgayNhanGiao
                )
                .HasColumnType(
                    "datetime"
                );


                entity.Property(
                    e => e.NgayBatDauGiao
                )
                .HasColumnType(
                    "datetime"
                );


                entity.Property(
                    e => e.NgayGiaoThanhCong
                )
                .HasColumnType(
                    "datetime"
                );


                entity.Property(
                    e => e.LyDoGiaoThatBai
                )
                .HasMaxLength(
                    500
                );


                // =========================================
                // FOREIGN KEY - KHÁCH HÀNG
                // =========================================

                entity.HasOne(
                    d => d.MaKhNavigation
                )
                .WithMany(
                    p => p.HoaDons
                )
                .HasForeignKey(
                    d => d.MaKh
                )
                .OnDelete(
                    DeleteBehavior.ClientSetNull
                )
                .HasConstraintName(
                    "FK_HoaDon_KhachHang"
                );


                // =========================================
                // FOREIGN KEY - NHÂN VIÊN GIAO HÀNG
                // =========================================

                entity.HasOne(
                    d => d.MaTaiKhoanGiaoNavigation
                )
                .WithMany()
                .HasForeignKey(
                    d => d.MaTaiKhoanGiao
                )
                .OnDelete(
                    DeleteBehavior.SetNull
                )
                .HasConstraintName(
                    "FK_HoaDon_TaiKhoanGiao"
                );
            }
        );


        // =================================================
        // ĐÁNH GIÁ SẢN PHẨM
        // =================================================

        modelBuilder.Entity<DanhGiaSanPham>(
            entity =>
            {
                entity.HasKey(
                    e => e.MaDanhGia
                );


                entity.ToTable(
                    "DanhGiaSanPham"
                );


                entity.Property(
                    e => e.MaDanhGia
                )
                .HasColumnName(
                    "MaDanhGia"
                );


                entity.Property(
                    e => e.MaKh
                )
                .HasColumnName(
                    "MaKH"
                );


                entity.Property(
                    e => e.MaSp
                )
                .HasColumnName(
                    "MaSP"
                );


                entity.Property(
                    e => e.MaHd
                )
                .HasColumnName(
                    "MaHD"
                );


                entity.Property(
                    e => e.SoSao
                )
                .HasColumnName(
                    "SoSao"
                );


                entity.Property(
                    e => e.NoiDung
                )
                .HasMaxLength(
                    1000
                );


                entity.Property(
                    e => e.NgayDanhGia
                )
                .HasColumnType(
                    "datetime"
                )
                .HasDefaultValueSql(
                    "(getdate())"
                );


                // =========================================
                // KHÁCH HÀNG
                // =========================================

                entity.HasOne(
                    d => d.MaKhNavigation
                )
                .WithMany()
                .HasForeignKey(
                    d => d.MaKh
                )
                .OnDelete(
                    DeleteBehavior.ClientSetNull
                )
                .HasConstraintName(
                    "FK_DanhGiaSanPham_KhachHang"
                );


                // =========================================
                // SẢN PHẨM
                // =========================================

                entity.HasOne(
                    d => d.MaSpNavigation
                )
                .WithMany()
                .HasForeignKey(
                    d => d.MaSp
                )
                .OnDelete(
                    DeleteBehavior.ClientSetNull
                )
                .HasConstraintName(
                    "FK_DanhGiaSanPham_SanPham"
                );


                // =========================================
                // HÓA ĐƠN
                // =========================================

                entity.HasOne(
                    d => d.MaHdNavigation
                )
                .WithMany()
                .HasForeignKey(
                    d => d.MaHd
                )
                .OnDelete(
                    DeleteBehavior.ClientSetNull
                )
                .HasConstraintName(
                    "FK_DanhGiaSanPham_HoaDon"
                );


                // =========================================
                // UNIQUE
                // Mỗi khách chỉ đánh giá 1 lần
                // cho 1 sản phẩm trong 1 hóa đơn
                // =========================================

                entity.HasIndex(
                    e => new
                    {
                        e.MaKh,
                        e.MaSp,
                        e.MaHd
                    }
                )
                .IsUnique()
                .HasDatabaseName(
                    "UQ_DanhGiaSanPham"
                );
            }
        );

        // =================================================
        // ĐÁNH GIÁ COMBO
        // =================================================

        modelBuilder.Entity<DanhGiaCombo>(
            entity =>
            {
                entity.HasKey(
                    e => e.MaDanhGia
                );


                entity.ToTable(
                    "DanhGiaCombo"
                );


                entity.Property(
                    e => e.MaDanhGia
                )
                .HasColumnName(
                    "MaDanhGia"
                );


                entity.Property(
                    e => e.MaKh
                )
                .HasColumnName(
                    "MaKH"
                );


                entity.Property(
                    e => e.MaCombo
                )
                .HasColumnName(
                    "MaCombo"
                );


                entity.Property(
                    e => e.MaHd
                )
                .HasColumnName(
                    "MaHD"
                );


                entity.Property(
                    e => e.SoSao
                )
                .HasColumnName(
                    "SoSao"
                );


                entity.Property(
                    e => e.NoiDung
                )
                .HasMaxLength(
                    1000
                );


                entity.Property(
                    e => e.NgayDanhGia
                )
                .HasColumnType(
                    "datetime"
                )
                .HasDefaultValueSql(
                    "(getdate())"
                );


                // =========================================
                // KHÁCH HÀNG
                // =========================================

                entity.HasOne(
                    d => d.MaKhNavigation
                )
                .WithMany()
                .HasForeignKey(
                    d => d.MaKh
                )
                .OnDelete(
                    DeleteBehavior.ClientSetNull
                )
                .HasConstraintName(
                    "FK_DanhGiaCombo_KhachHang"
                );


                // =========================================
                // COMBO
                // =========================================

                entity.HasOne(
                    d => d.MaComboNavigation
                )
                .WithMany()
                .HasForeignKey(
                    d => d.MaCombo
                )
                .OnDelete(
                    DeleteBehavior.ClientSetNull
                )
                .HasConstraintName(
                    "FK_DanhGiaCombo_Combo"
                );


                // =========================================
                // HÓA ĐƠN
                // =========================================

                entity.HasOne(
                    d => d.MaHdNavigation
                )
                .WithMany()
                .HasForeignKey(
                    d => d.MaHd
                )
                .OnDelete(
                    DeleteBehavior.ClientSetNull
                )
                .HasConstraintName(
                    "FK_DanhGiaCombo_HoaDon"
                );


                // =========================================
                // UNIQUE
                // Mỗi khách chỉ đánh giá 1 lần
                // cho 1 combo trong 1 hóa đơn
                // =========================================

                entity.HasIndex(
                    e => new
                    {
                        e.MaKh,
                        e.MaCombo,
                        e.MaHd
                    }
                )
                .IsUnique()
                .HasDatabaseName(
                    "UQ_DanhGiaCombo"
                );
            }
        );


        // =================================================
        // ICE LEVEL
        // =================================================

        modelBuilder.Entity<IceLevel>(entity =>
        {
            entity.HasKey(e => e.Id)
                .HasName(
                    "PK__IceLevel__3214EC07B8328FFD"
                );

            entity.ToTable("IceLevel");

            entity.Property(e => e.TenDa)
                .HasMaxLength(50);
        });


        // =================================================
        // KHÁCH HÀNG
        // =================================================

        modelBuilder.Entity<KhachHang>(entity =>
        {
            entity.HasKey(e => e.MaKh)
                .HasName(
                    "PK__KhachHan__2725CF1E040423DE"
                );


            entity.ToTable("KhachHang");


            entity.Property(e => e.MaKh)
                .HasColumnName("MaKH");

            entity.Property(e => e.DiaChi)
                .HasMaxLength(255);

            entity.Property(e => e.DienThoai)
                .HasMaxLength(20);

            entity.Property(e => e.Email)
                .HasMaxLength(100);

            entity.Property(e => e.HoTen)
                .HasMaxLength(100);
        });


        // =================================================
        // LOẠI SẢN PHẨM
        // =================================================

        modelBuilder.Entity<LoaiSanPham>(
            entity =>
            {
                entity.HasKey(e => e.MaLoai)
                    .HasName(
                        "PK__LoaiSanP__730A57592E50CAAC"
                    );


                entity.ToTable("LoaiSanPham");


                entity.Property(e => e.TenLoai)
                    .HasMaxLength(100);


                entity.HasMany(
                        d => d.MaTuyChons
                    )
                    .WithMany(
                        p => p.MaLoais
                    )

                    .UsingEntity<
                        Dictionary<string, object>
                    >(

                        "LoaiSanPhamTuyChonMon",

                        r => r
                            .HasOne<TuyChonMon>()
                            .WithMany()
                            .HasForeignKey(
                                "MaTuyChon"
                            )
                            .OnDelete(
                                DeleteBehavior.ClientSetNull
                            )
                            .HasConstraintName(
                                "FK_LSP_TCM_TuyChonMon"
                            ),

                        l => l
                            .HasOne<LoaiSanPham>()
                            .WithMany()
                            .HasForeignKey(
                                "MaLoai"
                            )
                            .OnDelete(
                                DeleteBehavior.ClientSetNull
                            )
                            .HasConstraintName(
                                "FK_LSP_TCM_LoaiSanPham"
                            ),

                        j =>
                        {
                            j.HasKey(
                                "MaLoai",
                                "MaTuyChon"
                            )
                            .HasName(
                                "PK__LoaiSanP__7025717FBDE273E6"
                            );

                            j.ToTable(
                                "LoaiSanPham_TuyChonMon"
                            );
                        }
                    );
            });


        // =================================================
        // LỰA CHỌN MÓN
        // =================================================

        modelBuilder.Entity<LuaChonMon>(
            entity =>
            {
                entity.HasKey(
                        e => e.MaLuaChon
                    )
                    .HasName(
                        "PK__LuaChonM__99B0F9F8B9DAF7FE"
                    );


                entity.ToTable(
                    "LuaChonMon"
                );


                entity.Property(
                        e => e.GiaThem
                    )
                    .HasColumnType(
                        "decimal(18, 0)"
                    );

                entity.Property(
                        e => e.TenLuaChon
                    )
                    .HasMaxLength(100);

                entity.Property(
                        e => e.ThuTu
                    )
                    .HasDefaultValue(1);

                entity.Property(
                        e => e.TrangThai
                    )
                    .HasDefaultValue(true);


                entity.HasOne(
                        d => d.MaTuyChonNavigation
                    )
                    .WithMany(
                        p => p.LuaChonMons
                    )
                    .HasForeignKey(
                        d => d.MaTuyChon
                    )
                    .OnDelete(
                        DeleteBehavior.ClientSetNull
                    )
                    .HasConstraintName(
                        "FK_LuaChonMon_TuyChonMon"
                    );
            });


        // =================================================
        // NHÂN VIÊN
        // =================================================

        modelBuilder.Entity<NhanVien>(
            entity =>
            {
                entity.HasKey(
                        e => e.MaNv
                    )
                    .HasName(
                        "PK__NhanVien__2725D70A71DD95D8"
                    );


                entity.ToTable(
                    "NhanVien"
                );


                entity.Property(
                        e => e.MaNv
                    )
                    .HasColumnName(
                        "MaNV"
                    );

                entity.Property(
                        e => e.ChucVu
                    )
                    .HasMaxLength(50);

                entity.Property(
                        e => e.DienThoai
                    )
                    .HasMaxLength(20);

                entity.Property(
                        e => e.Email
                    )
                    .HasMaxLength(100);

                entity.Property(
                        e => e.HoTen
                    )
                    .HasMaxLength(100);

                entity.Property(
                        e => e.MatKhau
                    )
                    .HasMaxLength(100);

                entity.Property(
                        e => e.TenDangNhap
                    )
                    .HasMaxLength(50);
            });


        // =================================================
        // PRODUCT OPTION
        // =================================================

        modelBuilder.Entity<ProductOption>(
            entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName(
                        "PK__ProductO__3214EC079A2771EA"
                    );


                entity.ToTable(
                    "ProductOption"
                );


                entity.Property(e => e.MaSp)
                    .HasColumnName(
                        "MaSP"
                    );


                entity.HasOne(
                        d => d.MaSpNavigation
                    )
                    .WithMany(
                        p => p.ProductOptions
                    )
                    .HasForeignKey(
                        d => d.MaSp
                    )
                    .OnDelete(
                        DeleteBehavior.ClientSetNull
                    )
                    .HasConstraintName(
                        "FK_ProductOption_SanPham"
                    );
            });


        // =================================================
        // PRODUCT SIZE
        // =================================================

        modelBuilder.Entity<ProductSize>(
            entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName(
                        "PK__ProductS__3214EC0765051DA0"
                    );


                entity.ToTable(
                    "ProductSize"
                );


                entity.Property(
                        e => e.GiaThem
                    )
                    .HasColumnType(
                        "decimal(18, 0)"
                    );

                entity.Property(
                        e => e.MaSp
                    )
                    .HasColumnName(
                        "MaSP"
                    );

                entity.Property(
                        e => e.TenSize
                    )
                    .HasMaxLength(20);

                entity.Property(
                        e => e.ThuTu
                    )
                    .HasDefaultValue(1);


                entity.HasOne(
                        d => d.MaSpNavigation
                    )
                    .WithMany(
                        p => p.ProductSizes
                    )
                    .HasForeignKey(
                        d => d.MaSp
                    )
                    .OnDelete(
                        DeleteBehavior.ClientSetNull
                    )
                    .HasConstraintName(
                        "FK_ProductSize_SanPham"
                    );
            });


        // =================================================
        // PRODUCT TOPPING
        // =================================================

        modelBuilder.Entity<ProductTopping>(
            entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName(
                        "PK__ProductT__3214EC0794F0657D"
                    );


                entity.ToTable(
                    "ProductTopping"
                );


                entity.Property(
                        e => e.GiaThem
                    )
                    .HasColumnType(
                        "decimal(18, 0)"
                    );

                entity.Property(
                        e => e.HinhAnh
                    )
                    .HasMaxLength(255);

                entity.Property(
                        e => e.IsActive
                    )
                    .HasDefaultValue(true);

                entity.Property(
                        e => e.TenTopping
                    )
                    .HasMaxLength(100);
            });


        // =================================================
        // SẢN PHẨM
        // =================================================

        modelBuilder.Entity<SanPham>(
            entity =>
            {
                entity.HasKey(e => e.MaSp)
                    .HasName(
                        "PK__SanPham__2725081C8E8DB98B"
                    );


                entity.ToTable(
                    "SanPham"
                );


                entity.Property(
                        e => e.MaSp
                    )
                    .HasColumnName(
                        "MaSP"
                    );

                entity.Property(
                        e => e.DonGia
                    )
                    .HasColumnType(
                        "decimal(18, 2)"
                    );

                entity.Property(
                        e => e.GiaGoc
                    )
                    .HasColumnType(
                        "decimal(18, 2)"
                    );

                entity.Property(
                        e => e.GiaKhuyenMai
                    )
                    .HasColumnType(
                        "decimal(18, 2)"
                    );

                entity.Property(
                        e => e.HinhAnh
                    )
                    .HasMaxLength(500);

                entity.Property(
                        e => e.NgayBatDau
                    )
                    .HasColumnType(
                        "datetime"
                    );

                entity.Property(
                        e => e.NgayKetThuc
                    )
                    .HasColumnType(
                        "datetime"
                    );

                entity.Property(
                        e => e.TenSp
                    )
                    .HasMaxLength(200)
                    .HasColumnName(
                        "TenSP"
                    );


                entity.HasOne(
                        d => d.MaLoaiNavigation
                    )
                    .WithMany(
                        p => p.SanPhams
                    )
                    .HasForeignKey(
                        d => d.MaLoai
                    )
                    .OnDelete(
                        DeleteBehavior.ClientSetNull
                    )
                    .HasConstraintName(
                        "FK_SanPham_LoaiSanPham"
                    );


                entity.HasMany(
                        d => d.Toppings
                    )
                    .WithMany(
                        p => p.MaSps
                    )

                    .UsingEntity<
                        Dictionary<string, object>
                    >(

                        "ProductProductTopping",

                        r => r
                            .HasOne<ProductTopping>()
                            .WithMany()
                            .HasForeignKey(
                                "ToppingId"
                            )
                            .OnDelete(
                                DeleteBehavior.ClientSetNull
                            )
                            .HasConstraintName(
                                "FK_ProductProductTopping_Topping"
                            ),

                        l => l
                            .HasOne<SanPham>()
                            .WithMany()
                            .HasForeignKey(
                                "MaSp"
                            )
                            .OnDelete(
                                DeleteBehavior.ClientSetNull
                            )
                            .HasConstraintName(
                                "FK_ProductProductTopping_Product"
                            ),

                        j =>
                        {
                            j.HasKey(
                                "MaSp",
                                "ToppingId"
                            )
                            .HasName(
                                "PK__ProductP__69C524D4DF9D211F"
                            );

                            j.ToTable(
                                "ProductProductTopping"
                            );

                            j.IndexerProperty<int>(
                                    "MaSp"
                                )
                                .HasColumnName(
                                    "MaSP"
                                );
                        }
                    );
            });


        // =================================================
        // SUGAR LEVEL
        // =================================================

        modelBuilder.Entity<SugarLevel>(
            entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName(
                        "PK__SugarLev__3214EC074600FA23"
                    );

                entity.ToTable(
                    "SugarLevel"
                );

                entity.Property(
                        e => e.TenDuong
                    )
                    .HasMaxLength(50);
            });


        // =================================================
        // TÙY CHỌN MÓN
        // =================================================

        modelBuilder.Entity<TuyChonMon>(
            entity =>
            {
                entity.HasKey(
                        e => e.MaTuyChon
                    )
                    .HasName(
                        "PK__TuyChonM__32F262619597F393"
                    );


                entity.ToTable(
                    "TuyChonMon"
                );


                entity.Property(
                        e => e.BatBuoc
                    )
                    .HasDefaultValue(true);

                entity.Property(
                        e => e.ChonToiDa
                    )
                    .HasDefaultValue(1);

                entity.Property(
                        e => e.TenTuyChon
                    )
                    .HasMaxLength(50);

                entity.Property(
                        e => e.ThuTu
                    )
                    .HasDefaultValue(1);
            });


        // =================================================
        // COMBO
        // =================================================

        modelBuilder.Entity<Combo>(
            entity =>
            {
                entity.HasKey(
                    e => e.MaCombo
                );


                entity.ToTable(
                    "Combo"
                );


                entity.Property(
                        e => e.GiaBan
                    )
                    .HasColumnType(
                        "decimal(18, 2)"
                    );

                entity.Property(
                        e => e.GiaGoc
                    )
                    .HasColumnType(
                        "decimal(18, 2)"
                    );
            });


        // =================================================
        // CHI TIẾT COMBO
        // ĐÚNG MODEL: Combo + SanPham
        // =================================================

        modelBuilder.Entity<ChiTietCombo>(
            entity =>
            {
                entity.HasKey(
                    e => new
                    {
                        e.MaCombo,
                        e.MaSanPham
                    }
                );


                entity.ToTable(
                    "ChiTietCombo"
                );


                entity.HasOne(
                        d => d.Combo
                    )
                    .WithMany(
                        p => p.ChiTietCombos
                    )
                    .HasForeignKey(
                        d => d.MaCombo
                    );


                entity.HasOne(
                        d => d.SanPham
                    )
                    .WithMany()
                    .HasForeignKey(
                        d => d.MaSanPham
                    );
            });


        // =================================================
        // THÀNH VIÊN
        // =================================================

        modelBuilder.Entity<ThanhVien>(
            entity =>
            {
                entity.HasKey(
                    e => e.MaTv
                );


                entity.ToTable(
                    "ThanhVien"
                );


                entity.Property(
                        e => e.MaTv
                    )
                    .HasColumnName(
                        "MaTV"
                    );

                entity.Property(
                        e => e.MaKh
                    )
                    .HasColumnName(
                        "MaKH"
                    );

                entity.Property(
                        e => e.MaThanhVien
                    )
                    .HasMaxLength(20);

                entity.Property(
                        e => e.HangThanhVien
                    )
                    .HasMaxLength(20);

                entity.Property(
                        e => e.NgayThamGia
                    )
                    .HasColumnType(
                        "datetime"
                    );


                entity.HasOne(
                        d => d.MaKhNavigation
                    )
                    .WithMany()
                    .HasForeignKey(
                        d => d.MaKh
                    )
                    .OnDelete(
                        DeleteBehavior.ClientSetNull
                    );
            });


        // =================================================
        // VOUCHER
        // =================================================

        modelBuilder.Entity<Voucher>(
            entity =>
            {
                entity.HasKey(
                    e => e.MaVoucher
                );


                entity.ToTable(
                    "Voucher"
                );


                entity.Property(
                        e => e.MaCode
                    )
                    .HasMaxLength(30);

                entity.Property(
                        e => e.TenVoucher
                    )
                    .HasMaxLength(100);

                entity.Property(
                        e => e.LoaiGiam
                    )
                    .HasMaxLength(20);

                entity.Property(
                        e => e.GiaTriGiam
                    )
                    .HasColumnType(
                        "decimal(18,2)"
                    );

                entity.Property(
                        e => e.DonToiThieu
                    )
                    .HasColumnType(
                        "decimal(18,2)"
                    );

                entity.Property(
                        e => e.NgayHetHan
                    )
                    .HasColumnType(
                        "datetime"
                    );

                entity.Property(
                        e => e.MoTa
                    )
                    .HasMaxLength(255);
            });


        // =================================================
        // KHÁCH HÀNG VOUCHER
        // ĐÚNG MODEL: MaKhVoucher
        // =================================================

        modelBuilder.Entity<KhachHangVoucher>(
            entity =>
            {
                entity.HasKey(
                    e => e.MaKhVoucher
                );


                entity.ToTable(
                    "KhachHangVoucher"
                );


                entity.Property(
                        e => e.MaKhVoucher
                    )
                    .HasColumnName(
                        "MaKHVoucher"
                    );


                entity.Property(
                        e => e.MaKh
                    )
                    .HasColumnName(
                        "MaKH"
                    );


                entity.Property(
                        e => e.NgayNhan
                    )
                    .HasColumnType(
                        "datetime"
                    );


                entity.Property(
                        e => e.NgaySuDung
                    )
                    .HasColumnType(
                        "datetime"
                    );


                entity.HasOne(
                        d => d.MaKhNavigation
                    )
                    .WithMany()
                    .HasForeignKey(
                        d => d.MaKh
                    )
                    .OnDelete(
                        DeleteBehavior.ClientSetNull
                    );


                entity.HasOne(
                        d => d.MaVoucherNavigation
                    )
                    .WithMany(
                        p => p.KhachHangVouchers
                    )
                    .HasForeignKey(
                        d => d.MaVoucher
                    )
                    .OnDelete(
                        DeleteBehavior.ClientSetNull
                    );
            });


        // =================================================
        // CHI NHÁNH
        // ĐÚNG MODEL: MaCN
        // =================================================

        modelBuilder.Entity<ChiNhanh>(
            entity =>
            {
                entity.HasKey(
                    e => e.MaCN
                );


                entity.ToTable(
                    "ChiNhanh"
                );


                entity.Property(
                        e => e.TenChiNhanh
                    )
                    .HasMaxLength(150);


                entity.Property(
                        e => e.DiaChi
                    )
                    .HasMaxLength(255);


                entity.Property(
                        e => e.Quan
                    )
                    .HasMaxLength(100);


                entity.Property(
                        e => e.ThanhPho
                    )
                    .HasMaxLength(100);


                entity.Property(
                        e => e.DienThoai
                    )
                    .HasMaxLength(20);


                entity.Property(
                        e => e.Email
                    )
                    .HasMaxLength(100);


                entity.Property(
                        e => e.GioMoCua
                    )
                    .HasMaxLength(100);


                entity.Property(
                        e => e.HinhAnh
                    )
                    .HasMaxLength(255);


                entity.Property(
                        e => e.GoogleMap
                    )
                    .HasMaxLength(500);


                entity.Property(
                        e => e.DanhGia
                    )
                    .HasColumnType(
                        "decimal(2,1)"
                    );
            });



        // =================================================
        // TÀI KHOẢN
        // =================================================

        modelBuilder.Entity<TaiKhoan>(entity =>
        {
            entity.HasKey(e => e.MaTaiKhoan);

            entity.ToTable("TaiKhoan");

            entity.HasIndex(e => e.TenDangNhap)
                .IsUnique();

            entity.Property(e => e.TenDangNhap)
                .HasMaxLength(100);

            entity.Property(e => e.MatKhau)
                .HasMaxLength(255);

            entity.Property(e => e.HoTen)
                .HasMaxLength(150);

            entity.Property(e => e.Email)
                .HasMaxLength(150);

            entity.Property(e => e.DienThoai)
                .HasMaxLength(20);

            entity.Property(e => e.IsActive)
                .HasDefaultValue(true);

            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.MaVaiTroNavigation)
                .WithMany(p => p.TaiKhoans)
                .HasForeignKey(d => d.MaVaiTro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TaiKhoan_VaiTro");

            entity.Property(e => e.HinhAnh)
                .HasMaxLength(255);
        });


        // =================================================
        // VAI TRÒ
        // =================================================

        modelBuilder.Entity<VaiTro>(entity =>
        {
            entity.HasKey(e => e.MaVaiTro);

            entity.ToTable("VaiTro");

            entity.HasIndex(e => e.TenVaiTro)
                .IsUnique();

            entity.Property(e => e.TenVaiTro)
                .HasMaxLength(50);

            entity.Property(e => e.MoTa)
                .HasMaxLength(255);

            entity.Property(e => e.IsActive)
                .HasDefaultValue(true);

            entity.HasMany(d => d.MaQuyens)
                .WithMany(p => p.MaVaiTros)
                .UsingEntity<Dictionary<string, object>>(
                    "VaiTroQuyen",
                    r => r
                        .HasOne<Quyen>()
                        .WithMany()
                        .HasForeignKey("MaQuyen")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_VaiTroQuyen_Quyen"),
                    l => l
                        .HasOne<VaiTro>()
                        .WithMany()
                        .HasForeignKey("MaVaiTro")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_VaiTroQuyen_VaiTro"),
                    j =>
                    {
                        j.HasKey("MaVaiTro", "MaQuyen");
                        j.ToTable("VaiTroQuyen");
                    });
        });


        // =================================================
        // QUYỀN
        // =================================================

        modelBuilder.Entity<Quyen>(entity =>
        {
            entity.HasKey(e => e.MaQuyen);

            entity.ToTable("Quyen");

            entity.HasIndex(e => e.TenQuyen)
                .IsUnique();

            entity.Property(e => e.TenQuyen)
                .HasMaxLength(100);

            entity.Property(e => e.MoTa)
                .HasMaxLength(255);

            entity.Property(e => e.IsActive)
                .HasDefaultValue(true);
        });


        // =================================================
        // PARTIAL
        // =================================================

        OnModelCreatingPartial(
            modelBuilder
        );
    }


    partial void OnModelCreatingPartial(
        ModelBuilder modelBuilder
    );
}