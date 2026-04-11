using System.Collections.Generic;
using System.Threading.Tasks;
using PAS.BlindMatch.Models;
using PAS.BlindMatch.ViewModels;

namespace PAS.BlindMatch.Services
{
    public interface IMatchingService
    {
        Task<List<Project>> GetBlindProjectsForSupervisorAsync(string supervisorId);
        Task ExpressInterestAsync(int projectId, string supervisorId);
        Task ConfirmMatchAsync(int matchRequestId);
        Task<MatchedRevealViewModel> GetRevealedMatchDetailsAsync(int matchRequestId);
    }
}