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
using PAS.BlindMatch.Enums;
using PAS.BlindMatch.Models;
using Xunit;

namespace PAS.BlindMatch.Tests
{
    public class StudentControllerTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        private Mock<UserManager<ApplicationUser>> GetMockUserManager(ApplicationUser userToReturn)
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            var mock = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);
            mock.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(userToReturn);
            return mock;
        }

        private StudentController CreateController(ApplicationDbContext context, ApplicationUser user)
        {
            var controller = new StudentController(context, GetMockUserManager(user).Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, user.Id) })) }
                },
                TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
            };
            return controller;
        }

        [Fact]
        public async Task MyProject_Get_ReturnsView_WithNewProjectModel()
        {
            var context = GetInMemoryDbContext();
            var user = new ApplicationUser { Id = "Student1" };
            var controller = CreateController(context, user);

            var result = await controller.MyProject();

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<Project>(viewResult.Model);
        }

        [Fact]
        public async Task SubmitProject_ValidModel_SavesToDb_AndRedirects()
        {
            var context = GetInMemoryDbContext();
            var user = new ApplicationUser { Id = "Student1" };
            var controller = CreateController(context, user);

            var newProject = new Project { Title = "AI Test", Abstract = "Valid Abstract 50+ chars...", TechnicalStack = "C#", ResearchAreaId = 1 };

            var result = await controller.SubmitProject(newProject);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("MyProposals", redirectResult.ActionName);
            Assert.Single(await context.Projects.ToListAsync());
        }

        [Fact]
        public async Task WithdrawProject_ValidId_RemovesFromDb()
        {
            var context = GetInMemoryDbContext();
            var user = new ApplicationUser { Id = "Student1" };

            var project = new Project { Id = 1, StudentId = "Student1", Status = ProjectStatus.Pending, Title = "Test", Abstract = "abc", TechnicalStack = "C#", ResearchAreaId = 1 };
            context.Projects.Add(project);
            await context.SaveChangesAsync();

            var controller = CreateController(context, user);

            var result = await controller.WithdrawProject(1);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("MyProposals", redirectResult.ActionName);
            Assert.Empty(await context.Projects.ToListAsync());
        }
    }
}