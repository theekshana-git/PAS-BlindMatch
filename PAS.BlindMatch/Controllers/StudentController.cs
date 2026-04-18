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

        [HttpGet]
        public async Task<IActionResult> MyProject()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            bool hasMatchedProject = await _context.Projects
                .AnyAsync(p => p.StudentId == user.Id && p.Status == ProjectStatus.Matched);

            ViewBag.HasMatchedProject = hasMatchedProject;
            ViewBag.ResearchAreas = new SelectList(await _context.ResearchAreas.ToListAsync(), "Id", "Name");

            return View(new Project());
        }

        [HttpPost]
        public async Task<IActionResult> SubmitProject(Project project)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            bool hasMatchedProject = await _context.Projects
                .AnyAsync(p => p.StudentId == user.Id && p.Status == ProjectStatus.Matched);

            if (hasMatchedProject)
            {
                TempData["ErrorMessage"] = "You already have a matched project. Submission locked.";
                return RedirectToAction(nameof(MyProject));
            }

            if (ModelState.IsValid)
            {
                project.StudentId = user.Id;
                project.Status = ProjectStatus.Pending;

                _context.Projects.Add(project);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Project submitted successfully!";
                return RedirectToAction(nameof(MyProposals));
            }

            
            TempData["ErrorMessage"] = "Submission failed. Please check the requirements (Abstract must be 50+ chars).";

            ViewBag.HasMatchedProject = false;
            ViewBag.ResearchAreas = new SelectList(await _context.ResearchAreas.ToListAsync(), "Id", "Name");
            return View("MyProject", project);
        }


        [HttpGet]
        public async Task<IActionResult> MyProposals()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var myProjects = await _context.Projects
                .Include(p => p.ResearchArea)
                .Include(p => p.MatchRequests)
                    .ThenInclude(m => m.Supervisor)
                .Where(p => p.StudentId == user.Id)
                .OrderByDescending(p => p.Id)
                .ToListAsync();

            return View(myProjects);
        }

        
        [HttpPost]
        public async Task<IActionResult> WithdrawProject(int projectId)
        {
            var user = await _userManager.GetUserAsync(User);

            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == projectId && p.StudentId == user.Id);

            if (project == null || project.Status == ProjectStatus.Matched)
            {
                TempData["ErrorMessage"] = "Cannot withdraw this project.";
                return RedirectToAction(nameof(MyProposals));
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Proposal successfully withdrawn.";
            return RedirectToAction(nameof(MyProposals));
        }

        [HttpGet]
        public async Task<IActionResult> EditProject(int projectId)
        {
            var user = await _userManager.GetUserAsync(User);

            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == projectId && p.StudentId == user.Id);

            if (project == null || project.Status == ProjectStatus.Matched)
            {
                TempData["ErrorMessage"] = "Cannot edit this project.";
                return RedirectToAction(nameof(MyProposals));
            }

            ViewBag.HasMatchedProject = false;
            ViewBag.ResearchAreas = new SelectList(await _context.ResearchAreas.ToListAsync(), "Id", "Name", project.ResearchAreaId);
            return View("MyProject", project);
        }

        [HttpPost]
        public async Task<IActionResult> EditProject(Project updatedProject)
        {
            var user = await _userManager.GetUserAsync(User);

            var existingProject = await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == updatedProject.Id && p.StudentId == user.Id);

            if (existingProject == null || existingProject.Status == ProjectStatus.Matched)
            {
                TempData["ErrorMessage"] = "Update failed. Project locked or not found.";
                return RedirectToAction(nameof(MyProposals));
            }

            if (ModelState.IsValid)
            {
                existingProject.Title = updatedProject.Title;
                existingProject.Abstract = updatedProject.Abstract;
                existingProject.TechnicalStack = updatedProject.TechnicalStack;
                existingProject.ResearchAreaId = updatedProject.ResearchAreaId;

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Proposal updated successfully.";
                return RedirectToAction(nameof(MyProposals));
            }

            ViewBag.HasMatchedProject = false;
            ViewBag.ResearchAreas = new SelectList(await _context.ResearchAreas.ToListAsync(), "Id", "Name", updatedProject.ResearchAreaId);
            return View("MyProject", updatedProject);
        }
    }
}