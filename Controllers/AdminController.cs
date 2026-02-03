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


    public async Task<ActionResult> Index()
    {
        var context = await _context.Orders
                       .Include(i => i.OrderItems)
                       .ThenInclude(i => i.Urun)
                       .ToListAsync();

        return View(context);
    }



}