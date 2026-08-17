using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoffeeShopManagement.Models;

public partial class ChiTietHoaDon
{
    public int MaCthd { get; set; }

    public int MaHd { get; set; }

    // =====================================================
    // SẢN PHẨM THƯỜNG
    // =====================================================

    public int? MaSp { get; set; }

    // =====================================================
    // COMBO
    // =====================================================

    public int? MaCombo { get; set; }

    // =====================================================
    // SỐ LƯỢNG
    // =====================================================

    public int SoLuong { get; set; }

    // =====================================================
    // ĐƠN GIÁ
    // =====================================================

    public decimal DonGia { get; set; }

    // =====================================================
    // LỰA CHỌN
    // =====================================================

    public virtual ICollection<ChiTietHoaDonLuaChon>
        ChiTietHoaDonLuaChons
    { get; set; }
        = new List<ChiTietHoaDonLuaChon>();

    // =====================================================
    // HÓA ĐƠN
    // =====================================================

    [ForeignKey(nameof(MaHd))]
    public virtual HoaDon MaHdNavigation { get; set; } = null!;

    // =====================================================
    // SẢN PHẨM
    // =====================================================

    [ForeignKey(nameof(MaSp))]
    public virtual SanPham? MaSpNavigation { get; set; }

    // =====================================================
    // COMBO
    // =====================================================

    [ForeignKey(nameof(MaCombo))]
    public virtual Combo? MaComboNavigation { get; set; }
}