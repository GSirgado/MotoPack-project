using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MotoPack_project.Data;
<<<<<<< HEAD
using MotoPack_project.Models;

var builder = WebApplication.CreateBuilder(args);

<<<<<<< HEAD
// Caminho completo para o ficheiro .db na pasta "Database"
var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "Database", "MotoPack.db");

// Adiciona suporte a controladores e Razor Pages
builder.Services.AddControllersWithViews();

// Configuração da base de dados SQLite
=======
// Adiciona controladores e views
builder.Services.AddControllersWithViews();

var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "Database", "MotoPack.db");
>>>>>>> de178ab14944c736a2c455ac24c31151131d2a97
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlite($"Data Source={dbPath}");
});
=======
using MotoPack_project.Database;

<<<<<<< HEAD
// Autenticação com Cookies
=======
var builder = WebApplication.CreateBuilder(args);

// 1. Construir o caminho correto para o ficheiro .db
var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "Database", "MotoPack.db");

// 2. Configurar serviços
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Apenas esta chamada é necessária
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));
>>>>>>> 42004b439946ff58f9fe1d645b35c653ec6c69ed

// Autenticação com cookies
>>>>>>> de178ab14944c736a2c455ac24c31151131d2a97
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Conta/Login";   // atualizado
        options.LogoutPath = "/Conta/Logout"; // atualizado
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
    });

<<<<<<< HEAD
// Política de Cookies
=======
// Política de cookies
>>>>>>> de178ab14944c736a2c455ac24c31151131d2a97
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => false;
    options.MinimumSameSitePolicy = SameSiteMode.Lax;
});

<<<<<<< HEAD
// Sessões
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

=======
// 3. Inicializar base de dados (aplica migrações)
IniciarBaseDados.Iniciar(builder.Services.BuildServiceProvider());

// 4. Construir aplicação
>>>>>>> 42004b439946ff58f9fe1d645b35c653ec6c69ed
var app = builder.Build();

<<<<<<< HEAD
// Tratamento de erros e HTTPS
=======
// 5. Middleware
>>>>>>> de178ab14944c736a2c455ac24c31151131d2a97
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
app.UseAuthentication();
app.UseAuthorization();

// Rota padrão
=======
<<<<<<< HEAD
app.UseSession();
=======
>>>>>>> 42004b439946ff58f9fe1d645b35c653ec6c69ed
app.UseAuthentication();
app.UseAuthorization();

// 6. Rotas
>>>>>>> de178ab14944c736a2c455ac24c31151131d2a97
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

<<<<<<< HEAD
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    db.Database.Migrate(); // Aplica automaticamente as migrações, se necessário

    if (!db.Registars.Any(u => u.Email == "admin@gmail.com"))
    {
        db.Registars.Add(new MotoPack_project.Models.Registar
=======
<<<<<<< HEAD
// Criar base de dados e admin inicial
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.EnsureCreated();

    if (!db.Registars.Any(u => u.Email == "admin@gmail.com"))
    {
        db.Registars.Add(new Registar
>>>>>>> de178ab14944c736a2c455ac24c31151131d2a97
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

<<<<<<< HEAD
=======
=======
app.MapRazorPages();

// 7. Correr aplicação
>>>>>>> 42004b439946ff58f9fe1d645b35c653ec6c69ed
>>>>>>> de178ab14944c736a2c455ac24c31151131d2a97
app.Run();
