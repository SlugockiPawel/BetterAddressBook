using BetterAddressBook.Data;
using BetterAddressBook.Models;
using BetterAddressBook.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BetterAddressBook.Services;

public class CategoryService : ICategoryService
{
    private readonly ApplicationDbContext _context;

    public CategoryService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddCategory(CategoryModel category)
    {
        try
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task UpdateCategory(CategoryModel category)
    {
        try
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task RemoveCategory(CategoryModel category)
    {
        try
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public bool CategoryExists(int id)
    {
        try
        {
            return _context.Categories.Any(c => c.Id == id);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<CategoryModel?> GetCategoryWithContactsForUser(int categoryId, string userId)
    {
        try
        {
            return await _context.Categories
                .Include(c => c.Contacts)
                .FirstOrDefaultAsync(c => c.Id == categoryId && c.AppUserId == userId);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}