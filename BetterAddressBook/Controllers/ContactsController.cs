using BetterAddressBook.Data;
using BetterAddressBook.Enums;
using BetterAddressBook.Models;
using BetterAddressBook.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BetterAddressBook.Controllers;

[Authorize]
public class ContactsController : Controller
{
    private readonly IContactService _contactService;
    private readonly ApplicationDbContext _context;
    private readonly IImageService _imageService;
    private readonly UserManager<AppUserModel> _userManager;
    private readonly IUserService _userService;

    public ContactsController(
        ApplicationDbContext context,
        UserManager<AppUserModel> userManager,
        IImageService imageService,
        IContactService contactService,
        IUserService userService
    )
    {
        _context = context;
        _userManager = userManager;
        _imageService = imageService;
        _contactService = contactService;
        _userService = userService;
    }

    // GET: Contacts
    public IActionResult Index(int categoryId)
    {
        var userId = _userManager.GetUserId(User);
        var appUser = _userService.GetUserWithContactsAndCategories(userId);

        if (appUser is null)
        {
            return NotFound();
        }

        var categories = appUser.Contacts.SelectMany(c => c.Categories);

        List<ContactModel> orderedContacts;

        if (categoryId == 0)
        {
            orderedContacts = appUser.Contacts
                .OrderBy(c => c.LastName)
                .ThenBy(c => c.FirstName)
                .ToList();
        }
        else
        {
            orderedContacts = appUser.Contacts
                .Where(contact => contact.Categories.Any(category => category.Id == categoryId))
                .OrderBy(c => c.LastName)
                .ThenBy(c => c.FirstName)
                .ToList();
        }

        ViewData["CategoryId"] = new SelectList(categories, "Id", "Name", categoryId);

        return View(orderedContacts);
    }

    [Authorize]
    public IActionResult SearchContacts(string searchString)
    {
        var appUserId = _userManager.GetUserId(User);
        var appUser = _userService.GetUserWithContactsAndCategories(appUserId);

        if (appUser is null)
        {
            return NotFound();
        }

        List<ContactModel> orderedContacts;
        var categories = appUser.Contacts.SelectMany(c => c.Categories);

        if (string.IsNullOrWhiteSpace(searchString))
        {
            orderedContacts = appUser.Contacts
                .OrderBy(c => c.LastName)
                .ThenBy(c => c.FirstName)
                .ToList();
        }
        else
        {
            orderedContacts = appUser.Contacts
                .Where(u => u.FullName!.ToLower().Contains(searchString.ToLower()))
                .OrderBy(c => c.LastName)
                .ThenBy(c => c.FirstName)
                .ToList();
        }

        ViewData["CategoryId"] = new SelectList(categories, "Id", "Name");

        return View(nameof(Index), orderedContacts);
    }

    // GET: Contacts/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null || _context.Contacts == null)
        {
            return NotFound();
        }

        var contactModel = await _context.Contacts
            .Include(c => c.AppUser)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (contactModel == null)
        {
            return NotFound();
        }

        return View(contactModel);
    }

    // GET: Contacts/Create
    public async Task<IActionResult> Create()
    {
        var userId = _userManager.GetUserId(User);

        ViewData["StatesList"] = new SelectList(Enum.GetValues(typeof(States))); //.Cast<States>().ToList()
        ViewData["CategoryList"] = new MultiSelectList(
            await _userService.GetUserCategoriesAsync(userId),
            "Id",
            "Name"
        );

        return View();
    }

    // POST: Contacts/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind(
            "Id,FirstName,LastName,BirthDate,Address1,Address2,City,State,ZipCode,Email,PhoneNumber,ImageFile"
        )]
        ContactModel contactModel,
        List<int> categoryList
    )
    {
        ModelState.Remove("AppUserId");

        if (!ModelState.IsValid)
            return RedirectToAction(nameof(Index));

        contactModel.AppUserId = _userManager.GetUserId(User);
        contactModel.Created = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);

        if (contactModel.BirthDate is not null)
        {
            contactModel.BirthDate = DateTime.SpecifyKind(
                contactModel.BirthDate.Value,
                DateTimeKind.Utc
            );
        }

        // add image
        if (contactModel.ImageFile is not null)
        {
            contactModel.ImageData = await _imageService.ConvertToByteArrayAsync(
                contactModel.ImageFile
            );
            contactModel.ImageType = contactModel.ImageFile.ContentType;
        }

        _context.Add(contactModel);
        await _context.SaveChangesAsync();

        // add/remove categories

        foreach (var categoryId in categoryList)
        {
            await _contactService.AddContactToCategoryAsync(categoryId, contactModel.Id);
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: Contacts/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var appUserId = _userManager.GetUserId(User);

        var contactModel = await _contactService.GetContactForUser(id, appUserId);
        if (contactModel is null)
        {
            return NotFound();
        }

        ViewData["StatesList"] = new SelectList(Enum.GetValues<States>());
        ViewData["CategoryList"] = new MultiSelectList(
            await _userService.GetUserCategoriesAsync(appUserId),
            "Id",
            "Name",
            await _contactService.GetContactCategoryIdsAsync(contactModel.Id)
        );

        return View(contactModel);
    }

    // POST: Contacts/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        List<int> categoryList,
        [Bind(
            "Id,AppUserId,FirstName,LastName,BirthDate,Address1,Address2,City,State,ZipCode,Email,PhoneNumber,Created,ImageData,ImageType, ImageFile"
        )]
        ContactModel contactModel
    )
    {
        if (id != contactModel.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                contactModel.Created = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);

                if (contactModel.BirthDate is not null)
                {
                    contactModel.BirthDate = DateTime.SpecifyKind(contactModel.BirthDate.Value, DateTimeKind.Utc);
                }

                if (contactModel.ImageFile is not null)
                {
                    contactModel.ImageData = await _imageService.ConvertToByteArrayAsync(contactModel.ImageFile);
                    contactModel.ImageType = contactModel.ImageFile.ContentType;
                }
                
                _context.Update(contactModel);
                await _context.SaveChangesAsync();
                
                // save categories
                var oldCategoryIds = await _contactService
                    .GetContactCategoryIdsAsync(contactModel.Id);

                foreach (var categoryId in oldCategoryIds)
                {
                    await _contactService.RemoveContactFromCategoryAsync(categoryId, contactModel.Id);
                }

                foreach (var selectedCategoryId in categoryList)
                {
                    await _contactService.AddContactToCategoryAsync(selectedCategoryId, contactModel.Id);
                }

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContactModelExists(contactModel.Id))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        ViewData["AppUserId"] = new SelectList(_context.Users, "Id", "Id", contactModel.AppUserId);
        return View(contactModel);
    }

    // GET: Contacts/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null || _context.Contacts == null)
        {
            return NotFound();
        }

        var contactModel = await _context.Contacts
            .Include(c => c.AppUser)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (contactModel == null)
        {
            return NotFound();
        }

        return View(contactModel);
    }

    // POST: Contacts/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (_context.Contacts == null)
        {
            return Problem("Entity set 'ApplicationDbContext.Contacts'  is null.");
        }

        var contactModel = await _context.Contacts.FindAsync(id);
        if (contactModel != null)
        {
            _context.Contacts.Remove(contactModel);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool ContactModelExists(int id)
    {
        return (_context.Contacts?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}