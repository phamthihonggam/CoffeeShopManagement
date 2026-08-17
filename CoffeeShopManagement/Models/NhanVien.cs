using System;
using System.Collections.Generic;

namespace CoffeeShopManagement.Models;

public partial class NhanVien
{
    public int MaNv { get; set; }

    public string HoTen { get; set; } = null!;

    public string? Email { get; set; }

    public string? DienThoai { get; set; }

    public string TenDangNhap { get; set; } = null!;

    public string MatKhau { get; set; } = null!;

    public string ChucVu { get; set; } = null!;
}
