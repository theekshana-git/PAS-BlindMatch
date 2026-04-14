using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using PAS.BlindMatch.Models;

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

    public List<ApplicationUser> ActiveUsers { get; set; } = new List<ApplicationUser>();
}