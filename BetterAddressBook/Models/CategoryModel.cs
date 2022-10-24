using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BetterAddressBook.Models;

public class CategoryModel
{
    public int Id { get; set; }

    [Required] public string? AppUserId { get; set; }

    [Required]
    [DisplayName("Category Name")]
    public string? Name { get; set; }

    public virtual AppUserModel? AppUser { get; set; }
    public ICollection<ContactModel> Contacts { get; set; } = new HashSet<ContactModel>();
}