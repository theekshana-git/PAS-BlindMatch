using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PAS.BlindMatch.Controllers
{
    
    [Authorize]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {

            if (User.IsInRole("Admin"))
            {
                // Redirects to the Admin Oversight view
                return RedirectToAction("AllocationOversight", "Admin");
            }
            else if (User.IsInRole("Supervisor"))
            {
                // Redirects to the Blind Review view
                return RedirectToAction("BlindReview", "Supervisor");
            }
            else if (User.IsInRole("Student"))
            {
                // Redirects to the Student's project submission view
                return RedirectToAction("MyProject", "Student");
            }

            
            return View();
        }
    }
}