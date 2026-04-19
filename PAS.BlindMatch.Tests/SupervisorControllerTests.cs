using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Moq;
using PAS.BlindMatch.Controllers;
using PAS.BlindMatch.Data;
using PAS.BlindMatch.Models;
using PAS.BlindMatch.Services;
using PAS.BlindMatch.ViewModels;
using Xunit;

namespace PAS.BlindMatch.Tests
{
    public class SupervisorControllerTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        private SupervisorController CreateController(ApplicationDbContext context, Mock<IMatchingService> mockService)
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            var mockUserManager = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);

            var controller = new SupervisorController(mockService.Object, context, mockUserManager.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, "Supervisor1") })) }
                },
                TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
            };
            return controller;
        }

        [Fact]
        public async Task Expertise_Get_ReturnsViewModel()
        {
            var context = GetInMemoryDbContext();
            var mockService = new Mock<IMatchingService>();
            var controller = CreateController(context, mockService);

            var result = await controller.Expertise();

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<ExpertiseViewModel>(viewResult.Model);
        }

        [Fact]
        public async Task Expertise_Post_UpdatesDatabase_AndRedirects()
        {
            var context = GetInMemoryDbContext();
            var mockService = new Mock<IMatchingService>();
            var controller = CreateController(context, mockService);

            var selectedIds = new List<int> { 1, 2 };

            var result = await controller.Expertise(selectedIds);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Expertise", redirectResult.ActionName);
            Assert.Equal(2, await context.SupervisorExpertises.CountAsync());
        }

        [Fact]
        public async Task ExpressInterest_Post_CallsService_AndRedirects()
        {
            var context = GetInMemoryDbContext();
            var mockService = new Mock<IMatchingService>();
            mockService.Setup(s => s.ExpressInterestAsync(99, "Supervisor1")).Returns(Task.CompletedTask);

            var controller = CreateController(context, mockService);

            var result = await controller.ExpressInterest(99);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("BlindReview", redirectResult.ActionName);
            mockService.Verify(s => s.ExpressInterestAsync(99, "Supervisor1"), Times.Once);
        }
    }
}