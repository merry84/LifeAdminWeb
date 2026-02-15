using LifeAdminModels.Models;
using Microsoft.AspNetCore.Identity;

namespace LifeAdmin.Web.Infrastructure
{
    public static class SeedData
    {
        public const string AdminRoleName = "Admin";

        public static async Task SeedAdminAsync(IServiceProvider services, IConfiguration config)
        {
            using var scope = services.CreateScope();

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            if (!await roleManager.RoleExistsAsync(AdminRoleName))
            {
                await roleManager.CreateAsync(new IdentityRole(AdminRoleName));
            }

            var email = config["AdminUser:Email"];
            var password = config["AdminUser:Password"];

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return; 

            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                var createResult = await userManager.CreateAsync(user, password);

                if (!createResult.Succeeded)
                {
                    var errors = string.Join("; ", createResult.Errors.Select(e => e.Description));
                    throw new InvalidOperationException("Admin seed failed: " + errors);
                }
            }
            if (!await userManager.IsInRoleAsync(user, AdminRoleName))
            {
                await userManager.AddToRoleAsync(user, AdminRoleName);
            }
        }
    }
}
