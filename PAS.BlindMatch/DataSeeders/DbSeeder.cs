using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PAS.BlindMatch.Data;
using PAS.BlindMatch.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace PAS.BlindMatch.DataSeeders
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            await context.Database.MigrateAsync();

            
            string[] roles = { "Admin", "Supervisor", "Student" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            
            var adminEmail = "admin@nsbm.ac.lk";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "System",
                    LastName = "Admin",
                    UniversityId = "ADMIN001",
                    EmailConfirmed = true
                };
                if ((await userManager.CreateAsync(admin, "Admin123!")).Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }

            
            var supEmail = "supervisor@nsbm.ac.lk";
            if (await userManager.FindByEmailAsync(supEmail) == null)
            {
                var sup = new ApplicationUser
                {
                    UserName = supEmail,
                    Email = supEmail,
                    FirstName = "System",
                    LastName = "Supervisor",
                    UniversityId = "SUP001",
                    EmailConfirmed = true
                };
                if ((await userManager.CreateAsync(sup, "Supervisor123!")).Succeeded)
                {
                    await userManager.AddToRoleAsync(sup, "Supervisor");
                }
            }

            
            var studentEmail = "student@nsbm.ac.lk";
            if (await userManager.FindByEmailAsync(studentEmail) == null)
            {
                var student = new ApplicationUser
                {
                    UserName = studentEmail,
                    Email = studentEmail,
                    FirstName = "Test",
                    LastName = "Student",
                    UniversityId = "STU001",
                    EmailConfirmed = true
                };
                if ((await userManager.CreateAsync(student, "Student123!")).Succeeded)
                {
                    await userManager.AddToRoleAsync(student, "Student");
                }
            }

            // Seed Research Areas
            if (!context.ResearchAreas.Any())
            {
                context.ResearchAreas.AddRange(
                    new ResearchArea { Name = "Artificial Intelligence" },
                    new ResearchArea { Name = "Web Development" },
                    new ResearchArea { Name = "Cybersecurity" },
                    new ResearchArea { Name = "Cloud Computing" }
                );
                await context.SaveChangesAsync();
            }
        }
    }
}