using System;
using System.Collections.Generic;

namespace CoffeeShopManagement.Models;

public partial class ProductOption
{
    public int Id { get; set; }

    public int MaSp { get; set; }

    public bool AllowSize { get; set; }

    public bool AllowIce { get; set; }

    public bool AllowSugar { get; set; }

    public bool AllowTopping { get; set; }

    public virtual SanPham MaSpNavigation { get; set; } = null!;
}
