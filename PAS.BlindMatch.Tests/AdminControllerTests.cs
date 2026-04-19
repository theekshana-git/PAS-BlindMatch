using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using PAS.BlindMatch.Controllers;
using PAS.BlindMatch.Data;
using PAS.BlindMatch.Models;
using PAS.BlindMatch.Services;
using Xunit;

namespace PAS.BlindMatch.Tests
{
    public class AdminControllerTests
    {
        [Fact]
        public async Task ResetProject_CallsMatchingService_AndRedirectsWithSuccess()
        {
            
            var mockMatchingService = new Mock<IMatchingService>();

            
            mockMatchingService.Setup(s => s.ResetProjectStatusAsync(99)).Returns(Task.CompletedTask);

            
            var mockUserManager = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);


            var controller = new AdminController(mockUserManager.Object, null, mockMatchingService.Object);

            
            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            
            var result = await controller.ResetProject(99);

            
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);

            
            Assert.Equal("AllocationOversight", redirectResult.ActionName);

            
            Assert.Equal("Project has been reset to Pending status.", controller.TempData["SuccessMessage"]);

            
            mockMatchingService.Verify(s => s.ResetProjectStatusAsync(99), Times.Once);
        }
    }
}