using System;

namespace CoffeeShopManagement.Models
{
    public partial class KhachHangVoucher
    {
        public int MaKhVoucher { get; set; }

        public int MaKh { get; set; }

        public int MaVoucher { get; set; }

        public DateTime NgayNhan { get; set; }

        public DateTime? NgaySuDung { get; set; }

        public bool DaSuDung { get; set; }

        public virtual KhachHang MaKhNavigation { get; set; } = null!;

        public virtual Voucher MaVoucherNavigation { get; set; } = null!;
    }
}