using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PAS.BlindMatch.Models
{
    public class ApplicationUser : IdentityUser
    {
        //Custom Properties

        [Required(ErrorMessage = "First Name is required.")]
        [StringLength(50, ErrorMessage = "First Name cannot exceed 50 characters.")]
        [RegularExpression(@"^[a-zA-Z\s\-]+$", ErrorMessage = "First Name can only contain letters, spaces, and hyphens.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required.")]
        [StringLength(50, ErrorMessage = "Last Name cannot exceed 50 characters.")]
        [RegularExpression(@"^[a-zA-Z\s\-]+$", ErrorMessage = "Last Name can only contain letters, spaces, and hyphens.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "University ID is required.")]
        [StringLength(20, ErrorMessage = "University ID cannot exceed 20 characters.")]
        [RegularExpression(@"^[A-Za-z0-9]+$", ErrorMessage = "University ID must be alphanumeric without special characters.")]
        [Display(Name = "University ID")]
        public string UniversityId { get; set; }

        [Display(Name = "Is this a Group Lead account?")]
        public bool IsGroupLead { get; set; }

        //Entity Framework Navigation Properties
        
        public virtual ICollection<Project> SubmittedProjects { get; set; } = new List<Project>();

        public virtual ICollection<SupervisorExpertise> Expertises { get; set; } = new List<SupervisorExpertise>();

        public virtual ICollection<MatchRequest> MatchRequests { get; set; } = new List<MatchRequest>();

        public string FullName => $"{FirstName} {LastName}";
    }
}