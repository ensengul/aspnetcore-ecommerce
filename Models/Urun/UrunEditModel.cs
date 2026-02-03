using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models;


public class UrunEditModel
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "Urun Adı")]
    public string? UrunAdi { get; set; }

    [Required]
    [Display(Name = "Fiyat")]
    public double Fiyat { get; set; }

    [Required]
    [Display(Name = "Açıklama")]
    public string? Aciklama { get; set; }

    public string? Resim { get; set; }


    public IFormFile? ResimDosyasi { get; set; }

    public int KategoriId { get; set; }

}