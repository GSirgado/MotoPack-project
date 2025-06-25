using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using MotoPack_project.Data;

var builder = WebApplication.CreateBuilder(args);

// Caminho completo para o ficheiro .db na pasta "Database"
var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "Database", "MotoPack.db");

// Adiciona suporte a controladores e Razor Pages
builder.Services.AddControllersWithViews();

// Configuração da base de dados SQLite
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlite($"Data Source={dbPath}");
});

// Autenticação com Cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Home/Login";
        options.LogoutPath = "/Home/Logout";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
    });

// Política de Cookies
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => false;
    options.MinimumSameSitePolicy = SameSiteMode.Lax;
});

var app = builder.Build();

// Tratamento de erros e HTTPS
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

// Rota padrão
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    db.Database.Migrate(); // Aplica automaticamente as migrações, se necessário

    if (!db.Registars.Any(u => u.Email == "admin@gmail.com"))
    {
        db.Registars.Add(new MotoPack_project.Models.Registar
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
