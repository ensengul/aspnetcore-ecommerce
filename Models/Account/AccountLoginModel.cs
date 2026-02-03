using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models;



public class AccountLoginModel
{

    [Required]
    [Display(Name = "Email")]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    [Display(Name = "Parola")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;
    public bool BeniHatirla { get; set; }


}