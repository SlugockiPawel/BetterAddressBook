namespace BetterAddressBook.Models;

public class CustomError
{
    public int Code { get; set; }
    public string Message { get; set; } = string.Empty;
    public string ViewPath { get; set; } = string.Empty;
}