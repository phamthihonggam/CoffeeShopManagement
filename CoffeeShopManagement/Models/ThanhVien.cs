using System;

namespace CoffeeShopManagement.Models
{
    public partial class ThanhVien
    {
        public int MaTv { get; set; }

        public int MaKh { get; set; }

        public string MaThanhVien { get; set; } = null!;

        public int Diem { get; set; }

        public string HangThanhVien { get; set; } = null!;

        public DateTime NgayThamGia { get; set; }

        public bool TrangThai { get; set; }

        public virtual KhachHang MaKhNavigation { get; set; } = null!;
    }
}