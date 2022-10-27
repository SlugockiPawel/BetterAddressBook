using BetterAddressBook.Models;

namespace BetterAddressBook.Services.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<CategoryModel>> GetCategoriesForUserAsync(string userId);
}