namespace BetterAddressBook.Models.ViewModels;

public class EmailCategoryViewModel
{
    public HashSet<ContactModel> Contacts { get; set; } = new();
    public EmailData? EmailData { get; set; }
    
}