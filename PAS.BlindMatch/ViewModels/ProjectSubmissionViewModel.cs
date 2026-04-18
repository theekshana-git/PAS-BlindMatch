using System.ComponentModel.DataAnnotations;

namespace PAS.BlindMatch.ViewModels
{
    public class ProjectSubmissionViewModel
    {
        [Required(ErrorMessage = "Please provide a project title.")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
        [Display(Name = "Project Title")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Please provide an abstract.")]
        [MinLength(50, ErrorMessage = "Abstract must be detailed (at least 50 characters).")]
        [MaxLength(2000, ErrorMessage = "The abstract cannot exceed 2000 characters.")]
        [Display(Name = "Project Abstract")]
        public string Abstract { get; set; }

        [Required(ErrorMessage = "Please specify the technical stack (e.g., C#, React, SQL).")]
        [StringLength(200)]
        [RegularExpression(@"^[a-zA-Z0-9\s,+#\-\.]+$", ErrorMessage = "Tech stack contains invalid characters. Use letters, numbers, commas, and basic symbols (+, #, -).")]
        [Display(Name = "Technical Stack")]
        public string TechnicalStack { get; set; }

        [Required(ErrorMessage = "Please select a research area.")]
        [Display(Name = "Research Area")]
        public int ResearchAreaId { get; set; }
    }
}