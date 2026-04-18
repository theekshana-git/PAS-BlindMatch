using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PAS.BlindMatch.Data;
using PAS.BlindMatch.Enums;
using PAS.BlindMatch.Models;
using PAS.BlindMatch.Services;
using PAS.BlindMatch.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PAS.BlindMatch.Controllers
{
    [Authorize(Roles = "Supervisor")]
    public class SupervisorController : Controller
    {
        private readonly IMatchingService _matchingService;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public SupervisorController(IMatchingService matchingService, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _matchingService = matchingService;
            _context = context;
            _userManager = userManager;
        }

        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);

        [HttpGet]
        public async Task<IActionResult> Expertise()
        {
            var userId = GetUserId();
            var model = new ExpertiseViewModel
            {
                AllAvailableAreas = await _context.ResearchAreas.ToDictionaryAsync(r => r.Id, r => r.Name),
                SelectedAreaIds = await _context.SupervisorExpertises
                                        .Where(se => se.SupervisorId == userId)
                                        .Select(se => se.ResearchAreaId)
                                        .ToListAsync()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Expertise(List<int> selectedAreaIds)
        {
            var userId = GetUserId();

            var existing = _context.SupervisorExpertises.Where(se => se.SupervisorId == userId);
            _context.SupervisorExpertises.RemoveRange(existing);

            var newExpertise = selectedAreaIds.Select(id => new SupervisorExpertise { SupervisorId = userId, ResearchAreaId = id });
            _context.SupervisorExpertises.AddRange(newExpertise);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Your expertise preferences have been updated.";
            return RedirectToAction("Expertise");
        }

        public async Task<IActionResult> BlindReview()
        {
            var userId = GetUserId();
            var dbProjects = await _matchingService.GetBlindProjectsForSupervisorAsync(userId);

            var model = dbProjects.Select(p => new ProjectCardViewModel
            {
                ProjectId = p.Id,
                Title = p.Title,
                Abstract = p.Abstract,
                TechStack = p.TechnicalStack,
                ResearchArea = p.ResearchArea?.Name ?? "Unknown Area",
                Status = p.Status.ToString()
            }).ToList();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ExpressInterest(int projectId)
        {
            var userId = GetUserId();
            await _matchingService.ExpressInterestAsync(projectId, userId);

            TempData["SuccessMessage"] = "Interest expressed! Project moved to 'My Matches'.";
            return RedirectToAction("BlindReview");
        }

        
        public async Task<IActionResult> MyMatches()
        {
            var userId = GetUserId();

            var matchRequests = await _context.MatchRequests
                .Include(m => m.Project).ThenInclude(p => p.ResearchArea)
                .Include(m => m.Project).ThenInclude(p => p.Student)
                .Where(m => m.SupervisorId == userId)
                .ToListAsync();

            var model = matchRequests.Select(m => new ProjectCardViewModel
            {
                ProjectId = m.ProjectId,
                MatchRequestId = m.Id,
                Title = m.Project.Title,
                Abstract = m.Project.Abstract,
                Status = m.Project.Status.ToString(),
                StudentName = m.Status == MatchStatus.Confirmed ? m.Project.Student.FirstName + " " + m.Project.Student.LastName : null,
                StudentEmail = m.Status == MatchStatus.Confirmed ? m.Project.Student.Email : null,
                StudentUniversityId = m.Status == MatchStatus.Confirmed ? m.Project.Student.UniversityId : null
            }).ToList();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmMatch(int matchRequestId)
        {
            await _matchingService.ConfirmMatchAsync(matchRequestId);

            TempData["SuccessMessage"] = "Match Confirmed! Student identity has been revealed.";
            return RedirectToAction("MyMatches");
        }
    }
}