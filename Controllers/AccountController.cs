using System.Security.Claims;
using System.Threading.Tasks;
using Ecommerce.Models;
using Ecommerce.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

public class AccountController : Controller
{
    private readonly SignInManager<AppUser> _signInManager;
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;
    private readonly DataContext _context;
    private readonly ICartService _cartService;
    public AccountController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, DataContext context, ICartService cartService, RoleManager<AppRole> roleManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _context = context;
        _cartService = cartService;
        _roleManager = roleManager;
    }

    public ActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<ActionResult> Register(AccountCreateModel model)
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
                var entity = await _roleManager.RoleExistsAsync("User");

                if (!entity)
                {
                    var role = new AppRole { Name = "User" };
                    await _roleManager.CreateAsync(role);
                }

                await _userManager.AddToRoleAsync(user, "User");
                await _signInManager.SignInAsync(user, isPersistent: false);

                return RedirectToAction("Index", "Home");

            }

            foreach (var err in result.Errors)
            {
                ModelState.AddModelError("", err.Description);
            }

        }
        return View(model);
    }

    public ActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<ActionResult> Login(AccountLoginModel model, string? returnUrl)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user != null)
            {
                await _signInManager.SignOutAsync();

                var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.BeniHatirla, true);

                if (result.Succeeded)
                {
                    await _userManager.ResetAccessFailedCountAsync(user);
                    await _userManager.SetLockoutEndDateAsync(user, null);


                    await _cartService.TransferCartToUser(user.UserName!);
                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }

                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else if (result.IsLockedOut)
                {
                    var louckoutDate = await _userManager.GetLockoutEndDateAsync(user);
                    var timeLef = louckoutDate.Value - DateTime.UtcNow;
                    ModelState.AddModelError("", $"Hesabınız kilitlendi. lütfen {timeLef.Minutes + 1}");
                }
                else
                {
                    ModelState.AddModelError("", "Hatalı Parola");
                }
            }

            else
            {
                ModelState.AddModelError("", "Hatalı Email");
            }
        }
        return View(model);
    }

    public async Task<ActionResult> EditUser()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var user = await _userManager.FindByIdAsync(userId!);

        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        return View(new AccountEditUser
        {
            AdSoyad = user.FullName,
            Email = user.Email!,
        });
    }

    [HttpPost]
    public async Task<ActionResult> EditUser(AccountEditUser model)
    {
        if (ModelState.IsValid)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId!);

            if (user != null)
            {
                user.Email = model.Email;
                user.FullName = model.AdSoyad;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    TempData["Mesaj"] = "Bilgileriniz Güncellendi.";
                }
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }
            }
        }
        return View(model);
    }

    public ActionResult ChangePassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<ActionResult> ChangePassword(AccountChangePassword model)
    {
        if (ModelState.IsValid)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId!);

            if (user != null)
            {
                var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.Password);

                if (result.Succeeded)
                {
                    TempData["Mesaj"] = $"{user.FullName} isimli kullanıcı Şifre Güncellendi.";
                }

                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }
            }
        }
        return View(model);
    }

    public async Task<ActionResult> LogOut()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Register", "Account");
    }
}