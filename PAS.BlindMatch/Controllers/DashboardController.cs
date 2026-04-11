using Microsoft.AspNetCore.Mvc;

namespace PAS.BlindMatch.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
