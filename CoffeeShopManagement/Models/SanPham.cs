using System;
using System.Collections.Generic;

namespace CoffeeShopManagement.Models;

public partial class SanPham
{
    public int MaSp { get; set; }

    public string TenSp { get; set; } = null!;

    public decimal DonGia { get; set; }

    public string? HinhAnh { get; set; }

    public string? MoTa { get; set; }

    public int MaLoai { get; set; }

    public decimal? GiaGoc { get; set; }

    public decimal? GiaKhuyenMai { get; set; }

    public int? PhanTramGiam { get; set; }

    public bool DangKhuyenMai { get; set; }

    public DateTime? NgayBatDau { get; set; }

    public DateTime? NgayKetThuc { get; set; }

    public virtual ICollection<ChiTietHoaDon> ChiTietHoaDons { get; set; } = new List<ChiTietHoaDon>();

    public virtual LoaiSanPham MaLoaiNavigation { get; set; } = null!;

    public virtual ICollection<ProductOption> ProductOptions { get; set; } = new List<ProductOption>();

    public virtual ICollection<ProductSize> ProductSizes { get; set; } = new List<ProductSize>();

    public virtual ICollection<ProductTopping> Toppings { get; set; } = new List<ProductTopping>();
}
