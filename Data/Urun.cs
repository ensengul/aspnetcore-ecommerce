namespace Ecommerce.Models
{
    public class Urun
    {
        public int Id { get; set; }

        public string UrunAdi { get; set; } = null!;
        public double Fiyat { get; set; }
        public string? Aciklama { get; set; }
        public string Resim { get; set; } = null!;

        public Kategori Kategori { get; set; } = null!;
        public int KategoriId { get; set; }

    }
}