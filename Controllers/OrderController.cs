using System.Threading.Tasks;
using Ecommerce.Models;
using Ecommerce.Services;
using Iyzipay;
using Iyzipay.Model;
using Iyzipay.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Ecommerce.Controllers;

[Authorize]
public class OrderController : Controller
{

    private readonly DataContext _context;
    private readonly ICartService _cartService;
    private readonly IConfiguration _configuration;

    public OrderController(DataContext context, ICartService cartService, IConfiguration configuration)
    {
        _context = context;
        _cartService = cartService;
        _configuration = configuration;

    }


    public ActionResult Index()
    {

        return View(_context.Orders.ToList());
    }
    public async Task<ActionResult> Details(int id)
    {
        var order = await _context.Orders
            .Include(i => i.OrderItems)
            .ThenInclude(i => i.Urun)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (order == null)
        {
            return NotFound();
        }
        return View(order);
    }



    public async Task<ActionResult> CheckOut()
    {
        ViewBag.Cart = await _cartService.GetCart(User.Identity?.Name!);
        return View();
    }

    [HttpPost]
    public async Task<ActionResult> CheckOut(OrderCreateModel model)
    {
        var username = User.Identity?.Name;
        var cart = await _cartService.GetCart(username!);

        if (cart.CartItems.Count == 0)
        {
            ModelState.AddModelError("", "Sepetinizde Ürün yok");
        }

        if (ModelState.IsValid)
        {

            var order = new Order
            {
                AdSoyad = model.AdSoyad,
                Telefon = model.Telefon,
                AdresSatiri = model.AdresSatiri,
                PostaKodu = model.PostaKodu,
                Sehir = model.Sehir,
                SiparisNotu = model.SiparisNotu!,
                SiparisTarihi = DateTime.Now,
                ToplamFiyat = cart.Toplam(),
                Username = username!,
                OrderItems = cart.CartItems.Select(i => new Models.OrderItem
                {
                    UrunId = i.UrunId,
                    Fiyat = i.Urun.Fiyat,
                    Miktar = i.Miktar

                }).ToList()
            };


            var payment = await ProcessPayment(model, cart);

            if (payment.Status == "success")
            {
                _context.Orders.Add(order);
                _context.Carts.Remove(cart);

                await _context.SaveChangesAsync();

                return RedirectToAction("Completed", new { orderId = order.Id });
            }

            else
            {
                ModelState.AddModelError("", payment.ErrorMessage);
            }

        }
        ViewBag.Cart = cart;
        return View(model);
    }

    private async Task<Payment> ProcessPayment(OrderCreateModel model, Cart cart)
    {
        Options options = new Options();
        options.ApiKey = _configuration["PaymentAPI:APIKey"]; // burada neden configuration yazıyoruz
        options.SecretKey = _configuration["PaymentAPI:SecretKey"];
        options.BaseUrl = "https://sandbox-api.iyzipay.com";

        CreatePaymentRequest request = new CreatePaymentRequest();
        request.Locale = Locale.TR.ToString();
        request.ConversationId = Guid.NewGuid().ToString();
        request.Price = cart.Toplam().ToString();
        request.PaidPrice = cart.Toplam().ToString();
        request.Currency = Currency.TRY.ToString();
        request.Installment = 1;
        request.BasketId = "B67832";
        request.PaymentChannel = PaymentChannel.WEB.ToString();
        request.PaymentGroup = PaymentGroup.PRODUCT.ToString();

        PaymentCard paymentCard = new PaymentCard();
        paymentCard.CardHolderName = model.CartName;
        paymentCard.CardNumber = model.CartNumber;
        paymentCard.ExpireMonth = model.CartExpirationMonth;
        paymentCard.ExpireYear = model.CartExpirationYear;
        paymentCard.Cvc = model.CartCVV;
        paymentCard.RegisterCard = 0;
        request.PaymentCard = paymentCard;

        Buyer buyer = new Buyer();
        buyer.Id = "BY789";
        buyer.Name = model.AdSoyad;
        buyer.Surname = "Doe";
        buyer.GsmNumber = model.Telefon;
        buyer.Email = "email@email.com";
        buyer.IdentityNumber = "74300864791";
        buyer.LastLoginDate = "2015-10-05 12:43:35";
        buyer.RegistrationDate = "2013-04-21 15:12:09";
        buyer.RegistrationAddress = "Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1";
        buyer.Ip = "85.34.78.112";
        buyer.City = model.Sehir;
        buyer.Country = "Turkey";
        buyer.ZipCode = model.PostaKodu;
        request.Buyer = buyer;

        Address address = new Address();
        address.ContactName = model.AdSoyad;
        address.City = model.Sehir;
        address.Country = "Turkey";
        address.Description = model.AdresSatiri;
        address.ZipCode = model.PostaKodu;
        request.ShippingAddress = address;
        request.BillingAddress = address;

        List<BasketItem> basketItems = new List<BasketItem>();

        foreach (var item in cart.CartItems)
        {
            BasketItem basketItem = new BasketItem();
            basketItem.Id = item.CartItemId.ToString();
            basketItem.Name = item.Urun.UrunAdi;
            basketItem.Category1 = "Bileklik";
            basketItem.ItemType = BasketItemType.PHYSICAL.ToString();
            basketItem.Price = (item.Urun.Fiyat * 1.2).ToString();

            basketItems.Add(basketItem);
        }


        request.BasketItems = basketItems;

        return await Payment.Create(request, options);
    }

    public ActionResult Completed(string orderId)
    {
        return View("Completed", orderId);
    }

    public async Task<ActionResult> OrderList()
    {
        var username = User.Identity?.Name!;
        var order = await _context.Orders
                    .Include(i => i.OrderItems)
                    .ThenInclude(i => i.Urun)
                    .Where(i => i.Username == username)
                    .ToListAsync();
        return View(order);
    }


}