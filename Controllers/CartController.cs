using System.Threading.Tasks;
using Ecommerce.Models;
using Ecommerce.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Controllers;


public class CartController : Controller
{

    private readonly ICartService _cartService;
    public CartController(DataContext context, ICartService cartServic)
    {
        _cartService = cartServic;
    }

    public async Task<ActionResult> Index()
    {
        var customerId = _cartService.GetCustomerId();
        var cart = await _cartService.GetCart(customerId);
        return View(cart);
    }

    [HttpPost]
    public async Task<ActionResult> AddToCart(int urunId, int miktar = 1)
    {
        await _cartService.AddToCart(urunId, miktar);

        return RedirectToAction("Index", "Cart");
    }

    [HttpPost]
    public async Task<ActionResult> RemoveItem(int urunId, int miktar)
    {
        await _cartService.RemoveItem(urunId, miktar);

        return RedirectToAction("Index", "Cart");
    }

}