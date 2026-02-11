using System.Security.Claims;
using System.Threading.Tasks;
using Ecommerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Ecommerce.Controllers;

[Authorize]
public class AdminController : Controller
{

    private readonly DataContext _context;

    public AdminController(DataContext context)
    {
        _context = context;
    }


    public ActionResult Index()
    {
        var urun = _context.Urunler.Count();
        var satis = _context.Orders
                    .SelectMany(i => i.OrderItems)
                    .Sum(i => i.Fiyat * i.Miktar);
        var siparis = _context.Orders.Count();

        var orders = _context.Orders
                    .Include(i => i.OrderItems)
                    .OrderByDescending(i => i.SiparisTarihi)
                    .Take(5)
                    .ToList();


        var model = new Dashboard
        {
            Satis = satis,
            Urun = urun,
            Siparis = siparis,
            Orders = orders
        };
        return View(model);
    }



}
