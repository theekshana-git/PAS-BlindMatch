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

        private Mock<UserManager<ApplicationUser>> GetMockUserManager()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            return new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);
        }

        [Fact]
        public async Task SubmitProject_WhenStudentAlreadyMatched_ShouldBlockSubmission()
        {
            
            var context = GetInMemoryDbContext();

            
            var existingProject = new Project
            {
                Id = 99,
                StudentId = "TestStudent1",
                Status = ProjectStatus.Matched,
                Title = "Old Project",
                Abstract = "Old Abstract",
                ResearchAreaId = 1
            };
            context.Projects.Add(existingProject);
            await context.SaveChangesAsync();

           
            var mockUserManager = GetMockUserManager();
            var fakeUser = new ApplicationUser { Id = "TestStudent1", UserName = "student@test.com" };

            
            mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(fakeUser);

            
            var controller = new StudentController(context, mockUserManager.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            controller.TempData = new TempDataDictionary(controller.HttpContext, Mock.Of<ITempDataProvider>());

           
            var newProject = new Project { Title = "New Idea", Abstract = "New Abstract", ResearchAreaId = 2 };

           
            var result = await controller.SubmitProject(newProject);

            
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("MyProject", redirectResult.ActionName);
            Assert.Equal("You already have a matched project. Submission locked.", controller.TempData["ErrorMessage"]);
        }
    }
}