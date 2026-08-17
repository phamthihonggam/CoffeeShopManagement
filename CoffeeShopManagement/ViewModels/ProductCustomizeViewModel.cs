using CoffeeShopManagement.Models;

namespace CoffeeShopManagement.ViewModels
{
    public class ProductCustomizeViewModel
    {
        public SanPham? Product { get; set; }

        public List<ProductSize> Sizes { get; set; } = new();

        public List<IceLevel> IceLevels { get; set; } = new();

        public List<SugarLevel> SugarLevels { get; set; } = new();

        public List<ProductTopping> Toppings { get; set; } = new();

        public int Quantity { get; set; } = 1;

        public string? Note { get; set; }
    }
}