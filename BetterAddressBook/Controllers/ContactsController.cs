using BetterAddressBook.Data;
using BetterAddressBook.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BetterAddressBook.Controllers;

[Authorize]
public class ContactsController : Controller
{
    private readonly ApplicationDbContext _context;

    public ContactsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Contacts
    public async Task<IActionResult> Index()
    {
        var applicationDbContext = _context.Contacts.Include(c => c.AppUser);
        return View(await applicationDbContext.ToListAsync());
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
    public IActionResult Create()
    {
        ViewData["AppUserId"] = new SelectList(_context.Users, "Id", "Id");
        return View();
    }

    // POST: Contacts/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind(
            "Id,AppUserId,FirstName,LastName,BirthDate,Address1,Address2,City,State,ZipCode,Email,PhoneNumber,Created,ImageData,ImageType"
        )]
        ContactModel contactModel
    )
    {
        if (ModelState.IsValid)
        {
            _context.Add(contactModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewData["AppUserId"] = new SelectList(
            _context.Users,
            "Id",
            "Id",
            contactModel.AppUserId
        );
        return View(contactModel);
    }

    // GET: Contacts/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || _context.Contacts == null)
        {
            return NotFound();
        }

        var contactModel = await _context.Contacts.FindAsync(id);
        if (contactModel == null)
        {
            return NotFound();
        }

        ViewData["AppUserId"] = new SelectList(
            _context.Users,
            "Id",
            "Id",
            contactModel.AppUserId
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
        [Bind(
            "Id,AppUserId,FirstName,LastName,BirthDate,Address1,Address2,City,State,ZipCode,Email,PhoneNumber,Created,ImageData,ImageType"
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
                _context.Update(contactModel);
                await _context.SaveChangesAsync();
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

        ViewData["AppUserId"] = new SelectList(
            _context.Users,
            "Id",
            "Id",
            contactModel.AppUserId
        );
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