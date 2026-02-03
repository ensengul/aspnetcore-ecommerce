using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models;


public class UserEditModel
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "Ad Soyad")]
    public string FullName { get; set; } = null!;


    [Required]
    [Display(Name = "Email")]
    public string Email { get; set; } = null!;


    [Display(Name = "Parola")]
    [DataType(DataType.Password)]
    public string? Password { get; set; }


    [Display(Name = "Parola Tekrarı")]
    [DataType(DataType.Password)]
    public string? PasswordConfirm { get; set; }


    [Required(ErrorMessage = "Rol seçemk zorunlu")]
    [Display(Name = "Role")]
    public IList<string>? SelectedRoles { get; set; }
}