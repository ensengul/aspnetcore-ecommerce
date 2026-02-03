using Microsoft.AspNetCore.Identity;

namespace Ecommerce.Models;



public class AppUser : IdentityUser<int>

{
    public string FullName { get; set; } = null!;
}