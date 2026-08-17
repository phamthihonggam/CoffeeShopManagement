using System;
using System.Collections.Generic;

namespace CoffeeShopManagement.Models;

public partial class ProductSize
{
    public int Id { get; set; }

    public int MaSp { get; set; }

    public string TenSize { get; set; } = null!;

    public decimal GiaThem { get; set; }

    public int? ThuTu { get; set; }

    public virtual SanPham MaSpNavigation { get; set; } = null!;
}
