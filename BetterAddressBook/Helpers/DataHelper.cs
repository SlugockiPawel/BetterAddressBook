using BetterAddressBook.Data;
using Microsoft.EntityFrameworkCore;

namespace BetterAddressBook.Helpers;

public static class DataHelper
{
    public static async Task ManageDataAsync(IServiceProvider serviceProvider)
    {
        var dbContextService = serviceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContextService.Database.MigrateAsync();
    }
}