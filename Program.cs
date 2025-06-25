using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MotoPack_project.Data;
using MotoPack_project.Models;

var builder = WebApplication.CreateBuilder(args);

// Adiciona controladores e views
builder.Services.AddControllersWithViews();

var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "Database", "MotoPack.db");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlite($"Data Source={dbPath}");
});

// Autenticação com cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Conta/Login";   // atualizado
        options.LogoutPath = "/Conta/Logout"; // atualizado
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

var app = builder.Build();

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Criar base de dados e admin inicial
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.EnsureCreated();

    if (!db.Registars.Any(u => u.Email == "admin@gmail.com"))
    {
        db.Registars.Add(new Registar
        {
            Nome = "Admin",
            Email = "admin@gmail.com",
            Pass = "admin1234",
            ConfPass = "admin1234",
            IsAdmin = true
        });

        db.SaveChanges();
    }
}

app.Run();
