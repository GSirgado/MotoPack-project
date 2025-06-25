using Microsoft.AspNetCore.Mvc;

namespace MotoPack_project.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
