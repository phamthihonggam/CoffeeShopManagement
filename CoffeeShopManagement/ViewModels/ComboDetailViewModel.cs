namespace CoffeeShopManagement.ViewModels
{
    public class ComboDetailViewModel
    {
        public int MaCombo { get; set; }

        public string TenCombo { get; set; } = "";

        public string? MoTa { get; set; }

        public string? HinhAnh { get; set; }

        public decimal GiaGoc { get; set; }

        public decimal GiaBan { get; set; }

        public int PhanTramGiam { get; set; }

        public List<ComboProductItemViewModel>
            Products
        { get; set; }
            = new();
    }


    public class ComboProductItemViewModel
    {
        public string TenSanPham { get; set; } = "";

        public int SoLuong { get; set; }
    }
}