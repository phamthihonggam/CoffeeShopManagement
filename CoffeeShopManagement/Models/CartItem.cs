namespace CoffeeShopManagement.Models
{
    public class CartItem
    {
        // Mã riêng cho từng dòng trong giỏ hàng
        public Guid RowId { get; set; } = Guid.NewGuid();

        public int MaSP { get; set; }

        public bool IsCombo { get; set; } = false;

        public int? MaCombo { get; set; }

        public string TenSP { get; set; } = string.Empty;

        public decimal DonGia { get; set; }

        public int SoLuong { get; set; }

        public string? HinhAnh { get; set; }

        // ==========================
        // CUSTOMIZE
        // ==========================

        public string Size { get; set; } = "S";

        public string MucDa { get; set; } = "Đá vừa";

        public string DoNgot { get; set; } = "100%";

        public List<string> Toppings { get; set; } = new();

        public string? GhiChu { get; set; }

        // Giá cộng thêm từ Size
        public decimal GiaSize { get; set; }

        // Giá cộng thêm từ Topping
        public decimal GiaTopping { get; set; }

        // Tổng tiền
        public decimal ThanhTien =>
            (DonGia + GiaSize + GiaTopping) * SoLuong;
    }
}