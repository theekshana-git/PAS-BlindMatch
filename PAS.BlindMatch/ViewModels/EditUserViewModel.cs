using System.ComponentModel.DataAnnotations;

namespace PAS.BlindMatch.ViewModels
{
    public class EditUserViewModel
    {
        [Required]
        public string Id { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "University ID")]
        public string UniversityId { get; set; }

        [Required]
        [Display(Name = "Role")]
        public string Role { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "New Password (leave blank to keep current)")]
        public string NewPassword { get; set; }
    }
}