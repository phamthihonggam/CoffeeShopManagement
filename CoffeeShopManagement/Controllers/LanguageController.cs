using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeShopManagement.Controllers
{
    public class LanguageController : Controller
    {
        [HttpGet]
        public IActionResult SetLanguage(
            string culture,
            string returnUrl)
        {
            if (culture != "vi" && culture != "en")
            {
                culture = "vi";
            }

            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(
                    new RequestCulture(culture)
                ),
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(1),
                    IsEssential = true
                }
            );

            return LocalRedirect(
                string.IsNullOrEmpty(returnUrl)
                    ? "/"
                    : returnUrl
            );
        }
    }
}