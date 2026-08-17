using System;
using System.Collections.Generic;

namespace CoffeeShopManagement.Models;

public partial class HoaDon
{
    public int MaHd { get; set; }

    public int MaKh { get; set; }

    public DateTime? NgayDat { get; set; }


    // =========================================
    // CHI TIẾT TIỀN
    // =========================================

    // Tổng tiền hàng trước voucher + shipping
    public decimal? TamTinh { get; set; }

    // Số tiền được giảm
    public decimal? GiamGia { get; set; }

    // Phí giao hàng
    public decimal? PhiGiaoHang { get; set; }

    // Số tiền khách thực tế phải thanh toán
    public decimal? TongTien { get; set; }


    // =========================================
    // TRẠNG THÁI ĐƠN HÀNG
    // =========================================

    public string? TrangThai { get; set; }


    // =========================================
    // THÔNG TIN GIAO HÀNG CỦA ĐƠN
    // =========================================

    // Lưu riêng theo từng đơn hàng,
    // không phụ thuộc thông tin hiện tại của KhachHang
    public string? HoTenNguoiNhan { get; set; }

    public string? DienThoaiNguoiNhan { get; set; }

    public string? DiaChiGiaoHang { get; set; }

    public string? QuanHuyenGiaoHang { get; set; }


    // =========================================
    // THANH TOÁN
    // =========================================

    // COD / VNPAY
    public string? PhuongThucThanhToan { get; set; }


    // =========================================
    // HỦY ĐƠN
    // =========================================

    // Lý do khách hàng chọn khi hủy / yêu cầu hủy
    public string? LyDoHuy { get; set; }

    // Nội dung nhập thêm khi khách chọn "Khác"
    public string? GhiChuHuy { get; set; }

    // Thời điểm khách gửi yêu cầu hủy
    public DateTime? NgayYeuCauHuy { get; set; }

    // Thời điểm Admin / Nhân viên xử lý yêu cầu
    public DateTime? NgayXuLyHuy { get; set; }


    // =========================================
    // HOÀN TIỀN
    // =========================================

    // Không cần hoàn tiền
    // Chờ hoàn tiền
    // Đã hoàn tiền
    public string? TrangThaiHoanTien { get; set; }

    // Thời điểm hoàn tiền hoàn tất
    public DateTime? NgayHoanTien { get; set; }

    // Số tiền cần hoàn
    public decimal? SoTienHoan { get; set; }


    // =========================================
    // GIAO HÀNG
    // =========================================

    // Tài khoản nhân viên giao hàng được phân công
    public int? MaTaiKhoanGiao { get; set; }

    // Thời điểm shipper nhận đơn
    public DateTime? NgayNhanGiao { get; set; }

    // Thời điểm bắt đầu giao
    public DateTime? NgayBatDauGiao { get; set; }

    // Thời điểm giao thành công
    public DateTime? NgayGiaoThanhCong { get; set; }

    // Lý do khi giao thất bại
    public string? LyDoGiaoThatBai { get; set; }


    // =========================================
    // NAVIGATION
    // =========================================

    public virtual ICollection<ChiTietHoaDon>
        ChiTietHoaDons
    { get; set; }
        = new List<ChiTietHoaDon>();


    public virtual KhachHang MaKhNavigation
    { get; set; } = null!;


    public virtual TaiKhoan? MaTaiKhoanGiaoNavigation
    { get; set; }
}