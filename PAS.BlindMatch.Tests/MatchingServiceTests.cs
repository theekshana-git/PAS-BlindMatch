using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PAS.BlindMatch.Data;
using PAS.BlindMatch.Enums;
using PAS.BlindMatch.Models;
using PAS.BlindMatch.Services;
using Xunit;

namespace PAS.BlindMatch.Tests
{
    public class MatchingServiceTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task ExpressInterestAsync_ShouldCreateMatchRequest_AndChangeProjectStatus()
        {
            var context = GetInMemoryDbContext();
            var service = new MatchingService(context);
            var project = new Project { Id = 1, Title = "Test", Abstract = "Test Abstract...", TechnicalStack = "C#", Status = ProjectStatus.Pending, StudentId = "S1", ResearchAreaId = 1 };

            context.Projects.Add(project);
            await context.SaveChangesAsync();

            await service.ExpressInterestAsync(1, "Supervisor1");

            var matchRequest = await context.MatchRequests.FirstOrDefaultAsync();
            var updatedProject = await context.Projects.FindAsync(1);

            Assert.NotNull(matchRequest);
            Assert.Equal(MatchStatus.Interested, matchRequest.Status);
            Assert.Equal(ProjectStatus.UnderReview, updatedProject.Status);
        }

        [Fact]
        public async Task GetRevealedMatchDetailsAsync_ShouldThrowException_IfMatchNotConfirmed()
        {
            var context = GetInMemoryDbContext();
            var service = new MatchingService(context);
            var unconfirmedMatch = new MatchRequest { Id = 1, ProjectId = 1, SupervisorId = "Sup1", Status = MatchStatus.Interested };

            context.MatchRequests.Add(unconfirmedMatch);
            await context.SaveChangesAsync();

            await Assert.ThrowsAsync<InvalidOperationException>(() => service.GetRevealedMatchDetailsAsync(1));
        }
        [Fact]
        public async Task ConfirmMatchAsync_ShouldUpdateMatchAndProjectStatus_ToConfirmed()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var service = new MatchingService(context);

            var project = new Project { Id = 1, Status = ProjectStatus.UnderReview, StudentId = "S1", Abstract = "abc", TechnicalStack = "C#", Title = "Title", ResearchAreaId = 1 };
            var matchRequest = new MatchRequest { Id = 1, ProjectId = 1, SupervisorId = "Sup1", Status = MatchStatus.Interested };

            context.Projects.Add(project);
            context.MatchRequests.Add(matchRequest);
            await context.SaveChangesAsync();

            // Act
            await service.ConfirmMatchAsync(1);

            // Assert
            var updatedMatch = await context.MatchRequests.FindAsync(1);
            var updatedProject = await context.Projects.FindAsync(1);

            Assert.Equal(MatchStatus.Confirmed, updatedMatch.Status);
            Assert.Equal(ProjectStatus.Matched, updatedProject.Status);
            Assert.NotNull(updatedMatch.ConfirmedDate);
        }
    }
}