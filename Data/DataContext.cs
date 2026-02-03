using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Models;

public class DataContext : IdentityDbContext<AppUser, AppRole, int>
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
        // Program.cs’te tanımlanan veritabanı ayarlarını alıp, EF Core’un DbContext sınıfına iletmek.
    }

    public DbSet<Urun> Urunler { get; set; }
    public DbSet<Kategori> Kategoriler { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<Order> Orders { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Urun>().HasData(
            new List<Urun>
            {
              new Urun(){Id=1,UrunAdi="Telefon",Resim="iphone-17.jpg",Aciklama ="Lorem ipsum dolor sit amet consectetur adipisicing elit",Fiyat=600, KategoriId=1},
              new Urun(){Id=2,UrunAdi="Laptop",Resim="MSI.jpg",Aciklama =" Lorem ipsum dolor sit amet consectetur adipisicing elit",Fiyat=600,KategoriId=2},
              new Urun(){Id=3,UrunAdi="Tablet",Aciklama=" Lorem ipsum dolor sit amet consectetur adipisicing elit",Resim="Samsung-Tablet.jpg",Fiyat=700,KategoriId=3},
              new Urun(){Id=4,UrunAdi="Kulaklık",Aciklama=" Lorem ipsum dolor sit amet consectetur adipisicing elit.",Resim="jbl.jpg",Fiyat=4000,KategoriId=4},
              new Urun(){Id=5,UrunAdi="Televizyon",Aciklama=" Lorem ipsum dolor sit amet consectetur adipisicing elit.",Resim="Sunny 65.jpg",Fiyat=5000,KategoriId=5},

            }
        );


        modelBuilder.Entity<Kategori>().HasData(
            new List<Kategori>
            {
                new Kategori(){Id=1,KategoriAdi="Telefon",Url="telefon"},
                new Kategori(){Id=2,KategoriAdi="Laptop",Url="laptop"},
                new Kategori(){Id=3,KategoriAdi="Tablet",Url="tablet"},
                new Kategori(){Id=4,KategoriAdi="Kulaklık",Url="kulaklık"},
                new Kategori(){Id=5,KategoriAdi="Televizyon",Url="televizyon"},
            }
        );
    }



}
