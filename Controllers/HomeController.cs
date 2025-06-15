using Microsoft.AspNetCore.Mvc;
using MotoPack_project.Models;
using MotoPack_project.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace MotoPack_project.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Registar()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Registar(Registar model)
        {
            if (model.Pass != model.ConfPass)
            {
                ViewBag.Erro = "As palavras-passe não coincidem.";
                return View();
            }

            var existe = _context.Registars.Any(u => u.Email == model.Email);
            if (existe)
            {
                ViewBag.Erro = "Email já registado.";
                return View();
            }

            _context.Registars.Add(model);
            _context.SaveChanges();

            return RedirectToAction("Login");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(Registar model)
        {
            var user = _context.Registars
                .FirstOrDefault(u => u.Email == model.Email && u.Pass == model.Pass);

            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Nome),
                    new Claim(ClaimTypes.Email, user.Email)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    authProperties
                );

                return RedirectToAction("Perfil");
            }

            ViewBag.Erro = "Email ou palavra-passe inválidos.";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [Authorize]
        public IActionResult Perfil()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return RedirectToAction("Login");

            var user = _context.Registars.Find(int.Parse(userId));
            if (user == null) return RedirectToAction("Login");

            return View(user);
        }

        public IActionResult Catalogo()
        {
            var produtos = _context.Produtos.ToList();
            return View(produtos);
        }

        public IActionResult Categorias()
        {
            return View();
        }

        public IActionResult Suporte()
        {
            return View();
        }

    }
}
