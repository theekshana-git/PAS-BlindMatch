using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PAS.BlindMatch.Data;
using PAS.BlindMatch.Enums;
using PAS.BlindMatch.Models;
using PAS.BlindMatch.ViewModels;
using PAS.BlindMatch.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PAS.BlindMatch.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IMatchingService _matchingService;

        public AdminController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IMatchingService matchingService)
        {
            _userManager = userManager;
            _context = context;
            _matchingService = matchingService;
            
        }

        

        [HttpGet]
        public async Task<IActionResult> AllocationOversight(string searchString, string statusFilter, string categoryFilter)
        {
            var projectsQuery = _context.Projects
                .Include(p => p.Student)
                .Include(p => p.ResearchArea)
                .Include(p => p.MatchRequests)
                .ThenInclude(m => m.Supervisor)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                projectsQuery = projectsQuery.Where(p =>
                    (p.Title != null && p.Title.Contains(searchString)) ||
                    (p.Student != null && p.Student.FirstName != null && p.Student.FirstName.Contains(searchString)) ||
                    (p.Student != null && p.Student.LastName != null && p.Student.LastName.Contains(searchString)));
            }

            
            if (!string.IsNullOrEmpty(statusFilter))
            {
                if (System.Enum.TryParse<ProjectStatus>(statusFilter, true, out var parsedStatus))
                {
                    projectsQuery = projectsQuery.Where(p => p.Status == parsedStatus);
                }
            }

            if (!string.IsNullOrEmpty(categoryFilter))
            {
                projectsQuery = projectsQuery.Where(p => p.ResearchArea != null && p.ResearchArea.Name == categoryFilter);
            }

            ViewBag.StatusOptions = new SelectList(new[] { "Pending", "UnderReview", "Matched" });

            var categories = await _context.ResearchAreas
                .Where(r => !string.IsNullOrEmpty(r.Name))
                .Select(r => r.Name)
                .ToListAsync();

            ViewBag.CategoryOptions = new SelectList(categories);

            return View(await projectsQuery.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAllocation(int projectId)
        {
            var project = await _context.Projects.FindAsync(projectId);
            if (project != null)
            {
                _context.Projects.Remove(project);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Project allocation successfully revoked.";
            }
            return RedirectToAction(nameof(AllocationOversight));
        }

        

        [HttpGet]
        public async Task<IActionResult> ResearchAreas()
        {
            var areas = await _context.ResearchAreas.ToListAsync();
            return View(areas);
        }

        [HttpPost]
        public async Task<IActionResult> AddResearchArea(string areaName)
        {
            if (!string.IsNullOrWhiteSpace(areaName))
            {
                var newArea = new ResearchArea { Name = areaName };
                _context.ResearchAreas.Add(newArea);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Research Area '{areaName}' added.";
            }
            return RedirectToAction(nameof(ResearchAreas));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteResearchArea(int id)
        {
            var area = await _context.ResearchAreas.FindAsync(id);
            if (area != null)
            {
                _context.ResearchAreas.Remove(area);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Research Area deleted.";
            }
            return RedirectToAction(nameof(ResearchAreas));
        }

        

        private async Task<List<UserWithRoleViewModel>> GetActiveUsersListAsync()
        {
            var users = _userManager.Users.ToList();
            var userList = new List<UserWithRoleViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userList.Add(new UserWithRoleViewModel
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    UniversityId = user.UniversityId,
                    Role = roles.FirstOrDefault() ?? "Unassigned"
                });
            }
            return userList;
        }

        [HttpGet]
        public async Task<IActionResult> UserManagement()
        {
            var model = new UserManagementViewModel
            {
                ActiveUsers = await GetActiveUsersListAsync()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UserManagement(UserManagementViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    UniversityId = model.UniversityId
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, model.Role);
                    TempData["SuccessMessage"] = $"{model.FirstName} successfully registered as {model.Role}.";
                    return RedirectToAction("UserManagement");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            model.ActiveUsers = await GetActiveUsersListAsync();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
                TempData["SuccessMessage"] = $"User {user.FirstName} was successfully deleted.";
            }
            return RedirectToAction("UserManagement");
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);

            var model = new EditUserViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                UniversityId = user.UniversityId,
                Role = roles.FirstOrDefault()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.Id);
                if (user == null) return NotFound();

                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Email = model.Email;
                user.UserName = model.Email;
                user.UniversityId = model.UniversityId;

                if (!string.IsNullOrEmpty(model.NewPassword))
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var passwordResult = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

                    if (!passwordResult.Succeeded)
                    {
                        foreach (var error in passwordResult.Errors)
                            ModelState.AddModelError(string.Empty, error.Description);
                        return View(model);
                    }
                }

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    var currentRoles = await _userManager.GetRolesAsync(user);
                    await _userManager.RemoveFromRolesAsync(user, currentRoles);
                    await _userManager.AddToRoleAsync(user, model.Role);

                    TempData["SuccessMessage"] = $"{user.FirstName}'s profile updated.";
                    return RedirectToAction("UserManagement");
                }

                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        // POST: Admin/ResetProject
        [HttpPost]
        public async Task<IActionResult> ResetProject(int projectId)
        {
            await _matchingService.ResetProjectStatusAsync(projectId);
            TempData["SuccessMessage"] = "Project has been reset to Pending status.";
            return RedirectToAction(nameof(AllocationOversight));
        }

        // GET: Admin/ReassignProject
        [HttpGet]
        public async Task<IActionResult> ReassignProject(int projectId)
        {
            var project = await _context.Projects.FindAsync(projectId);
            if (project == null) return NotFound();

            // Get all supervisors for the dropdown
            var supervisors = await _userManager.GetUsersInRoleAsync("Supervisor");

            ViewBag.Supervisors = new SelectList(supervisors, "Id", "FirstName");
            return View(project);
        }

        // POST: Admin/ReassignProject
        [HttpPost]
        public async Task<IActionResult> ReassignProject(int projectId, string newSupervisorId)
        {
            await _matchingService.ReassignProjectAsync(projectId, newSupervisorId);
            TempData["SuccessMessage"] = "Project successfully reassigned to new supervisor.";
            return RedirectToAction(nameof(AllocationOversight));
        }
    }
}