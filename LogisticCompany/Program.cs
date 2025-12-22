using LogisticCompany.Components;
using LogisticCompany.Components.LoginRegister;
using LogisticCompany.Db;
using LogisticCompany.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    ));

builder.Services.AddQuickGridEntityFrameworkAdapter();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddHttpClient();


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.ExpireTimeSpan = TimeSpan.FromHours(24);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();


builder.Services.AddScoped<OpenStreetMapService>();

// HTTP Client для карт
builder.Services.AddHttpClient<OpenStreetMapService>();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    try
    {
        await SeedAdmin(db);

        Console.WriteLine(" Система инициализирована успешно");
    }
    catch (Exception ex)
    {
        Console.WriteLine($" Ошибка инициализации: {ex.Message}");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseMigrationsEndPoint();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();




app.Run();
static async Task SeedAdmin(AppDbContext db)
{
    if (!await db.Users.AnyAsync(u => u.Role == "MainAdmin"))
    {
        var admin = new User
        {
            Email = "admin@logistic.ru",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
            Role = "MainAdmin",
            IsTemporaryPassword = false
        };

        db.Users.Add(admin);
        await db.SaveChangesAsync();

        Console.WriteLine(" Создан системный администратор: admin@logistic.ru / Admin123!");
    }
    else
    {
        Console.WriteLine(" Администратор уже существует в системе");
    }
}