using Microsoft.AspNetCore.Mvc;

namespace TheWorld.Controllers.Web
{
    public class AppController : Controller
    {
        public IActionResult Index()
        {
            return View(nameof(Index));
        }

        public IActionResult Contact()
        {
            return View(nameof(Contact));
        }

        public IActionResult About()
        {
            return View(nameof(About));
        }
    }
}