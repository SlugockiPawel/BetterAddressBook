using BetterAddressBook.Models;

namespace BetterAddressBook.Services.Interfaces;

public interface IContactService
{
    Task AddContactToCategoryAsync(int categoryId, int contactId);
    Task<bool> IsContactInCategory(int categoryId, int contactId);
    IEnumerable<ContactModel> SearchForContacts(string searchString, string userId);
    Task<ICollection<int>> GetContactCategoryIdsAsync(int contactId);
    Task<ICollection<CategoryModel>> GetContactCategoriesAsync(int contactId);
    Task RemoveContactFromCategoryAsync(int categoryId, int contactId);

    //TODO try to add this method to UserManager as extension
    Task<IEnumerable<CategoryModel>> GetUserCategoriesAsync(string userId);
}