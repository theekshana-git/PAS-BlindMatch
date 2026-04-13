using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PAS.BlindMatch.Controllers
{
    [Authorize(Roles = "Supervisor")]
    public class SupervisorController : Controller
    {
        public IActionResult BlindReview()
        {
            return View();
        }

        public IActionResult Expertise()
        {
            return View();
        }

        public IActionResult MyMatches()
        {
            return View();
        }
    }
}