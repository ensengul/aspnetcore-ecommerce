using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models;



public class AccountChangePassword
{
    [Required]
    [Display(Name = "Eski Parola")]
    [DataType(DataType.Password)]
    public string OldPassword { get; set; } = null!;
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