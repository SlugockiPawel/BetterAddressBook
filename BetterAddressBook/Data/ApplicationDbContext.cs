using BetterAddressBook.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BetterAddressBook.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUserModel>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<CategoryModel> Categories { get; set; }
        public virtual DbSet<ContactModel> Contacts { get; set; }
    }
}