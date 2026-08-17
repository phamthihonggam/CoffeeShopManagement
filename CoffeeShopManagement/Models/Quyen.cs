using System.Collections.Generic;

namespace CoffeeShopManagement.Models;

public partial class Quyen
{
    public int MaQuyen { get; set; }

    public string TenQuyen { get; set; } = null!;

    public string? MoTa { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<VaiTro> MaVaiTros { get; set; }
        = new List<VaiTro>();
}