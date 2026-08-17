using System.Collections.Generic;

namespace CoffeeShopManagement.Models;

public partial class VaiTro
{
    public int MaVaiTro { get; set; }

    public string TenVaiTro { get; set; } = null!;

    public string? MoTa { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<TaiKhoan> TaiKhoans { get; set; }
        = new List<TaiKhoan>();

    public virtual ICollection<Quyen> MaQuyens { get; set; }
        = new List<Quyen>();
}