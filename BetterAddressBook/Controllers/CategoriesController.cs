using BetterAddressBook.Models;
using BetterAddressBook.Models.ViewModels;
using BetterAddressBook.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BetterAddressBook.Controllers;

[Authorize]
public class CategoriesController : Controller
{
    private readonly ICategoryService _categoryService;
    private readonly IEmailSender _emailService;
    private readonly UserManager<AppUserModel> _userManager;
    private readonly IUserService _userService;

    public CategoriesController(
        UserManager<AppUserModel> userManager,
        IUserService userService,
        ICategoryService categoryService, IEmailSender emailService)
    {
        _userManager = userManager;
        _userService = userService;
        _categoryService = categoryService;
        _emailService = emailService;
    }

    // GET: Categories
    public async Task<IActionResult> Index(string? swalMessage = null)
    {
        ViewData["SwalMessage"] = swalMessage;
        var appUserId = _userManager.GetUserId(User);
        var categories = await _userService.GetUserCategoriesAsync(appUserId);

        return View(categories);
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

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> EmailCategory(int id)
    {
        var appUserId = _userManager.GetUserId(User);
        var category = await _categoryService.GetCategoryWithContactsForUser(id, appUserId);

        if (category is null)
        {
            return NotFound();
        }

        var emails = category.Contacts.Select(c => c.Email).ToList();

        EmailCategoryViewModel model = new()
        {
            Contacts = category.Contacts.ToList(),
            EmailData = new EmailData
            {
                GroupName = category.Name,
                EmailAddress = string.Join(";", emails),
                Subject = $"Group Name: {category.Name}"
            }
        };

        return View(model);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EmailCategory(EmailCategoryViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await _emailService.SendEmailAsync(model.EmailData?.EmailAddress, model.EmailData?.Subject,
                    model.EmailData?.Body);

                return RedirectToAction("Index", "Categories", new { swalMessage = "Success: Email Sent!" });
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Categories", new { swalMessage = "Error: Email Send Failed!" });
            }
        }

        return View(model);
    }
}