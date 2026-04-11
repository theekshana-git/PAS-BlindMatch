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

            // Returns ONLY pending projects and DOES NOT include the Student data (Anonymity Constraint)
            return await _context.Projects
                .Include(p => p.ResearchArea)
                .Where(p => p.Status == ProjectStatus.Pending && supervisorAreas.Contains(p.ResearchAreaId))
                .ToListAsync();
        }

        public async Task ExpressInterestAsync(int projectId, string supervisorId)
        {
            var matchRequest = new MatchRequest
            {
                ProjectId = projectId,
                SupervisorId = supervisorId,
                Status = MatchStatus.Interested,
                RequestedDate = DateTime.UtcNow
            };

            var project = await _context.Projects.FindAsync(projectId);
            if (project != null)
            {
                project.Status = ProjectStatus.UnderReview;
            }

            _context.MatchRequests.Add(matchRequest);
            await _context.SaveChangesAsync();
        }

        public async Task ConfirmMatchAsync(int matchRequestId)
        {
            var matchRequest = await _context.MatchRequests
                .Include(m => m.Project)
                .FirstOrDefaultAsync(m => m.Id == matchRequestId);

            if (matchRequest == null) throw new Exception("Match request not found.");

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
    }
}