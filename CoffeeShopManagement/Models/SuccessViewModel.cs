using System.Collections.Generic;

namespace CoffeeShopManagement.Models
{
    public class SuccessViewModel
    {
        // Thông tin hóa đơn
        public HoaDon HoaDon { get; set; } = new HoaDon();

        // Thông tin khách hàng
        public KhachHang KhachHang { get; set; } = new KhachHang();

        // Danh sách sản phẩm trong hóa đơn
        public List<SuccessItem> SanPham { get; set; } = new();
    }

    public class SuccessItem
    {
        public int MaSP { get; set; }

        public string TenSP { get; set; } = "";

        public string? HinhAnh { get; set; }

        public decimal DonGia { get; set; }

        public int SoLuong { get; set; }

        public decimal ThanhTien
        {
            get
            {
                return DonGia * SoLuong;
            }
        }
    }
}