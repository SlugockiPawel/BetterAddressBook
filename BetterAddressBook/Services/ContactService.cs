using BetterAddressBook.Data;
using BetterAddressBook.Models;
using BetterAddressBook.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BetterAddressBook.Services;

public class ContactService : IContactService
{
    private readonly ApplicationDbContext _context;

    public ContactService(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task AddContactToCategoryAsync(int categoryId, int contactId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> IsContactInCategory(int categoryId, int contactId)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<ContactModel> SearchForContacts(string searchString, string userId)
    {
        throw new NotImplementedException();
    }

    public Task<ICollection<int>> GetContactCategoryIdsAsync(int contactId)
    {
        throw new NotImplementedException();
    }

    public Task<ICollection<CategoryModel>> GetContactCategoriesAsync(int contactId)
    {
        throw new NotImplementedException();
    }

    public Task RemoveContactFromCategoryAsync(int categoryId, int contactId)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<CategoryModel>> GetUserCategoriesAsync(string userId)
    {
        try
        {
            return await _context.Categories
                .Where(c => c.AppUserId == userId)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}