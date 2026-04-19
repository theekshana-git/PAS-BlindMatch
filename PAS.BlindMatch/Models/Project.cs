using PAS.BlindMatch.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace PAS.BlindMatch.Models
{
    public class Project
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "A project title is required.")]
        [StringLength(100, ErrorMessage = "The title cannot exceed 100 characters.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "You must provide a project abstract.")]
        [MinLength(50, ErrorMessage = "The abstract must be at least 50 characters long.")]
        [MaxLength(2000, ErrorMessage = "The abstract cannot exceed 2000 characters.")]
        public string Abstract { get; set; } = string.Empty;

        
        [StringLength(200)]
        [RegularExpression(@"^[a-zA-Z0-9\s,+#\-\.]*$", ErrorMessage = "Tech stack contains invalid characters.")]
        public string? TechnicalStack { get; set; }

        [Required(ErrorMessage = "Please select a Research Area.")]
        public int ResearchAreaId { get; set; }

        [ValidateNever]
        public ResearchArea? ResearchArea { get; set; }

        
        public string? StudentId { get; set; }

        [ValidateNever]
        public ApplicationUser? Student { get; set; }

        public ProjectStatus Status { get; set; }

        
        public ICollection<MatchRequest>? MatchRequests { get; set; }
    }
}