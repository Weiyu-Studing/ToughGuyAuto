using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ToughGuyAuto.Models;

namespace ToughGuyAuto.DAL.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(
        IServiceProvider services)
    {
        var context =
            services.GetRequiredService<ToughGuyAutoDbContext>();

        var userManager =
            services.GetRequiredService<UserManager<ApplicationUser>>();

        var roleManager =
            services.GetRequiredService<RoleManager<IdentityRole>>();

        await context.Database.MigrateAsync();

        const string adminRole = "Admin";
        const string userRole = "User";

        if (!await roleManager.RoleExistsAsync(adminRole))
        {
            await roleManager.CreateAsync(
                new IdentityRole(adminRole));
        }

        if (!await roleManager.RoleExistsAsync(userRole))
        {
            await roleManager.CreateAsync(
                new IdentityRole(userRole));
        }

        var adminEmail = "admin@toughguyauto.com";

        var admin = await userManager.FindByEmailAsync(
            adminEmail);

        if (admin == null)
        {
            admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(
                admin,
                "Admin123!");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(
                    admin,
                    adminRole);
            }
        }

        var userEmail = "user@toughguyauto.com";

        var user = await userManager.FindByEmailAsync(
            userEmail);

        if (user == null)
        {
            user = new ApplicationUser
            {
                UserName = userEmail,
                Email = userEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(
                user,
                "User123!");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(
                    user,
                    userRole);
            }
        }
    }
}
