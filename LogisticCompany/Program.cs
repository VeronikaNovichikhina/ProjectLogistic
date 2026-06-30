using LogisticCompany.Application.Interfaces;
using LogisticCompany.Application.Services;
using LogisticCompany.Components;
using LogisticCompany.Db;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("login", limiter =>
    {
        limiter.PermitLimit = 5;                          
        limiter.Window = TimeSpan.FromMinutes(15);        
        limiter.QueueLimit = 0;                           
    });
});

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(
            builder.Configuration.GetConnectionString("DefaultConnection"))
    ));

builder.Services.AddQuickGridEntityFrameworkAdapter();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddHttpClient();

// Application services
builder.Services.AddScoped<IAuthService, AuthService>();
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
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITrackingService, TrackingService>();
builder.Services.AddScoped<IPasswordService, PasswordService>();


// OpenStreetMap — registered once via typed HttpClient
builder.Services.AddHttpClient<OpenStreetMapService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.LogoutPath = "/logout";

        options.Cookie.HttpOnly = true;

        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

        options.Cookie.SameSite = SameSiteMode.Strict;

        options.Cookie.Name = "lc_auth";

        options.ExpireTimeSpan = TimeSpan.FromHours(8);

        options.SlidingExpiration = true;

        options.AccessDeniedPath = "/access-denied";
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact", policy =>
        policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
});

builder.Services.AddControllers();
builder.Services.AddAuthorization();

builder.Services.AddHsts(options =>
{
    options.MaxAge = TimeSpan.FromDays(365);
    options.IncludeSubDomains = true;
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        await SeedAdmin(db);
        Console.WriteLine("Система инициализирована успешно");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ошибка инициализации: {ex.Message}");
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
else
{
    app.UseMigrationsEndPoint();
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseHttpsRedirection();

app.Use(async (context, next) =>
{
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["X-Frame-Options"] = "DENY";
    context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
    context.Response.Headers["Permissions-Policy"] = "geolocation=(), camera=(), microphone=()";
    await next();
});
app.MapGet("/logout", async (HttpContext context) =>
{
    await context.SignOutAsync(
        CookieAuthenticationDefaults.AuthenticationScheme);

    context.Response.Redirect("/login");
});
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
app.UseCors("AllowReact");
app.MapControllers();
app.Run();

static async Task SeedAdmin(AppDbContext db)
{
    if (!await db.Users.AnyAsync(u => u.Role == "MainAdmin"))
    {
        var adminPassword = Environment.GetEnvironmentVariable("ADMIN_INITIAL_PASSWORD")
                            ?? "Admin123!";

        var admin = new User
        {
            Email = "admin@logistic.ru",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(adminPassword),
            Role = "MainAdmin",
            IsTemporaryPassword = true   //временно
        };

        db.Users.Add(admin);
        await db.SaveChangesAsync();

        Console.WriteLine("Создан системный администратор: admin@logistic.ru");
    }
    else
    {
        Console.WriteLine("Администратор уже существует в системе");
    }
}