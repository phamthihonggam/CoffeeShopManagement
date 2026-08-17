using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoffeeShopManagement.Models
{
    [Table("ChiNhanh")]
    public class ChiNhanh
    {
        [Key]
        public int MaCN { get; set; }

        [Required]
        [StringLength(150)]
        public string TenChiNhanh { get; set; } = null!;

        [Required]
        [StringLength(255)]
        public string DiaChi { get; set; } = null!;

        [StringLength(100)]
        public string? Quan { get; set; }

        [StringLength(100)]
        public string? ThanhPho { get; set; }

        [StringLength(20)]
        public string? DienThoai { get; set; }

        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(100)]
        public string? GioMoCua { get; set; }

        public string? MoTa { get; set; }

        [StringLength(255)]
        public string? HinhAnh { get; set; }

        [StringLength(500)]
        public string? GoogleMap { get; set; }

        public double? ViDo { get; set; }

        public double? KinhDo { get; set; }

        public bool Wifi { get; set; }

        public bool BaiDoXe { get; set; }

        public bool MayLanh { get; set; }

        public bool OCam { get; set; }

        [Column(TypeName = "decimal(2,1)")]
        public decimal DanhGia { get; set; }

        public bool TrangThai { get; set; }
    }
}