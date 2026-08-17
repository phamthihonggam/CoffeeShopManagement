using System;

namespace CoffeeShopManagement.Models;

public partial class TaiKhoan
{
    public int MaTaiKhoan { get; set; }

    public string TenDangNhap { get; set; } = null!;

    public string MatKhau { get; set; } = null!;

    public string HoTen { get; set; } = null!;

    public string? Email { get; set; }

    public string? DienThoai { get; set; }

    public int MaVaiTro { get; set; }

    public bool IsActive { get; set; }

    public DateTime NgayTao { get; set; }

    public string? HinhAnh { get; set; }

    public virtual VaiTro MaVaiTroNavigation { get; set; } = null!;
}