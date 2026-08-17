using Microsoft.AspNetCore.Mvc;

namespace CoffeeShopManagement.ViewComponents
{
    public class MenuViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}