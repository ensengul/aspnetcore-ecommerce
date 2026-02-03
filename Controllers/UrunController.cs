using System.Threading.Tasks;
using Ecommerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SQLitePCL;
namespace Ecommerce.Controllers;

using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.Rendering;

public class UrunController : Controller
{


    string DosyaAdiTemizle(string text)
    {
        text = text.ToLowerInvariant();
        text = Regex.Replace(text, @"[^a-z0-9\s-]", "");
        text = Regex.Replace(text, @"\s+", "-");
        return text;
    }

    private readonly DataContext _context;
    public UrunController(DataContext context)
    {
        _context = context;
    }

    [Authorize(Roles = "Admin")]
    public ActionResult Index()
    {
        var uruns = _context.Urunler
                    .Include(i => i.Kategori)
                    .ToList();
        return View(uruns);
    }

    public ActionResult Create()
    {
        ViewBag.kategoriler = new SelectList(_context.Kategoriler.ToList(), "Id", "KategoriAdi");
        return View();
    }

    [HttpPost]
    public async Task<ActionResult> Create(UrunCreateModel model)
    {
        if (ModelState.IsValid)
        {
            if (model.ResimDosyasi == null)
            {
                ModelState.AddModelError("", "Resim Seçiniz");
                return View(model);
            }
            var uzanti = Path.GetExtension(model.ResimDosyasi!.FileName); // yüklenen dosyanın uzantsını al

            var uzantilar = new[] { ".jpg", ".jpeg", ".png", ".webp" };

            if (!uzantilar.Contains(uzanti.ToLower()))
            {
                ModelState.AddModelError("", "Geçersiz resim formatı");
                return View(model);
            }

            var fileName = $"{DosyaAdiTemizle(model.UrunAdi!)}-{Guid.NewGuid()}{uzanti}";

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", fileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await model.ResimDosyasi!.CopyToAsync(stream);
            }
            model.Resim = fileName;


            var urun = new Urun
            {
                UrunAdi = model.UrunAdi!,
                Fiyat = model.Fiyat,
                Aciklama = model.Aciklama,
                Resim = model.Resim!,
                KategoriId = model.KategoriId
            };
            _context.Urunler.Add(urun);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Urun");
        }
        ViewBag.kategoriler = new SelectList(_context.Kategoriler.ToList(), "Id", "KategoriAdi");
        return View(model);
    }


    public ActionResult Edit(int id)
    {
        var urun = _context.Urunler.FirstOrDefault(i => i.Id == id);

        var result = new UrunEditModel()
        {
            UrunAdi = urun!.UrunAdi,
            Fiyat = urun.Fiyat,
            Aciklama = urun.Aciklama,
            Resim = urun.Resim,
            KategoriId = urun.KategoriId
        };
        ViewBag.kategoriler = new SelectList(_context.Kategoriler.ToList(), "Id", "KategoriAdi", result.KategoriId);
        return View(result);
    }

    [HttpPost]
    public async Task<ActionResult> Edit(UrunEditModel model, int id)
    {
        if (ModelState.IsValid)
        {
            var urun = _context.Urunler.Find(id);
            if (urun != null)
            {
                if (model.ResimDosyasi != null)
                {
                    var uzanti = Path.GetExtension(model.ResimDosyasi.FileName);

                    var uzantilar = new[] { ".jpg", ".jpeg", ".png", ".webp" };

                    if (!uzantilar.Contains(uzanti.ToLower()))
                    {
                        ModelState.AddModelError("", "Geçersiz Resim Formatı");
                        ViewBag.kategoriler = new SelectList(_context.Kategoriler.ToList(), "Id", "KategoriAdi", model.KategoriId);
                        return View(model);
                    }


                    var fileName = $"{DosyaAdiTemizle(model.UrunAdi!)}-{Guid.NewGuid()}{uzanti}";

                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", fileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await model.ResimDosyasi.CopyToAsync(stream);
                    }

                    model.Resim = fileName;
                }

                urun.UrunAdi = model.UrunAdi!;
                urun.Fiyat = model.Fiyat;
                urun.Aciklama = model.Aciklama;
                urun.Resim = model.Resim!;
                urun.KategoriId = model.KategoriId;

                _context.SaveChanges();

                TempData["Mesaj"] = $"{urun.UrunAdi} Güncellendi.";

                return RedirectToAction("Index", "Urun");

            }

        }
        ViewBag.kategoriler = new SelectList(_context.Kategoriler.ToList(), "Id", "KategoriAdi", model.KategoriId);
        return View(model);
    }


    public ActionResult Details(int id)
    {
        var urun = _context.Urunler.FirstOrDefault(i => i.Id == id);
        TempData["BenzerUrunler"] = _context.Urunler.Where(i => i.KategoriId == urun!.KategoriId).ToList();
        return View(urun);
    }


    public ActionResult List(string q, string url)
    {
        if (!string.IsNullOrEmpty(q))
        {
            var query = _context.Urunler.Where(a => a.UrunAdi.ToLower().Contains(q.ToLower())).ToList();
            ViewData["q"] = q;
            return View(query);

        }

        if (!string.IsNullOrEmpty(url))
        {
            var query = _context.Urunler.Where(i => i.Kategori.Url == url).ToList();
            return View(query);
        }
        return View();
    }



    public ActionResult Delete(int id)
    {
        var urun = _context.Urunler.Find(id);

        if (urun == null)
        {
            return NotFound();
        }
        return View(urun);
    }

    [HttpPost]
    public ActionResult DeleteConfirm(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        var urun = _context.Urunler.Find(id);
        if (urun != null)
        {
            _context.Urunler.Remove(urun);
            _context.SaveChanges();
            TempData["Mesaj"] = $"{urun.UrunAdi} isimli ürün silindi.";
        }
        return RedirectToAction("Index");
    }

}




