using CoffeeShopManagement.Models;

namespace CoffeeShopManagement.ViewModels
{
    public class MenuLandingVM
    {
        public LoaiSanPham Category { get; set; } = null!;

        public string Image { get; set; } = "";

        public string Description { get; set; } = "";

        public string Background { get; set; } = "";
    }
}