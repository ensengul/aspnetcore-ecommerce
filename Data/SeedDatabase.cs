using AspNetCoreGeneratedDocument;
using Ecommerce.Models;
using Microsoft.AspNetCore.Identity;

namespace Ecommerce.Models;

public static class SeedDatabase
{
    public static async void Initialize(IApplicationBuilder app)
    {
        var userManager = app.ApplicationServices
                            .CreateScope()
                            .ServiceProvider
                            .GetRequiredService<UserManager<AppUser>>();

        var roleManager = app.ApplicationServices
                                    .CreateScope()
                                    .ServiceProvider
                                    .GetRequiredService<RoleManager<AppRole>>();

        if (!roleManager.Roles.Any())
        {
            var admin = new AppRole { Name = "Admin" };
            await roleManager.CreateAsync(admin);
        }

        if (!userManager.Users.Any())
        {
            var admin = new AppUser
            {
                FullName = "Admin",
                UserName = "admin",
                Email = "info@admin.com"
            };

            await userManager.CreateAsync(admin, "admin1234");
            await userManager.AddToRoleAsync(admin, "Admin");

            var customer = new AppUser
            {
                FullName = "User",
                UserName = "user",
                Email = "info@user.com"
            };

            await userManager.CreateAsync(customer, "user1234");

        }

    }

}

