using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using MotoPack_project.Data;
using MotoPack_project.Database;

var builder = WebApplication.CreateBuilder(args);

// 1. Construir o caminho correto para o ficheiro .db
var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "Database", "MotoPack.db");

// 2. Configurar serviços
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Apenas esta chamada é necessária
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Home/Login";
        options.LogoutPath = "/Home/Logout";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
    });

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => false;
    options.MinimumSameSitePolicy = SameSiteMode.Lax;
});

// 3. Inicializar base de dados (aplica migrações)
IniciarBaseDados.Iniciar(builder.Services.BuildServiceProvider());

// 4. Construir aplicação
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
app.UseAuthentication();
app.UseAuthorization();

// 6. Rotas
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

// 7. Correr aplicação
app.Run();
