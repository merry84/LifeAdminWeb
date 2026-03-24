using LifeAdminData;
using LifeAdminModels.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LifeAdmin.Web.Infrastructure
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            using var scope = serviceProvider.CreateScope();

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            await dbContext.Database.MigrateAsync();

            await SeedRolesAsync(roleManager);
            await SeedAdminAsync(userManager, configuration);
            await SeedCategoriesAsync(dbContext);
            await SeedTagsAsync(dbContext);
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "Administrator", "User" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        private static async Task SeedAdminAsync(
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration)
        {
            string adminEmail = configuration["AdminUser:Email"]!;
            string adminPassword = configuration["AdminUser:Password"]!;

            var user = await userManager.FindByEmailAsync(adminEmail);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    FirstName = "Admin",
                    LastName = "User"
                };

                var result = await userManager.CreateAsync(user, adminPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Administrator");
                }
            }
            else
            {
                if (!await userManager.IsInRoleAsync(user, "Administrator"))
                {
                    await userManager.AddToRoleAsync(user, "Administrator");
                }
            }
        }

        private static async Task SeedCategoriesAsync(ApplicationDbContext dbContext)
        {
            if (await dbContext.Categories.AnyAsync())
            {
                return;
            }

            var categories = new List<Category>
            {
                new Category { Name = "Work" },
                new Category { Name = "Personal" },
                new Category { Name = "Study" },
                new Category { Name = "Health" }
            };

            await dbContext.Categories.AddRangeAsync(categories);
            await dbContext.SaveChangesAsync();
        }

        private static async Task SeedTagsAsync(ApplicationDbContext dbContext)
        {
            if (await dbContext.Tags.AnyAsync())
            {
                return;
            }

            var tags = new List<Tag>
            {
                new Tag { Id = Guid.NewGuid(), Name = "Important" },
                new Tag { Id = Guid.NewGuid(), Name = "Work" },
                new Tag { Id = Guid.NewGuid(), Name = "Personal" }
            };

            await dbContext.Tags.AddRangeAsync(tags);
            await dbContext.SaveChangesAsync();
        }
    }
}