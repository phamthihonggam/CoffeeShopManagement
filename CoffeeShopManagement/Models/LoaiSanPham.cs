using System;
using System.Collections.Generic;

namespace CoffeeShopManagement.Models;

public partial class LoaiSanPham
{
    public int MaLoai { get; set; }

    public string TenLoai { get; set; } = null!;

    public virtual ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();

    public virtual ICollection<TuyChonMon> MaTuyChons { get; set; } = new List<TuyChonMon>();
}
