using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BetterAddressBook.Enums;

namespace BetterAddressBook.Models;

public class ContactModel
{
    public int Id { get; set; }

    [Required] public string? AppUserId { get; set; }

    [Required]
    [DisplayName("First Name")]
    [StringLength(
        50,
        ErrorMessage = "{0} must be between {2} and {1} characters long",
        MinimumLength = 2
    )]
    public string? FirstName { get; set; }

    [Required]
    [DisplayName("Last Name")]
    [StringLength(
        50,
        ErrorMessage = "{0} must be between {2} and {1} characters long",
        MinimumLength = 2
    )]
    public string? LastName { get; set; }

    [NotMapped] public string? FullName => $"{FirstName} {LastName}";

    [DisplayName("Birthday")]
    [DataType(DataType.Date)]
    public DateTime? BirthDate { get; set; }

    [Required] public string? Address1 { get; set; }

    public string? Address2 { get; set; }

    [Required] public string? City { get; set; }

    [Required] public States State { get; set; }

    [Required]
    [DisplayName("Zip Code")]
    [DataType(DataType.PostalCode)]
    public int ZipCode { get; set; }

    [Required] [EmailAddress] public string? Email { get; set; }

    [Required]
    [DisplayName("Phone Number")]
    public string? PhoneNumber { get; set; }

    [Required] [DataType(DataType.Date)] public DateTime Created { get; set; }

    // Image
    public byte[]? ImageData { get; set; }
    public string? ImageType { get; set; }

    [NotMapped] public IFormFile? ImageFile { get; set; }

    public virtual AppUserModel? AppUser { get; set; }

    public virtual ICollection<CategoryModel> Categories { get; set; } =
        new HashSet<CategoryModel>();
}