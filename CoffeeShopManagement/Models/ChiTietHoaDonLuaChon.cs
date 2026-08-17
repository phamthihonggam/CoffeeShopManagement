using System;
using System.Collections.Generic;

namespace CoffeeShopManagement.Models;

public partial class ChiTietHoaDonLuaChon
{
    public int MaCthd { get; set; }

    public int MaLuaChon { get; set; }

    public decimal GiaThem { get; set; }

    public virtual ChiTietHoaDon MaCthdNavigation { get; set; } = null!;

    public virtual LuaChonMon MaLuaChonNavigation { get; set; } = null!;
}
