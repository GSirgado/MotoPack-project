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

        // -------------------- REGISTO --------------------
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
            // Verifica se as palavras-passe coincidem
            if (model.Pass != model.ConfPass)
            {
                ViewBag.Erro = "As palavras-passe não coincidem.";
                return View();
            }

            // Verifica se o email já existe
            if (_context.Registars.Any(u => u.Email == model.Email))
            {
                ViewBag.Erro = "Email já registado.";
                return View();
            }

            // Hash da palavra-passe antes de guardar
            var passwordHasher = new PasswordHasher<Registar>();
            model.Pass = passwordHasher.HashPassword(model, model.Pass);

            _context.Registars.Add(model);
            _context.SaveChanges();

            return RedirectToAction("Login");
        }

        // -------------------- LOGIN --------------------
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
                    // Define as claims para o utilizador autenticado
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Name, user.Nome),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim("IsAdmin", user.IsAdmin.ToString()),
                        new Claim(ClaimTypes.Role, user.IsAdmin ? "Admin" : "User")
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

        // -------------------- LOGOUT --------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        // -------------------- PERFIL DO UTILIZADOR --------------------
        [Authorize]
        public IActionResult Perfil()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return RedirectToAction("Login");

            var user = _context.Registars.Find(int.Parse(userId));
            if (user == null) return RedirectToAction("Login");

            // Produtos publicados pelo utilizador
            var produtos = _context.Produtos
                .Where(p => p.UtilizadorId == user.Id)
                .ToList();

            ViewBag.Produtos = produtos;
            return View(user);
        }

        // -------------------- EDITAR PERFIL --------------------
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

            // Verifica palavra-passe atual
            var hasher = new PasswordHasher<Registar>();
            var verifica = hasher.VerifyHashedPassword(user, user.Pass, model.PasswordAtual);
            if (verifica != PasswordVerificationResult.Success)
            {
                ModelState.AddModelError(string.Empty, "Palavra-passe atual incorreta.");
                return View(model);
            }

            // Atualiza palavra-passe se nova for indicada
            if (!string.IsNullOrEmpty(model.NovaPass))
            {
                if (model.NovaPass != model.ConfNovaPass)
                {
                    ModelState.AddModelError(string.Empty, "Nova palavra-passe e confirmação não coincidem.");
                    return View(model);
                }

                user.Pass = hasher.HashPassword(user, model.NovaPass);
            }

            user.Nome = model.Nome;
            user.Email = model.Email;

            // Atualiza foto de perfil
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

        // -------------------- TROCAR FOTO (separado) --------------------
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

        // -------------------- EDITAR PRODUTO --------------------
        [Authorize]
        [HttpGet]
        public IActionResult EditarProduto(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Garante que o produto pertence ao utilizador autenticado
            var produto = _context.Produtos.FirstOrDefault(p => p.Id == id && p.UtilizadorId == userId);
            if (produto == null)
                return NotFound();

            return View("EditarProdutoUtilizador", produto);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditarProduto(Produto model)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var produto = _context.Produtos.FirstOrDefault(p => p.Id == model.Id && p.UtilizadorId == userId);

            if (produto == null)
                return Unauthorized();

            if (!ModelState.IsValid)
                return View("EditarProdutoUtilizador", model);

            // Atualiza dados principais
            produto.Nome = model.Nome;
            produto.Preco = model.Preco;
            produto.Categoria = model.Categoria;
            produto.Descricao = model.Descricao;

            // Atualiza imagem se nova for fornecida
            if (model.Imagem != null && model.Imagem.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.Imagem.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.Imagem.CopyToAsync(stream);
                }

                // Remove imagem anterior se existir
                if (!string.IsNullOrEmpty(produto.ImageUrl))
                {
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", produto.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                produto.ImageUrl = "/uploads/" + uniqueFileName;
            }

            _context.SaveChanges();

            TempData["Sucesso"] = "Produto atualizado com sucesso.";
            return RedirectToAction("Perfil");
        }

        // -------------------- APAGAR PRODUTO --------------------
        [Authorize]
        [HttpPost]
        public IActionResult ApagarProduto(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var produto = _context.Produtos.FirstOrDefault(p => p.Id == id && p.UtilizadorId == userId);
            if (produto == null)
                return Unauthorized();

            // Apaga imagem associada se existir
            if (!string.IsNullOrEmpty(produto.ImageUrl))
            {
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", produto.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            _context.Produtos.Remove(produto);
            _context.SaveChanges();

            TempData["Sucesso"] = "Produto apagado com sucesso.";
            return RedirectToAction("Perfil");
        }

    }
}