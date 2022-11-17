namespace BetterAddressBook.Models.ViewModels;

public class EmailCategoryViewModel
{
    public List<ContactModel> Contacts { get; set; } = new();
    public EmailData? EmailData { get; set; }
    
}