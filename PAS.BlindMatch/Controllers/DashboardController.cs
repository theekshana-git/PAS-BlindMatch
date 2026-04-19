using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace PAS.BlindMatch.Controllers
{
    [ExcludeFromCodeCoverage]
    [Authorize]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("AllocationOversight", "Admin");
            }
            else if (User.IsInRole("Supervisor"))
            {
                return RedirectToAction("BlindReview", "Supervisor");
            }
            else if (User.IsInRole("Student"))
            {
                return RedirectToAction("MyProject", "Student");
            }

            return View();
        }
    }
}