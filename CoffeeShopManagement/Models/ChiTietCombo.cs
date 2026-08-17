using System.ComponentModel.DataAnnotations.Schema;

namespace CoffeeShopManagement.Models
{
    [Table("ChiTietCombo")]
    public class ChiTietCombo
    {
        public int MaCombo { get; set; }

        public int MaSanPham { get; set; }

        public int SoLuong { get; set; }

        [ForeignKey(nameof(MaCombo))]
        public virtual Combo? Combo { get; set; }

        [ForeignKey(nameof(MaSanPham))]
        public virtual SanPham? SanPham { get; set; }
    }
}