namespace Ecommerce.Models;


public class Dashboard
{
    public double Satis { get; set; }

    public double Siparis { get; set; }

    public int Urun { get; set; }

    public List<Order> Orders { get; set; } = new();

}