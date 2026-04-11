using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PAS.BlindMatch.Models
{
    public class SupervisorExpertise
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string SupervisorId { get; set; }
        [ForeignKey("SupervisorId")]
        public virtual ApplicationUser Supervisor { get; set; }

        [Required]
        public int ResearchAreaId { get; set; }
        [ForeignKey("ResearchAreaId")]
        public virtual ResearchArea ResearchArea { get; set; }
    }
}