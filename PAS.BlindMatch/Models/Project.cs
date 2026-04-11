using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PAS.BlindMatch.Enums;

namespace PAS.BlindMatch.Models
{
    public class Project
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(150)]
        public string Title { get; set; }

        [Required(ErrorMessage = "Abstract is required.")]
        [MinLength(50)]
        public string Abstract { get; set; }

        [Required(ErrorMessage = "Technical Stack is required.")]
        public string TechnicalStack { get; set; }

        public ProjectStatus Status { get; set; } = ProjectStatus.Pending;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Required]
        public string StudentId { get; set; }
        [ForeignKey("StudentId")]
        public virtual ApplicationUser Student { get; set; }

        [Required]
        public int ResearchAreaId { get; set; }
        [ForeignKey("ResearchAreaId")]
        public virtual ResearchArea ResearchArea { get; set; }

        public virtual ICollection<MatchRequest> MatchRequests { get; set; } = new List<MatchRequest>();
    }
}