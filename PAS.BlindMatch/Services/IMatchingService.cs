using System.Collections.Generic;
using System.Threading.Tasks;
using PAS.BlindMatch.Models;
using PAS.BlindMatch.ViewModels;

namespace PAS.BlindMatch.Services
{
    public interface IMatchingService
    {
        // Supervisor Flow
        Task<List<Project>> GetBlindProjectsForSupervisorAsync(string supervisorId);
        Task ExpressInterestAsync(int projectId, string supervisorId);
        Task ConfirmMatchAsync(int matchRequestId);
        Task<MatchedRevealViewModel> GetRevealedMatchDetailsAsync(int matchRequestId);

        // Admin & Setup Flow 
        Task<Dictionary<int, string>> GetAllResearchAreasAsync();
        Task<List<int>> GetSupervisorExpertiseAsync(string supervisorId);
        Task UpdateSupervisorExpertiseAsync(string supervisorId, List<int> selectedAreaIds);

        // Student Flow 
        Task<bool> StudentHasMatchedProjectAsync(string studentId);

        Task ResetProjectStatusAsync(int projectId);
        Task ReassignProjectAsync(int projectId, string newSupervisorId);
    }
}