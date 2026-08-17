using System;
using System.Collections.Generic;

namespace CoffeeShopManagement.Models;

public partial class LuaChonMon
{
    public int MaLuaChon { get; set; }

    public int MaTuyChon { get; set; }

    public string TenLuaChon { get; set; } = null!;

    public decimal GiaThem { get; set; }

    public int ThuTu { get; set; }

    public bool TrangThai { get; set; }

    public virtual ICollection<ChiTietHoaDonLuaChon> ChiTietHoaDonLuaChons { get; set; } = new List<ChiTietHoaDonLuaChon>();

    public virtual TuyChonMon MaTuyChonNavigation { get; set; } = null!;
}
