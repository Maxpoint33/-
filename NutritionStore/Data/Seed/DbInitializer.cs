using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NutritionStore.Models;

namespace NutritionStore.Data.Seed
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            await context.Database.MigrateAsync();

            if (!await roleManager.RoleExistsAsync("Admin"))
                await roleManager.CreateAsync(new IdentityRole("Admin"));

            if (!await roleManager.RoleExistsAsync("User"))
                await roleManager.CreateAsync(new IdentityRole("User"));

            string adminEmail = "admin@nutrition.com";
            string adminPass = "Admin123";

            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var admin = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(admin, adminPass);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }

            if (!context.Categories.Any())
            {
                context.Categories.AddRange(
                    new Category { Name = "Протеини" },
                    new Category { Name = "Аминокиселини" },
                    new Category { Name = "Витамини" },
                    new Category { Name = "Фетбърнъри" }
                );

                await context.SaveChangesAsync();
            }

            if (!context.Products.Any())
            {
                var proteinCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Протеини");

                context.Products.Add(new Product
                {
                    Name = "Whey Protein 2kg",
                    Description = "High quality whey protein for muscle growth.",
                    Price = 89.99M,
                    ImageUrl = "https://via.placeholder.com/300",
                    Category = proteinCategory!
                });

                await context.SaveChangesAsync();
            }
        }
    }
}
