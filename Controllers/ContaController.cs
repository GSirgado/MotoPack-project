using Microsoft.AspNetCore.Mvc;
<<<<<<< HEAD
using MotoPack_project.Models;
using MotoPack_project.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
=======
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using MotoPack_project.Models;
using MotoPack_project.Data;
>>>>>>> de178ab14944c736a2c455ac24c31151131d2a97

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
<<<<<<< HEAD
        [AllowAnonymous]
        public IActionResult Registar()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
=======
        public IActionResult Registar() => View();

        [HttpPost]
>>>>>>> de178ab14944c736a2c455ac24c31151131d2a97
        public IActionResult Registar(Registar model)
        {
            if (model.Pass != model.ConfPass)
            {
                ViewBag.Erro = "As palavras-passe não coincidem.";
                return View();
            }

<<<<<<< HEAD
            var existe = _context.Registars.Any(u => u.Email == model.Email);
            if (existe)
=======
            if (_context.Registars.Any(u => u.Email == model.Email))
>>>>>>> de178ab14944c736a2c455ac24c31151131d2a97
            {
                ViewBag.Erro = "Email já registado.";
                return View();
            }

<<<<<<< HEAD
            // Hash da password
            var passwordHasher = new PasswordHasher<Registar>();
            model.Pass = passwordHasher.HashPassword(model, model.Pass);

=======
>>>>>>> de178ab14944c736a2c455ac24c31151131d2a97
            _context.Registars.Add(model);
            _context.SaveChanges();

            return RedirectToAction("Login");
        }

        [HttpGet]
<<<<<<< HEAD
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(Registar model)
        {
            var user = _context.Registars.FirstOrDefault(u => u.Email == model.Email);

            if (user != null)
            {
                var passwordHasher = new PasswordHasher<Registar>();
                var result = passwordHasher.VerifyHashedPassword(user, user.Pass, model.Pass);

                if (result == PasswordVerificationResult.Success)
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
            }

            ViewBag.Erro = "Email ou palavra-passe inválidos.";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
=======
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
>>>>>>> de178ab14944c736a2c455ac24c31151131d2a97
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

<<<<<<< HEAD
        [Authorize]
=======
        [HttpGet]
>>>>>>> de178ab14944c736a2c455ac24c31151131d2a97
        public IActionResult Perfil()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return RedirectToAction("Login");

            var user = _context.Registars.Find(int.Parse(userId));
            if (user == null) return RedirectToAction("Login");

<<<<<<< HEAD
            var produtos = _context.Produtos
                .Where(p => p.UtilizadorId == user.Id)
                .ToList();

=======
            var produtos = _context.Produtos.Where(p => p.UtilizadorId == user.Id).ToList();
>>>>>>> de178ab14944c736a2c455ac24c31151131d2a97
            ViewBag.Produtos = produtos;

            return View(user);
        }
    }
}
