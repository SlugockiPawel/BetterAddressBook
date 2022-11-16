using BetterAddressBook.Models;

namespace BetterAddressBook.Services.Interfaces;

public interface ICategoryService
{
    Task AddCategory(CategoryModel category);
    Task UpdateCategory(CategoryModel category);
    Task RemoveCategory(CategoryModel category);
    bool CategoryExists(int id);
}