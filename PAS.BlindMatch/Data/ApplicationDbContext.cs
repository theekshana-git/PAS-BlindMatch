using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PAS.BlindMatch.Models;

namespace PAS.BlindMatch.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Project> Projects { get; set; }
        public DbSet<ResearchArea> ResearchAreas { get; set; }
        public DbSet<SupervisorExpertise> SupervisorExpertises { get; set; }
        public DbSet<MatchRequest> MatchRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // 1. A User (Student) can have many Projects. Do not delete projects if a user is deleted.
            builder.Entity<Project>()
                .HasOne(p => p.Student)
                .WithMany(u => u.SubmittedProjects)
                .HasForeignKey(p => p.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            // 2. A User (Supervisor) can have many MatchRequests. Do not delete history if supervisor leaves.
            builder.Entity<MatchRequest>()
                .HasOne(m => m.Supervisor)
                .WithMany(u => u.MatchRequests)
                .HasForeignKey(m => m.SupervisorId)
                .OnDelete(DeleteBehavior.Restrict);

            // 3. If a Project is deleted, cascade and delete all its associated MatchRequests.
            builder.Entity<MatchRequest>()
                .HasOne(m => m.Project)
                .WithMany(p => p.MatchRequests)
                .HasForeignKey(m => m.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            // 4. Do not delete Projects if a Research Area is deleted by Admin.
            builder.Entity<Project>()
                .HasOne(p => p.ResearchArea)
                .WithMany(r => r.Projects)
                .HasForeignKey(p => p.ResearchAreaId)
                .OnDelete(DeleteBehavior.Restrict);

            // 5. If a Research Area is deleted, delete the Supervisor Expertise links to it.
            builder.Entity<SupervisorExpertise>()
                .HasOne(se => se.ResearchArea)
                .WithMany(r => r.SupervisorExpertises)
                .HasForeignKey(se => se.ResearchAreaId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}