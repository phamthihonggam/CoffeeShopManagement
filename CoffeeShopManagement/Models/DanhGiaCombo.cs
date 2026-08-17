using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoffeeShopManagement.Models
{
    public class DanhGiaCombo
    {
        public int MaDanhGia { get; set; }

        public int MaKh { get; set; }

        public int MaCombo { get; set; }

        public int MaHd { get; set; }

        public int SoSao { get; set; }

        public string? NoiDung { get; set; }

        public DateTime NgayDanhGia { get; set; }


        // =========================================
        // NAVIGATION
        // =========================================

        [ForeignKey(nameof(MaKh))]
        public virtual KhachHang MaKhNavigation { get; set; } = null!;


        [ForeignKey(nameof(MaCombo))]
        public virtual Combo MaComboNavigation { get; set; } = null!;


        [ForeignKey(nameof(MaHd))]
        public virtual HoaDon MaHdNavigation { get; set; } = null!;
    }
}