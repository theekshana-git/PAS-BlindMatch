using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using PAS.BlindMatch.Models;

namespace PAS.BlindMatch.ViewModels
{
    public class UserManagementViewModel
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Display(Name = "University ID")]
        public string UniversityId { get; set; }

        [Required]
        public string Role { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        
        public List<UserWithRoleViewModel> ActiveUsers { get; set; } = new List<UserWithRoleViewModel>();
    }

    
    public class UserWithRoleViewModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UniversityId { get; set; }
        public string Role { get; set; }
    }
}