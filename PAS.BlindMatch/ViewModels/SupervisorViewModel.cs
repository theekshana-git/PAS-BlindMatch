using System.Collections.Generic;

namespace PAS.BlindMatch.ViewModels
{
    public class ExpertiseViewModel
    {
        
        public Dictionary<int, string> AllAvailableAreas { get; set; } = new Dictionary<int, string>();
        public List<int> SelectedAreaIds { get; set; } = new List<int>();
    }

    public class ProjectCardViewModel
    {
        public int ProjectId { get; set; }
        public int MatchRequestId { get; set; } 
        public string Title { get; set; }
        public string Abstract { get; set; }
        public string TechStack { get; set; }
        public string ResearchArea { get; set; }
        public string Status { get; set; }

        // Identity Reveal Data
        public string StudentName { get; set; }
        public string StudentEmail { get; set; }
        public string StudentUniversityId { get; set; }
    }
}