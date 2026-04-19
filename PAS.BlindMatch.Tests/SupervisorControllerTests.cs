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
    public class SupervisorControllerTests
    {
        [Fact]
        public async Task ConfirmMatch_RedirectsToMyMatches_WithSuccessMessage()
        {
            
            var mockMatchingService = new Mock<IMatchingService>();
            var mockUserManager = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);

            
            mockMatchingService.Setup(s => s.ConfirmMatchAsync(5)).Returns(Task.CompletedTask);

           
            var controller = new SupervisorController(mockMatchingService.Object, null, mockUserManager.Object);

            
            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            controller.TempData = tempData;

            
            var result = await controller.ConfirmMatch(5);

            
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("MyMatches", redirectToActionResult.ActionName);
            Assert.Equal("Match Confirmed! Student identity has been revealed.", controller.TempData["SuccessMessage"]);

            
            mockMatchingService.Verify(s => s.ConfirmMatchAsync(5), Times.Once);
        }
    }
}