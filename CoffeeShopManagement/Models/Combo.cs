using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoffeeShopManagement.Models
{
    [Table("Combo")]
    public class Combo
    {
        [Key]
        public int MaCombo { get; set; }

        public string TenCombo { get; set; } = null!;

        public string? MoTa { get; set; }

        public decimal GiaGoc { get; set; }

        public decimal GiaBan { get; set; }

        public string? HinhAnh { get; set; }

        public int PhanTramGiam { get; set; }

        public bool TrangThai { get; set; }

        public virtual ICollection<ChiTietCombo> ChiTietCombos { get; set; }
            = new List<ChiTietCombo>();
    }
}