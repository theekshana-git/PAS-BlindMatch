using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PAS.BlindMatch.Data;
using PAS.BlindMatch.Models;

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

            // Roles
            string[] roles = { "Admin", "Supervisor", "Student" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Admin
            var adminEmail = "admin@nsbm.ac.lk";
            var admin = await userManager.FindByEmailAsync(adminEmail);

            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "System",
                    LastName = "Admin",
                    UniversityId = "ADMIN001",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(admin, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }

            // Supervisor
            var supEmail = "supervisor@nsbm.ac.lk";
            var sup = await userManager.FindByEmailAsync(supEmail);

            if (sup == null)
            {
                sup = new ApplicationUser
                {
                    UserName = supEmail,
                    Email = supEmail,
                    FirstName = "System",
                    LastName = "Supervisor",
                    UniversityId = "SUP001",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(sup, "Supervisor123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(sup, "Supervisor");
                }
            }

            // Research Areas
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