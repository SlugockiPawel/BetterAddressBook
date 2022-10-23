using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Principal;
using Microsoft.AspNetCore.Identity;

namespace BetterAddressBook.Models;

public class AppUser : IdentityUser
{
    [Required]
    [DisplayName("First Name")]
    [StringLength(50, ErrorMessage = "{0} must be between {2} and {1} characters long", MinimumLength = 2)]
    public string? FirstName { get; set; }
    
    [Required]
    [DisplayName("Last Name")]
    [StringLength(50, ErrorMessage = "{0} must be between {2} and {1} characters long", MinimumLength = 2)]
    public string? LastName { get; set; }
    
    [NotMapped] public string? FullName => $"{FirstName} {LastName}";
}