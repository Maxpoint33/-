using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Добавяне на услуги в контейнера (Dependency Injection)
builder.Services.AddControllersWithViews();

// 1. Регистриране на DbContext с SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// 2. Конфигурация на HTTP заявките
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// 3. Инициализация на базата данни (Seed данни)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<ApplicationDbContext>();
        DbInitializer.Initialize(dbContext); // Метод за попълване на данни
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Грешка при инициализация на базата данни!");
    }
}

// 4. Маршрутизация
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();