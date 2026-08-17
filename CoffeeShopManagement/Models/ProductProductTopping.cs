using System.ComponentModel.DataAnnotations.Schema;

namespace CoffeeShopManagement.Models
{
    public class ProductProductTopping
    {
        public int MaSP { get; set; }

        public int ToppingId { get; set; }

        [ForeignKey(nameof(MaSP))]
        public virtual SanPham? SanPham { get; set; }

        [ForeignKey(nameof(ToppingId))]
        public virtual ProductTopping? ProductTopping { get; set; }
    }
}