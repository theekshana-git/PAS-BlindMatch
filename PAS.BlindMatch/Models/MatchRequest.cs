using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PAS.BlindMatch.Enums;

namespace PAS.BlindMatch.Models
{
    public class MatchRequest
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProjectId { get; set; }
        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; }

        [Required]
        public string SupervisorId { get; set; }
        [ForeignKey("SupervisorId")]
        public virtual ApplicationUser Supervisor { get; set; }

        public MatchStatus Status { get; set; } = MatchStatus.Interested;

        public DateTime RequestedDate { get; set; } = DateTime.UtcNow;
        public DateTime? ConfirmedDate { get; set; }
    }
}