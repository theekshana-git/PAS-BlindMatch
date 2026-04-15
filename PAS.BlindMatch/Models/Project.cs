using PAS.BlindMatch.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PAS.BlindMatch.Models
{
    public class Project
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "A project title is required.")]
        [StringLength(100, ErrorMessage = "The title cannot exceed 100 characters.")]
        public string Title { get; set; } = string.Empty;

        // A+ Requirement: Lock down the Abstract field
        [Required(ErrorMessage = "You must provide a project abstract.")]
        [MinLength(50, ErrorMessage = "The abstract must be at least 50 characters long.")]
        public string Abstract { get; set; } = string.Empty;

        // Optional field based on your original model
        public string? TechnicalStack { get; set; }

        [Required(ErrorMessage = "Please select a Research Area.")]
        public int ResearchAreaId { get; set; }
        public ResearchArea? ResearchArea { get; set; }

        public string? StudentId { get; set; }
        public ApplicationUser? Student { get; set; }

        public ProjectStatus Status { get; set; }

        // Restored navigation property so EF Core doesn't crash
        public ICollection<MatchRequest>? MatchRequests { get; set; }
    }
}