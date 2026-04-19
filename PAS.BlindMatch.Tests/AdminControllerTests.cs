using System;
using System.Collections.Generic;
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
using Xunit;

namespace PAS.BlindMatch.Tests
{
    public class AdminControllerTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        private AdminController CreateController(ApplicationDbContext context, Mock<IMatchingService> mockService)
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            var mockUserManager = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);

            var controller = new AdminController(mockUserManager.Object, context, mockService.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() },
                TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
            };
            return controller;
        }

        [Fact]
        public async Task AllocationOversight_Get_ReturnsView_WithProjects()
        {
            var context = GetInMemoryDbContext();
            var mockService = new Mock<IMatchingService>();
            var controller = CreateController(context, mockService);

            var result = await controller.AllocationOversight(null, null, null);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<IEnumerable<Project>>(viewResult.Model);
        }

        [Fact]
        public async Task AddResearchArea_Post_AddsToDb_AndRedirects()
        {
            var context = GetInMemoryDbContext();
            var mockService = new Mock<IMatchingService>();
            var controller = CreateController(context, mockService);

            var result = await controller.AddResearchArea("Machine Learning");

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ResearchAreas", redirectResult.ActionName);
            Assert.Single(await context.ResearchAreas.ToListAsync());
        }

        [Fact]
        public async Task DeleteAllocation_Post_RemovesProject_AndRedirects()
        {
            var context = GetInMemoryDbContext();
            var project = new Project { Id = 5, Title = "Delete Me", Abstract = "abc", TechnicalStack = "C#", ResearchAreaId = 1, StudentId = "S1" };
            context.Projects.Add(project);
            await context.SaveChangesAsync();

            var mockService = new Mock<IMatchingService>();
            var controller = CreateController(context, mockService);

            var result = await controller.DeleteAllocation(5);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("AllocationOversight", redirectResult.ActionName);
            Assert.Empty(await context.Projects.ToListAsync());
        }
    }
}