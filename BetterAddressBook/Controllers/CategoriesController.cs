using BetterAddressBook.Data;
using BetterAddressBook.Models;
using BetterAddressBook.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BetterAddressBook.Controllers;

[Authorize]
public class CategoriesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<AppUserModel> _userManager;
    private readonly IUserService _userService;

    public CategoriesController(ApplicationDbContext context, UserManager<AppUserModel> userManager, IUserService userService)
    {
        _context = context;
        _userManager = userManager;
        _userService = userService;
    }

    // GET: Categories
    public async Task<IActionResult> Index()
    {
        var appUserId = _userManager.GetUserId(User);
        var categories = await _userService.GetUserCategoriesAsync(appUserId);
        
        return View(categories);
    }

    // GET: Categories/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null || _context.Categories == null)
        {
            return NotFound();
        }

        var categoryModel = await _context.Categories
            .Include(c => c.AppUser)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (categoryModel == null)
        {
            return NotFound();
        }

        return View(categoryModel);
    }

    // GET: Categories/Create
    public IActionResult Create()
    {
        ViewData["AppUserId"] = new SelectList(_context.Users, "Id", "Id");
        return View();
    }

    // POST: Categories/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,AppUserId,Name")] CategoryModel categoryModel)
    {
        if (ModelState.IsValid)
        {
            _context.Add(categoryModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewData["AppUserId"] = new SelectList(_context.Users, "Id", "Id", categoryModel.AppUserId);
        return View(categoryModel);
    }

    // GET: Categories/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var appUserId = _userManager.GetUserId(User);
        var category = (await _userService
            .GetUserCategoriesAsync(appUserId))
            .FirstOrDefault(c => c.Id == id);
        
        if (category == null)
        {
            return NotFound();
        }

        ViewData["AppUserId"] = new SelectList(_context.Users, "Id", "Id", category.AppUserId);
        return View(category);
    }

    // POST: Categories/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,AppUserId,Name")] CategoryModel categoryModel)
    {
        if (id != categoryModel.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                var appUserId = _userManager.GetUserId(User);

                if (categoryModel.AppUserId == appUserId)
                {
                    categoryModel.AppUserId = appUserId;
                    _context.Update(categoryModel);
                    await _context.SaveChangesAsync();
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryModelExists(categoryModel.Id))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        return View(categoryModel);
    }

    // GET: Categories/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null || _context.Categories == null)
        {
            return NotFound();
        }

        var categoryModel = await _context.Categories
            .Include(c => c.AppUser)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (categoryModel == null)
        {
            return NotFound();
        }

        return View(categoryModel);
    }

    // POST: Categories/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (_context.Categories == null)
        {
            return Problem("Entity set 'ApplicationDbContext.Categories'  is null.");
        }

        var categoryModel = await _context.Categories.FindAsync(id);
        if (categoryModel != null)
        {
            _context.Categories.Remove(categoryModel);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool CategoryModelExists(int id)
    {
        return (_context.Categories?.Any(e => e.Id == id)).GetValueOrDefault();
    }

    [HttpGet]
    public IActionResult EmailCategory(int? id)
    {
        throw new NotImplementedException();
    }
}