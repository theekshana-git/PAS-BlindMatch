using Microsoft.AspNetCore.Mvc;

namespace PAS.BlindMatch.Controllers
{
    public class StudentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
