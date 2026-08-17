using System.ComponentModel.DataAnnotations;

namespace CoffeeShopManagement.ViewModels
{
    public class DangNhapViewModel
    {
        // =====================================================
        // EMAIL HOẶC TÊN ĐĂNG NHẬP
        // =====================================================

        [Required(
            ErrorMessage = "Vui lòng nhập Email hoặc tên đăng nhập."
        )]
        [Display(
            Name = "Email hoặc tên đăng nhập"
        )]
        public string Email { get; set; } = "";


        // =====================================================
        // MẬT KHẨU
        // =====================================================

        [Required(
            ErrorMessage = "Vui lòng nhập mật khẩu."
        )]
        [DataType(
            DataType.Password
        )]
        public string MatKhau { get; set; } = "";


        // =====================================================
        // GHI NHỚ ĐĂNG NHẬP
        // =====================================================

        public bool RememberMe { get; set; }
    }
}