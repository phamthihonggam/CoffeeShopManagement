using CoffeeShopManagement.Models;

namespace CoffeeShopManagement.ViewModels
{
    public class PromotionViewModel
    {
        public List<SanPham> SanPhams { get; set; } = new();

        public List<Combo> Combos { get; set; } = new();
    }
}