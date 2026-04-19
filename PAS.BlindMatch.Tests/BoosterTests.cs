using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using PAS.BlindMatch.Controllers;
using PAS.BlindMatch.Data;
using PAS.BlindMatch.Models;
using PAS.BlindMatch.Services;
using Xunit;

namespace PAS.BlindMatch.Tests
{
    public class BoosterTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        private Mock<UserManager<ApplicationUser>> GetMockUserManager()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            var mock = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);
            // Always return a fake user so the controllers don't crash on null checks
            mock.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new ApplicationUser { Id = "TestUser" });
            return mock;
        }

        // --- STUDENT CONTROLLER BOOSTERS ---

        [Fact]
        public async Task MyProposals_ReturnsView()
        {
            var controller = new StudentController(GetInMemoryDbContext(), GetMockUserManager().Object)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
            };
            var result = await controller.MyProposals();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task EditProject_Get_ReturnsView()
        {
            var context = GetInMemoryDbContext();
            context.Projects.Add(new Project { Id = 99, StudentId = "TestUser", Title = "Title", Abstract = "Abstract", TechnicalStack = "C#" });
            await context.SaveChangesAsync();

            var controller = new StudentController(context, GetMockUserManager().Object)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
            };
            var result = await controller.EditProject(99);
            Assert.IsType<ViewResult>(result);
        }

        // --- SUPERVISOR CONTROLLER BOOSTERS ---

        [Fact]
        public async Task BlindReview_ReturnsView()
        {
            var mockService = new Mock<IMatchingService>();
            mockService.Setup(s => s.GetBlindProjectsForSupervisorAsync(It.IsAny<string>())).ReturnsAsync(new List<Project>());

            var controller = new SupervisorController(mockService.Object, GetInMemoryDbContext(), GetMockUserManager().Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, "Sup1") })) }
                }
            };
            var result = await controller.BlindReview();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task MyMatches_ReturnsView()
        {
            var controller = new SupervisorController(new Mock<IMatchingService>().Object, GetInMemoryDbContext(), GetMockUserManager().Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, "Sup1") })) }
                }
            };
            var result = await controller.MyMatches();
            Assert.IsType<ViewResult>(result);
        }
    }
}