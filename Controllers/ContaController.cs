using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using MotoPack_project.Models;
using MotoPack_project.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;

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

            if (_context.Registars.Any(u => u.Email == model.Email))
            {
                ViewBag.Erro = "Email já registado.";
                return View();
            }

            var passwordHasher = new PasswordHasher<Registar>();
            model.Pass = passwordHasher.HashPassword(model, model.Pass);

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
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim("IsAdmin", user.IsAdmin.ToString())
                    };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);

                    return RedirectToAction("Perfil");
                }
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

            var produtos = _context.Produtos
                .Where(p => p.UtilizadorId == user.Id)
                .ToList();

            ViewBag.Produtos = produtos;

            return View(user);
        }

        [Authorize]
        [HttpGet]
        public IActionResult EditarPerfil()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return RedirectToAction("Login");

            var user = _context.Registars.Find(int.Parse(userId));
            if (user == null) return RedirectToAction("Login");

            var model = new EditarPerfil
            {
                Nome = user.Nome,
                Email = user.Email
            };

            return View(model);
        }


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditarPerfil(EditarPerfil model, IFormFile? NovaFoto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return RedirectToAction("Login");

            var user = _context.Registars.Find(int.Parse(userId));
            if (user == null) return RedirectToAction("Login");

            // Verifica password atual
            var hasher = new PasswordHasher<Registar>();
            var verifica = hasher.VerifyHashedPassword(user, user.Pass, model.PasswordAtual);
            if (verifica != PasswordVerificationResult.Success)
            {
                ModelState.AddModelError(string.Empty, "Palavra-passe atual incorreta.");
                return View(model);
            }

            // Verifica nova password
            if (!string.IsNullOrEmpty(model.NovaPass))
            {
                if (model.NovaPass != model.ConfNovaPass)
                {
                    ModelState.AddModelError(string.Empty, "Nova palavra-passe e confirmação não coincidem.");
                    return View(model);
                }

                user.Pass = hasher.HashPassword(user, model.NovaPass);
            }

            // Atualiza nome/email
            user.Nome = model.Nome;
            user.Email = model.Email;

            // Atualiza foto, se houver
            if (NovaFoto != null && NovaFoto.Length > 0)
            {
                var fileName = $"{user.Id}_{Path.GetFileName(NovaFoto.FileName)}";
                var path = Path.Combine("wwwroot/imagens/perfis", fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await NovaFoto.CopyToAsync(stream);
                }

                user.FotoPerfil = $"/imagens/perfis/{fileName}";
            }

            _context.Update(user);
            _context.SaveChanges();

            return RedirectToAction("Perfil");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> TrocarFoto(IFormFile NovaFoto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null || NovaFoto == null) return RedirectToAction("Perfil");

            var user = _context.Registars.Find(int.Parse(userId));
            if (user == null) return RedirectToAction("Perfil");

            var fileName = $"{user.Id}_{Path.GetFileName(NovaFoto.FileName)}";
            var path = Path.Combine("wwwroot/imagens/perfis", fileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await NovaFoto.CopyToAsync(stream);
            }

            user.FotoPerfil = $"/imagens/perfis/{fileName}";
            _context.Update(user);
            _context.SaveChanges();

            return RedirectToAction("Perfil");
        }

    }
}
