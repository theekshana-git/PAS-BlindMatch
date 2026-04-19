using System.Collections.Generic;
using System.Linq;
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
using PAS.BlindMatch.ViewModels;
using Xunit;

namespace PAS.BlindMatch.Tests
{
    public class AdminIdentityTests
    {
        private Mock<UserManager<ApplicationUser>> GetMockUserManager()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            var mock = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);

            // Setup fake users list
            var users = new List<ApplicationUser> { new ApplicationUser { Id = "1", FirstName = "Test", Email = "test@test.com" } }.AsQueryable();
            mock.Setup(m => m.Users).Returns(users);
            mock.Setup(m => m.GetRolesAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(new List<string> { "Student" });
            mock.Setup(m => m.FindByIdAsync("1")).ReturnsAsync(users.First());
            mock.Setup(m => m.DeleteAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Success);

            return mock;
        }

        private AdminController CreateController()
        {
            var mockService = new Mock<IMatchingService>();
            var controller = new AdminController(GetMockUserManager().Object, null, mockService.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() },
                TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
            };
            return controller;
        }

        [Fact]
        public async Task UserManagement_Get_ReturnsViewWithUsers()
        {
            var controller = CreateController();
            var result = await controller.UserManagement();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<UserManagementViewModel>(viewResult.Model);
            Assert.Single(model.ActiveUsers);
        }

        [Fact]
        public async Task DeleteUser_Post_RemovesUser_AndRedirects()
        {
            var controller = CreateController();
            var result = await controller.DeleteUser("1");

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("UserManagement", redirectResult.ActionName);
            Assert.Equal("User Test was successfully deleted.", controller.TempData["SuccessMessage"]);
        }

        [Fact]
        public async Task EditUser_Get_ReturnsViewWithModel()
        {
            var controller = CreateController();
            var result = await controller.EditUser("1");

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<EditUserViewModel>(viewResult.Model);
            Assert.Equal("test@test.com", model.Email);
        }
    }
}