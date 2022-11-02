using BetterAddressBook.Data;
using BetterAddressBook.Models;
using BetterAddressBook.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BetterAddressBook.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;

    public UserService(ApplicationDbContext context)
    {
        _context = context;
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