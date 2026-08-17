using System.ComponentModel.DataAnnotations;

namespace CoffeeShopManagement.ViewModels
{
    public class VerifyOtpViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập mã xác nhận.")]
        [Display(Name = "Mã xác nhận")]
        public string Otp { get; set; } = "";
    }
}