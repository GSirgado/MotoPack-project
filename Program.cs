using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MotoPack_project.Data;
using MotoPack_project.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// --------------------------------------------
// Serviços
// --------------------------------------------

builder.Services.AddHttpContextAccessor();

// Caminho completo para a base de dados
var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "Database", "MotoPack.db");

// Serviços MVC, Razor Pages e API Controllers
builder.Services.AddControllersWithViews();  // MVC + Razor
builder.Services.AddRazorPages();
builder.Services.AddControllers();           // API JSON

// Configuração da base de dados SQLite
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

// Autenticação com cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Conta/Login";
        options.LogoutPath = "/Conta/Logout";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
    });

// Política de cookies
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => false;
    options.MinimumSameSitePolicy = SameSiteMode.Lax;
});

// Sessões
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --------------------------------------------
// Construir o app
// --------------------------------------------
var app = builder.Build();

// --------------------------------------------
// Middleware
// --------------------------------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCookiePolicy();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

// PROTEÇÃO DO SWAGGER UI SÓ PARA ADMIN
app.UseWhen(context => context.Request.Path.StartsWithSegments("/swagger"), appBuilder =>
{
    appBuilder.Use(async (ctx, next) =>
    {
        if (!ctx.User.Identity?.IsAuthenticated ?? true)
        {
            ctx.Response.StatusCode = 401; // Unauthorized
            return;
        }

        if (!ctx.User.IsInRole("Admin"))
        {
            ctx.Response.StatusCode = 403; // Forbidden
            return;
        }

        await next();
    });

    appBuilder.UseSwagger();
    appBuilder.UseSwaggerUI();
});

// ROTAS
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllers(); // Ativa endpoints API JSON

// --------------------------------------------
// Criar base de dados e admin por defeito
// --------------------------------------------
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();

    if (!db.Registars.Any(u => u.Email == "admin@gmail.com"))
    {
        var passwordHasher = new PasswordHasher<Registar>();
        var admin = new Registar
        {
            Nome = "Admin",
            Email = "admin@gmail.com",
            IsAdmin = true
        };

        admin.Pass = passwordHasher.HashPassword(admin, "admin1234");
        admin.ConfPass = admin.Pass;

        db.Registars.Add(admin);
        db.SaveChanges();
    }
}

// --------------------------------------------
// Iniciar o app
// --------------------------------------------
app.Run();