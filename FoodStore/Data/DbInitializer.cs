public static class DbInitializer
{
    public static void Initialize(ApplicationDbContext context)
    {
        if (context.Categories.Any()) return; // Базата вече е попълнена

        var categories = new List<Category>
        {
            new Category { Name = "Протеини" },
            new Category { Name = "Витамини" }
        };

        var products = new List<Product>
        {
            new Product { Name = "Whey Protein", Price = 59.99m, CategoryId = 1 },
            new Product { Name = "Vitamin C", Price = 19.99m, CategoryId = 2 }
        };

        context.Categories.AddRange(categories);
        context.Products.AddRange(products);
        context.SaveChanges();
    }
}