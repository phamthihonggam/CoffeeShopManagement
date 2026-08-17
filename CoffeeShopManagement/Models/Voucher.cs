using System;
using System.Collections.Generic;

namespace CoffeeShopManagement.Models
{
    public partial class Voucher
    {
        public Voucher()
        {
            KhachHangVouchers = new HashSet<KhachHangVoucher>();
        }

        public int MaVoucher { get; set; }

        public string MaCode { get; set; } = null!;

        public string TenVoucher { get; set; } = null!;

        public string LoaiGiam { get; set; } = null!;

        public decimal GiaTriGiam { get; set; }

        public decimal DonToiThieu { get; set; }

        public DateTime NgayHetHan { get; set; }

        public string? MoTa { get; set; }

        public bool TrangThai { get; set; }

        public virtual ICollection<KhachHangVoucher> KhachHangVouchers { get; set; }
    }
}