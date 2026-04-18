using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PAS.BlindMatch.Data;
using PAS.BlindMatch.Enums;
using PAS.BlindMatch.Models;
using PAS.BlindMatch.ViewModels;

namespace PAS.BlindMatch.Services
{
    public class MatchingService : IMatchingService
    {
        private readonly ApplicationDbContext _context;

        public MatchingService(ApplicationDbContext context)
        {
            _context = context;
        }

        

        public async Task<List<Project>> GetBlindProjectsForSupervisorAsync(string supervisorId)
        {
            var supervisorAreas = await _context.SupervisorExpertises
                .Where(se => se.SupervisorId == supervisorId)
                .Select(se => se.ResearchAreaId)
                .ToListAsync();

            
            return await _context.Projects
                .Include(p => p.ResearchArea)
                .Where(p => p.Status == ProjectStatus.Pending && supervisorAreas.Contains(p.ResearchAreaId))
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task ExpressInterestAsync(int projectId, string supervisorId)
        {
            var project = await _context.Projects.FindAsync(projectId);

            
            if (project == null)
                throw new Exception("Project not found.");

            if (project.Status != ProjectStatus.Pending)
                throw new InvalidOperationException("Project is no longer available for review.");

            // Create the request and update the project state
            var matchRequest = new MatchRequest
            {
                ProjectId = projectId,
                SupervisorId = supervisorId,
                Status = MatchStatus.Interested,
                RequestedDate = DateTime.UtcNow
            };

            project.Status = ProjectStatus.UnderReview;

            _context.MatchRequests.Add(matchRequest);
            await _context.SaveChangesAsync();
        }

        public async Task ConfirmMatchAsync(int matchRequestId)
        {
            var matchRequest = await _context.MatchRequests
                .Include(m => m.Project)
                .FirstOrDefaultAsync(m => m.Id == matchRequestId);

            if (matchRequest == null)
                throw new Exception("Match request not found.");

            // Reject if it's already confirmed
            if (matchRequest.Status == MatchStatus.Confirmed)
                throw new InvalidOperationException("This match is already confirmed.");

            
            matchRequest.Status = MatchStatus.Confirmed;
            matchRequest.ConfirmedDate = DateTime.UtcNow;
            matchRequest.Project.Status = ProjectStatus.Matched;

            await _context.SaveChangesAsync();
        }

        public async Task<MatchedRevealViewModel> GetRevealedMatchDetailsAsync(int matchRequestId)
        {
            var matchRequest = await _context.MatchRequests
                .Include(m => m.Project)
                    .ThenInclude(p => p.Student)
                .Include(m => m.Supervisor)
                .FirstOrDefaultAsync(m => m.Id == matchRequestId);

            if (matchRequest == null || matchRequest.Status != MatchStatus.Confirmed)
            {
                throw new InvalidOperationException("Identity Reveal Blocked: Match is not confirmed.");
            }

            return new MatchedRevealViewModel
            {
                ProjectTitle = matchRequest.Project.Title,
                StudentName = matchRequest.Project.Student.FullName,
                StudentEmail = matchRequest.Project.Student.Email,
                UniversityId = matchRequest.Project.Student.UniversityId,
                SupervisorName = matchRequest.Supervisor.FullName,
                SupervisorEmail = matchRequest.Supervisor.Email
            };
        }


        public async Task<Dictionary<int, string>> GetAllResearchAreasAsync()
        {
            return await _context.ResearchAreas
                .AsNoTracking()
                .ToDictionaryAsync(r => r.Id, r => r.Name);
        }

        public async Task<List<int>> GetSupervisorExpertiseAsync(string supervisorId)
        {
            return await _context.SupervisorExpertises
                .Where(se => se.SupervisorId == supervisorId)
                .Select(se => se.ResearchAreaId)
                .ToListAsync();
        }

        public async Task UpdateSupervisorExpertiseAsync(string supervisorId, List<int> selectedAreaIds)
        {
            var existing = _context.SupervisorExpertises.Where(se => se.SupervisorId == supervisorId);
            _context.SupervisorExpertises.RemoveRange(existing);

            if (selectedAreaIds != null && selectedAreaIds.Any())
            {
                var newExpertise = selectedAreaIds.Select(id => new SupervisorExpertise
                {
                    SupervisorId = supervisorId,
                    ResearchAreaId = id
                });
                _context.SupervisorExpertises.AddRange(newExpertise);
            }

            await _context.SaveChangesAsync();
        }


        public async Task<bool> StudentHasMatchedProjectAsync(string studentId)
        {
            // Quickly checks if the student has any project that is fully matched
            return await _context.Projects
                .AnyAsync(p => p.StudentId == studentId && p.Status == ProjectStatus.Matched);
        }

        public async Task ResetProjectStatusAsync(int projectId)
        {
            var project = await _context.Projects
                .Include(p => p.MatchRequests)
                .FirstOrDefaultAsync(p => p.Id == projectId);

            if (project != null)
            {
                if (project.MatchRequests != null && project.MatchRequests.Any())
                {
                    _context.MatchRequests.RemoveRange(project.MatchRequests);
                }

                project.Status = ProjectStatus.Pending;
                await _context.SaveChangesAsync();
            }
        }

        public async Task ReassignProjectAsync(int projectId, string newSupervisorId)
        {
            var project = await _context.Projects
                .Include(p => p.MatchRequests)
                .FirstOrDefaultAsync(p => p.Id == projectId);

            if (project == null) throw new Exception("Project not found.");

            // 1. Remove old match requests
            _context.MatchRequests.RemoveRange(project.MatchRequests);

            // 2. Create a new "Force-Confirmed" match
            var newMatch = new MatchRequest
            {
                ProjectId = projectId,
                SupervisorId = newSupervisorId,
                Status = MatchStatus.Confirmed, // Forced confirmation
                RequestedDate = DateTime.UtcNow,
                ConfirmedDate = DateTime.UtcNow
            };

            project.Status = ProjectStatus.Matched;

            _context.MatchRequests.Add(newMatch);
            await _context.SaveChangesAsync();
        }
    }
}