using BetterAddressBook.Models;

namespace BetterAddressBook.Services.Interfaces;

public interface IUserService
{
    Task<IEnumerable<CategoryModel>> GetUserCategoriesAsync(string userId);
}