using BetterAddressBook.Models;

namespace BetterAddressBook.Services.Interfaces;

public interface IContactService
{
    Task AddContactToCategoryAsync(int categoryId, int contactId);
    Task<bool> IsContactInCategory(int categoryId, int contactId);
    IEnumerable<ContactModel> SearchForContacts(string searchString, string userId);
    Task<ICollection<int>> GetContactCategoryIdsAsync(int contactId);
    Task<ICollection<CategoryModel>?> GetContactCategoriesAsync(int contactId);
    Task RemoveContactFromCategoryAsync(int categoryId, int contactId);
    bool IsContactInCategory(CategoryModel category, ContactModel contact);
    Task<CategoryModel?> GetCategoryWithContacts(int categoryId);
    Task<ContactModel?> GetContactForUser(int? contactId, string userId);
}