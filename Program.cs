using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MotoPack_project.Data;
<<<<<<< HEAD
using MotoPack_project.Models;

var builder = WebApplication.CreateBuilder(args);

// Adiciona controladores e views
builder.Services.AddControllersWithViews();

var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "Database", "MotoPack.db");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlite($"Data Source={dbPath}");
});
=======
using MotoPack_project.Database;

var builder = WebApplication.CreateBuilder(args);

// 1. Construir o caminho correto para o ficheiro .db
var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "Database", "MotoPack.db");

// 2. Configurar servi�os
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Apenas esta chamada � necess�ria
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));
>>>>>>> 42004b439946ff58f9fe1d645b35c653ec6c69ed

// Autentica��o com cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Conta/Login";   // atualizado
        options.LogoutPath = "/Conta/Logout"; // atualizado
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
    });

// Pol�tica de cookies
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => false;
    options.MinimumSameSitePolicy = SameSiteMode.Lax;
});

<<<<<<< HEAD
// Sess�es
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

=======
// 3. Inicializar base de dados (aplica migra��es)
IniciarBaseDados.Iniciar(builder.Services.BuildServiceProvider());

// 4. Construir aplica��o
>>>>>>> 42004b439946ff58f9fe1d645b35c653ec6c69ed
var app = builder.Build();

// 5. Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseCookiePolicy();
<<<<<<< HEAD
app.UseSession();
=======
>>>>>>> 42004b439946ff58f9fe1d645b35c653ec6c69ed
app.UseAuthentication();
app.UseAuthorization();

// 6. Rotas
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

<<<<<<< HEAD
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

=======
app.MapRazorPages();

// 7. Correr aplica��o
>>>>>>> 42004b439946ff58f9fe1d645b35c653ec6c69ed
app.Run();
