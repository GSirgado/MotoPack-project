using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using MotoPack_project.Models;
using MotoPack_project.Data;

namespace MotoPack_project.Controllers
{
    public class ContaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContaController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Registar() => View();

        [HttpPost]
        public IActionResult Registar(Registar model)
        {
            if (model.Pass != model.ConfPass)
            {
                ViewBag.Erro = "As palavras-passe não coincidem.";
                return View();
            }

            if (_context.Registars.Any(u => u.Email == model.Email))
            {
                ViewBag.Erro = "Email já registado.";
                return View();
            }

            _context.Registars.Add(model);
            _context.SaveChanges();

            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(Registar model)
        {
            var user = _context.Registars.FirstOrDefault(u => u.Email == model.Email && u.Pass == model.Pass);
            if (user == null)
            {
                ViewBag.Erro = "Email ou palavra-passe inválidos.";
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Nome),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("IsAdmin", user.IsAdmin.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Perfil");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Perfil()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return RedirectToAction("Login");

            var user = _context.Registars.Find(int.Parse(userId));
            if (user == null) return RedirectToAction("Login");

            var produtos = _context.Produtos.Where(p => p.UtilizadorId == user.Id).ToList();
            ViewBag.Produtos = produtos;

            return View(user);
        }
    }
}
