using System.Threading.Tasks;
using Ecommerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Ecommerce.Controllers;

[Authorize]
public class KategoriController : Controller
{

    private readonly DataContext _context;

    public KategoriController(DataContext context)
    {
        _context = context;
    }


    public async Task<ActionResult> Index()
    {
        var kategoriler = await _context.Kategoriler.Select(i => new KategoriGetModel
        {
            Id = i.Id,
            KategoriAdi = i.KategoriAdi,
            Url = i.Url,
            UrunSayisi = i.Urun.Count
        }).ToListAsync();

        return View(kategoriler);
    }

    public ActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public ActionResult Create(KategoriCreateModel model)
    {
        if (ModelState.IsValid)
        {
            var kategori = new Kategori
            {
                KategoriAdi = model.KategoriAdi,
                Url = model.Url
            };

            _context.Kategoriler.Add(kategori);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        return View(model);
    }

    public ActionResult Edit(int id)
    {
        var kategori = _context.Kategoriler.Select(i => new KategoriEditModel
        {
            KategoriAdi = i.KategoriAdi,
            Url = i.Url
        }).FirstOrDefault(i => i.Id == id);

        return View(kategori);
    }

    [HttpPost]
    public ActionResult Edit(KategoriEditModel model, int id)
    {
        if (ModelState.IsValid)
        {
            var kategori = _context.Kategoriler.Find(id);

            if (kategori != null)
            {
                kategori.Url = model.Url;
                kategori.KategoriAdi = model.KategoriAdi;

                _context.SaveChanges();

                TempData["Mesaj"] = $"{kategori.KategoriAdi} güncellendi.";

                return RedirectToAction("Index");
            }
        }
        return View(model);
    }

    public ActionResult Delete(int? id)
    {
        if (id == null)
        {
            return RedirectToAction("Index");
        }

        var entity = _context.Kategoriler.FirstOrDefault(i => i.Id == id);

        if (entity != null)
        {
            return View(entity);
        }
        return RedirectToAction("Index");
    }

    [HttpPost]
    public ActionResult DeleteConfirm(int? id)
    {
        if (id == null)
        {
            return RedirectToAction("Index");
        }

        var entity = _context.Kategoriler.FirstOrDefault(i => i.Id == id);

        if (entity != null)
        {
            _context.Kategoriler.Remove(entity);
            _context.SaveChanges();

            TempData["Mesaj"] = $"{entity.KategoriAdi} kategorisi silindi.";
        }
        return RedirectToAction("Index");
    }
}