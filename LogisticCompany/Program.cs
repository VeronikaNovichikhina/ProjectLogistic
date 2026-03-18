using LogisticCompany.Application.Interfaces;
using LogisticCompany.Application.Services;
using LogisticCompany.Components;
using LogisticCompany.Db;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<ICompanyClientService, CompanyClientService>();
builder.Services.AddScoped<IIndividualClientService, IndividualClientService>();
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<IDeliveryDictionaryService, DeliveryDictionaryService>();
builder.Services.AddScoped<IPriceCalculatorService, PriceCalculationService>();
builder.Services.AddScoped<IMapService, OpenStreetMapService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IOrderQueryService, OrderQueryService>();
builder.Services.AddScoped<IRoleHelper, RoleHelperService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<PaymentService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITrackingService, TrackingService>();



builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.ExpireTimeSpan = TimeSpan.FromHours(24);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();


builder.Services.AddScoped<OpenStreetMapService>();
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

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
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