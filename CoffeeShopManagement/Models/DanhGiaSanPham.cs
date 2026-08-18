using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoffeeShopManagement.Models
{
    public class DanhGiaSanPham
    {
        public int MaDanhGia { get; set; }

        public int MaKh { get; set; }

        public int MaSp { get; set; }

        public int MaHd { get; set; }

        public int SoSao { get; set; }

        public string? NoiDung { get; set; }

        public DateTime NgayDanhGia { get; set; }

        public string? PhanHoi { get; set; }

        public DateTime? NgayPhanHoi { get; set; }

        public int? MaTaiKhoanPhanHoi { get; set; }

        // =========================================
        // NAVIGATION
        // =========================================

        [ForeignKey(nameof(MaKh))]
        public virtual KhachHang MaKhNavigation { get; set; } = null!;


        [ForeignKey(nameof(MaSp))]
        public virtual SanPham MaSpNavigation { get; set; } = null!;


        [ForeignKey(nameof(MaHd))]
        public virtual HoaDon MaHdNavigation { get; set; } = null!;

        [ForeignKey(nameof(MaTaiKhoanPhanHoi))]
        public virtual TaiKhoan? MaTaiKhoanPhanHoiNavigation { get; set; }
    }

}