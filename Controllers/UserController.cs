using System.Threading.Tasks;
using Ecommerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;


[Authorize]
public class UserController : Controller
{

    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;

    public UserController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<ActionResult> Index()
    {
        var users = _userManager.Users.ToList();

        var model = new List<UserGetModel>();


        foreach (var user in users)
        {
            var role = await _userManager.GetRolesAsync(user);


            model.Add(new UserGetModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = role.FirstOrDefault()
            });
        }

        return View(model);
    }


    public ActionResult Create()
    {
        ViewBag.Roles = new SelectList(_roleManager.Roles.ToList(), "Name");
        return View();
    }

    [HttpPost]
    public async Task<ActionResult> Create(UserCreateModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new AppUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName
            };


            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, model.SelectedRole!);

                return RedirectToAction("Index");
            }

            foreach (var err in result.Errors)
            {
                ModelState.AddModelError("", err.Description);
            }


        }
        return View(model);
    }

    public async Task<ActionResult> Edit(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
        {
            return RedirectToAction("Index");
        }

        ViewBag.Roles = await _roleManager.Roles.Select(i => i.Name).ToListAsync(); // buna bak

        return View(
            new UserEditModel
            {
                FullName = user.FullName,
                Email = user.Email!,
                SelectedRoles = await _userManager.GetRolesAsync(user)
            }
        );
    }

    [HttpPost]
    public async Task<ActionResult> Edit(UserEditModel model, string id)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user != null)
            {
                user.Email = model.Email;
                user.FullName = model.FullName;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded && !string.IsNullOrEmpty(model.Password))
                {
                    // parola güncelle
                    await _userManager.RemovePasswordAsync(user);
                    await _userManager.AddPasswordAsync(user, model.Password);
                }

                if (result.Succeeded)
                {
                    await _userManager.RemoveFromRolesAsync(user, await _userManager.GetRolesAsync(user));
                    if (model.SelectedRoles != null)
                    {
                        await _userManager.AddToRolesAsync(user, model.SelectedRoles);
                    }
                    return RedirectToAction("Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
        }
        ViewBag.Roles = await _roleManager.Roles.Select(i => i.Name).ToListAsync();
        return View(model);
    }


    public async Task<ActionResult> Delete(string id)
    {
        if (id == null)
        {
            return RedirectToAction("Index");
        }

        var entity = await _userManager.FindByIdAsync(id);

        if (entity != null)
        {
            return View(entity);
        }
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<ActionResult> DeleteConfirm(string id)
    {
        if (id == null)
        {
            return RedirectToAction("Index");
        }

        var entity = await _userManager.FindByIdAsync(id);

        if (entity != null)
        {
            var result = await _userManager.DeleteAsync(entity);

            if (result.Succeeded)
            {
                TempData["Mesaj"] = $"{entity.FullName} isimli kişi silindi.";
            }

        }
        return RedirectToAction("Index");
    }


}