using System;
using System.Collections.Generic;

namespace CoffeeShopManagement.Models;

public partial class KhachHang
{
    public int MaKh { get; set; }

    public string HoTen { get; set; } = null!;

    public string? Email { get; set; }

    public string? DienThoai { get; set; }

    public string? DiaChi { get; set; }

    public string MatKhau { get; set; } = null!;

    public DateTime NgayTao { get; set; }

    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();
}