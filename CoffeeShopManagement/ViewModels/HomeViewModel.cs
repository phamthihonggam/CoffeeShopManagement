using CoffeeShopManagement.Models;

namespace CoffeeShopManagement.ViewModels
{
    public class HomeViewModel
    {
        public List<SanPham> BestSeller { get; set; }
            = new List<SanPham>();

        public List<SanPham> PromotionProducts { get; set; }
            = new List<SanPham>();
    }
}