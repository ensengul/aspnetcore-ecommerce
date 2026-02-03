using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models;

public class AccountEditUser
{

    [Required]
    [Display(Name = "Ad Soyad")]
    public string AdSoyad { get; set; } = null!;

    [Required]
    [Display(Name = "Email")]
    [EmailAddress]
    public string Email { get; set; } = null!;

}