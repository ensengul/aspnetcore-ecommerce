using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models;



public class AccountCreateModel
{

    [Required]
    [Display(Name = "Ad Soyad")]
    public string FullName { get; set; } = null!;

    [Required]
    [Display(Name = "Email")]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    [Display(Name = "Parola")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;

    [Required]
    [Display(Name = "Parola Tekrarı")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Parola Eşlemiyor")]
    public string PasswordConfirm { get; set; } = null!;

}