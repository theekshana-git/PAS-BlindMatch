using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PAS.BlindMatch.Models
{
    public class ResearchArea
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
        public virtual ICollection<SupervisorExpertise> SupervisorExpertises { get; set; } = new List<SupervisorExpertise>();
    }
}