namespace PAS.BlindMatch.Enums
{
    public enum ProjectStatus
    {
        Pending,        // Available for blind review
        UnderReview,    // A supervisor is looking at it
        Matched,        // Match confirmed, identity revealed
        Withdrawn       // Student pulled the project
    }

    public enum MatchStatus
    {
        Interested,     // Supervisor clicked "Express Interest" 
        Confirmed,      // Match finalized
        Rejected        // Supervisor or Admin declined the match
    }
}