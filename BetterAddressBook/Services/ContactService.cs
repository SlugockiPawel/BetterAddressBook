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

    public async Task AddContactToCategoryAsync(int categoryId, int contactId)
    {
        try
        {
            var contact = await _context.Contacts.FindAsync(contactId);
            var category = await GetCategoryWithContacts(categoryId);
            
            if (contact is not null && category is not null && !IsContactInCategory(category, contact))
            {
                category.Contacts.Add(contact);
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<bool> IsContactInCategory(int categoryId, int contactId)
    {
        var contact = await _context.Contacts.FindAsync(contactId);

        return contact is not null && await _context.Categories
            .Include(c => c.Contacts)
            .AnyAsync(c => c.Id == categoryId && c.Contacts.Contains(contact));
    }
    
    public bool IsContactInCategory(CategoryModel category, ContactModel contact)
    {
        return category.Contacts.Contains(contact);
    }

    public async Task<CategoryModel?> GetCategoryWithContacts(int categoryId)
    {
        try
        {
            return await _context.Categories
                    .Include(c => c.Contacts)
                    .FirstOrDefaultAsync(c => c.Id == categoryId);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<ContactModel?> GetContactForUser(int? contactId, string userId)
    {
        try
        {
            return await _context.Contacts
                .FirstOrDefaultAsync(c => c.Id == contactId && c.AppUserId == userId);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
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