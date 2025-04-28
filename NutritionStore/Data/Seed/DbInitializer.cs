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

            // Seed Roles
            if (!await roleManager.RoleExistsAsync("Admin"))
                await roleManager.CreateAsync(new IdentityRole("Admin"));

            if (!await roleManager.RoleExistsAsync("User"))
                await roleManager.CreateAsync(new IdentityRole("User"));

            // Seed Admin
            const string adminEmail = "admin@nutrition.com";
            const string adminPass = "Admin123";

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

            // Seed Categories
            if (!context.Categories.Any())
            {
                context.Categories.AddRange(
                    new Category { Name = "Proteins" },
                    new Category { Name = "Amino Acids" },
                    new Category { Name = "Vitamins" },
                    new Category { Name = "Fat Burners" }
                );
                await context.SaveChangesAsync();
            }

            // Seed Products
            if (!context.Products.Any())
            {
                var proteins = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Proteins");
                var amino = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Amino Acids");
                var vitamins = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Vitamins");
                var burners = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Fat Burners");

                if (proteins == null || amino == null || vitamins == null || burners == null)
                    throw new Exception("One or more categories were not found while seeding products.");

                context.Products.AddRange(
                    new Product { Name = "Whey Protein 2kg", Description = "High quality whey protein for muscle growth.", Price = 89.99M, ImageUrl = "https://via.placeholder.com/300", Category = proteins },
                    new Product { Name = "Isolate Protein", Description = "Fast absorbing isolate protein.", Price = 69.99M, ImageUrl = "https://via.placeholder.com/300", Category = proteins },
                    new Product { Name = "Casein Protein", Description = "Slow digesting casein protein.", Price = 59.99M, ImageUrl = "https://via.placeholder.com/300", Category = proteins },
                    new Product { Name = "Hydrolyzed Whey", Description = "Premium hydrolyzed whey protein.", Price = 74.99M, ImageUrl = "https://via.placeholder.com/300", Category = proteins },

                    new Product { Name = "BCAA 4:1:1", Description = "Essential branched-chain amino acids.", Price = 29.99M, ImageUrl = "https://via.placeholder.com/300", Category = amino },
                    new Product { Name = "Glutamine", Description = "Support muscle recovery and reduce fatigue.", Price = 24.99M, ImageUrl = "https://via.placeholder.com/300", Category = amino },
                    new Product { Name = "EAA", Description = "Essential amino acids for muscle growth.", Price = 34.99M, ImageUrl = "https://via.placeholder.com/300", Category = amino },
                    new Product { Name = "Citrulline Malate", Description = "Improve blood flow and performance.", Price = 19.99M, ImageUrl = "https://via.placeholder.com/300", Category = amino },

                    new Product { Name = "Multivitamin Complex", Description = "Complete daily multivitamin.", Price = 14.99M, ImageUrl = "https://via.placeholder.com/300", Category = vitamins },
                    new Product { Name = "Vitamin D3 5000IU", Description = "Supports immune and bone health.", Price = 9.99M, ImageUrl = "https://via.placeholder.com/300", Category = vitamins },
                    new Product { Name = "Vitamin C 1000mg", Description = "Antioxidant support and immune boost.", Price = 8.99M, ImageUrl = "https://via.placeholder.com/300", Category = vitamins },
                    new Product { Name = "Omega-3 Fish Oil", Description = "Supports heart and joint health.", Price = 16.99M, ImageUrl = "https://via.placeholder.com/300", Category = vitamins },
                    new Product { Name = "ZMA", Description = "Improves sleep and recovery.", Price = 18.99M, ImageUrl = "https://via.placeholder.com/300", Category = vitamins },
                    new Product { Name = "Magnesium Citrate", Description = "Supports muscle and nerve function.", Price = 12.99M, ImageUrl = "https://via.placeholder.com/300", Category = vitamins },

                    new Product { Name = "Thermogenic Burner", Description = "Boost metabolism and burn fat.", Price = 39.99M, ImageUrl = "https://via.placeholder.com/300", Category = burners },
                    new Product { Name = "L-Carnitine", Description = "Helps convert fat into energy.", Price = 19.99M, ImageUrl = "https://via.placeholder.com/300", Category = burners },
                    new Product { Name = "CLA", Description = "Supports fat loss and lean muscle.", Price = 22.99M, ImageUrl = "https://via.placeholder.com/300", Category = burners },
                    new Product { Name = "Green Tea Extract", Description = "Natural fat burner and antioxidant.", Price = 11.99M, ImageUrl = "https://via.placeholder.com/300", Category = burners }
                );

                await context.SaveChangesAsync();
            }
        }
    }
}
