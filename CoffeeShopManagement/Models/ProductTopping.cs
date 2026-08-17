using System;
using System.Collections.Generic;

namespace CoffeeShopManagement.Models;

public partial class ProductTopping
{
    public int Id { get; set; }

    public string TenTopping { get; set; } = null!;

    public decimal GiaThem { get; set; }

    public string? HinhAnh { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<SanPham> MaSps { get; set; } = new List<SanPham>();
}
