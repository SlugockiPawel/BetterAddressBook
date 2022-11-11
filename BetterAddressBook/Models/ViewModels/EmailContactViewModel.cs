namespace BetterAddressBook.Models.ViewModels;

public class EmailContactViewModel
{
    public ContactModel? ContactModel { get; set; }
    public EmailData EmailData { get; set; } = new();
}