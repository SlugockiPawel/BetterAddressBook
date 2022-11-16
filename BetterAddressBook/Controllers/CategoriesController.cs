using BetterAddressBook.Models;
using BetterAddressBook.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BetterAddressBook.Controllers;

[Authorize]
public class CategoriesController : Controller
{
    private readonly ICategoryService _categoryService;
    private readonly UserManager<AppUserModel> _userManager;
    private readonly IUserService _userService;

    public CategoriesController(
        UserManager<AppUserModel> userManager,
        IUserService userService,
        ICategoryService categoryService
    )
    {
        _userManager = userManager;
        _userService = userService;
        _categoryService = categoryService;
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
        if (id == null)
        {
            return NotFound();
        }

        var appUserId = _userManager.GetUserId(User);
        var category = (await _userService.GetUserCategoriesAsync(appUserId)).FirstOrDefault(
            c => c.AppUserId == appUserId
        );

        if (category == null)
        {
            return NotFound();
        }

        return View(category);
    }

    // GET: Categories/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Categories/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name")] CategoryModel categoryModel)
    {
        ModelState.Remove("AppUserId");
        if (ModelState.IsValid)
        {
            var userId = _userManager.GetUserId(User);
            categoryModel.AppUserId = userId;
            await _categoryService.AddCategory(categoryModel);

            return RedirectToAction(nameof(Index));
        }

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
        var category = (await _userService.GetUserCategoriesAsync(appUserId)).FirstOrDefault(
            c => c.Id == id
        );

        if (category == null)
        {
            return NotFound();
        }

        return View(category);
    }

    // POST: Categories/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        [Bind("Id,AppUserId,Name")] CategoryModel categoryModel
    )
    {
        if (id != categoryModel.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                var userId = _userManager.GetUserId(User);

                if (categoryModel.AppUserId == userId)
                {
                    await _categoryService.UpdateCategory(categoryModel);
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
        if (id == null)
        {
            return NotFound();
        }

        var appUserId = _userManager.GetUserId(User);
        var category = (await _userService.GetUserCategoriesAsync(appUserId)).FirstOrDefault(
            c => c.Id == id
        );

        if (category == null)
        {
            return NotFound();
        }

        return View(category);
    }

    // POST: Categories/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var appUserId = _userManager.GetUserId(User);
        var category = (await _userService.GetUserCategoriesAsync(appUserId)).FirstOrDefault(
            c => c.Id == id
        );

        if (category != null)
        {
            await _categoryService.RemoveCategory(category);
        }

        return RedirectToAction(nameof(Index));
    }

    private bool CategoryModelExists(int id)
    {
        return _categoryService.CategoryExists(id);
    }

    [HttpGet]
    public IActionResult EmailCategory(int? id)
    {
        throw new NotImplementedException();
    }
}