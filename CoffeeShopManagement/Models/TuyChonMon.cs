using System;
using System.Collections.Generic;

namespace CoffeeShopManagement.Models;

public partial class TuyChonMon
{
    public int MaTuyChon { get; set; }

    public string TenTuyChon { get; set; } = null!;

    public bool BatBuoc { get; set; }

    public int ChonToiDa { get; set; }

    public int ThuTu { get; set; }

    public virtual ICollection<LuaChonMon> LuaChonMons { get; set; } = new List<LuaChonMon>();

    public virtual ICollection<LoaiSanPham> MaLoais { get; set; } = new List<LoaiSanPham>();
}
