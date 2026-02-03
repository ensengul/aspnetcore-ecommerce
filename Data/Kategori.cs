namespace Ecommerce.Models;



public class Kategori
{

    public int Id { get; set; }

    public string KategoriAdi { get; set; } = null!;
    public string Url { get; set; } = null!;

    public List<Urun> Urun { get; set; } = new();

}