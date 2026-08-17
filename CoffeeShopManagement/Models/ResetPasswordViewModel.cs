using System.ComponentModel.DataAnnotations;

namespace CoffeeShopManagement.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới.")]
        [MinLength(8, ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự.")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu mới")]
        public string MatKhauMoi { get; set; } = "";


        [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu.")]
        [DataType(DataType.Password)]
        [Compare(
            nameof(MatKhauMoi),
            ErrorMessage = "Mật khẩu xác nhận không khớp."
        )]
        [Display(Name = "Xác nhận mật khẩu")]
        public string XacNhanMatKhau { get; set; } = "";
    }
}