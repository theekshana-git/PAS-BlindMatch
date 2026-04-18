using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PAS.BlindMatch.Data;
using PAS.BlindMatch.Enums;
using PAS.BlindMatch.Models;
using System.Linq;
using System.Threading.Tasks;

namespace PAS.BlindMatch.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public StudentController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ==========================================
        // 1. SUBMIT PROJECT PAGE (The Form)
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> MyProject()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            bool hasMatchedProject = await _context.Projects
                .AnyAsync(p => p.StudentId == user!.Id && p.Status == ProjectStatus.Matched);

            ViewBag.HasMatchedProject = hasMatchedProject;

            var researchAreas = await _context.ResearchAreas.ToListAsync();
            ViewBag.ResearchAreas = new SelectList(researchAreas, "Id", "Name");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SubmitProject(Project project)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            bool hasMatchedProject = await _context.Projects
                .AnyAsync(p => p.StudentId == user!.Id && p.Status == ProjectStatus.Matched);

            if (hasMatchedProject)
            {
                TempData["ErrorMessage"] = "You already have a matched project. Submission locked.";
                return RedirectToAction(nameof(MyProject));
            }

            if (ModelState.IsValid)
            {
                project.StudentId = user!.Id;
                project.Status = ProjectStatus.Pending;

                _context.Projects.Add(project);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Project submitted successfully! Check 'My Proposals' to track its status.";
                return RedirectToAction(nameof(MyProject));
            }

            ViewBag.HasMatchedProject = false;
            var researchAreas = await _context.ResearchAreas.ToListAsync();
            ViewBag.ResearchAreas = new SelectList(researchAreas, "Id", "Name");

            return View("MyProject", project);
        }

        // ==========================================
        // 2. MY PROPOSALS PAGE (The History List)
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> MyProposals()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var myProjects = await _context.Projects
                .Include(p => p.ResearchArea)
                .Where(p => p.StudentId == user!.Id)
                .OrderByDescending(p => p.Id)
                .ToListAsync();

            return View(myProjects);
        }
    }
}