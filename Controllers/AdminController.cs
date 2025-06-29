using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MotoPack_project.Models;
using MotoPack_project.Data;
using System.Security.Claims;
using MotoPack_project.ViewModels;

namespace MotoPack_project.Controllers
{
    [Authorize(Roles = "Admin")] // Garante que só administradores podem aceder
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // -------------------- LISTAR UTILIZADORES --------------------
        public IActionResult GerirUtilizadores()
        {
            // Carrega utilizadores com seus produtos
            var utilizadores = _context.Registars
                .Include(u => u.Produtos)
                .ToList();
            return View(utilizadores);
        }

        // -------------------- EDITAR UTILIZADOR --------------------
        [HttpGet]
        public IActionResult EditarUtilizador(int id)
        {
            // Busca utilizador pelo ID
            var utilizador = _context.Registars.Find(id);
            if (utilizador == null)
                return NotFound();

            // Preenche ViewModel para edição
            var model = new EditarUtilizadorViewModel
            {
                Id = utilizador.Id,
                Nome = utilizador.Nome,
                Email = utilizador.Email
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult EditarUtilizador(EditarUtilizadorViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var utilizador = _context.Registars.Find(model.Id);
            if (utilizador == null)
                return NotFound();

            // Atualiza dados
            utilizador.Nome = model.Nome;
            utilizador.Email = model.Email;

            _context.SaveChanges();
            TempData["Sucesso"] = "Utilizador atualizado com sucesso.";
            return RedirectToAction("GerirUtilizadores");
        }

        // -------------------- EDITAR PRODUTO --------------------
        [HttpGet]
        public IActionResult EditarProduto(int id)
        {
            var produto = _context.Produtos.FirstOrDefault(p => p.Id == id);
            if (produto == null)
                return NotFound();

            return View(produto);
        }

        [HttpPost]
        public IActionResult EditarProduto(Produto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var produto = _context.Produtos.Find(model.Id);
            if (produto == null)
                return NotFound();

            // Atualiza dados principais
            produto.Nome = model.Nome;
            produto.Preco = model.Preco;
            produto.Categoria = model.Categoria;
            produto.Descricao = model.Descricao;

            // Se nova imagem for enviada
            if (model.Imagem != null && model.Imagem.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.Imagem.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Guarda a nova imagem
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    model.Imagem.CopyTo(stream);
                }

                // Apaga imagem antiga (se existir)
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
            return RedirectToAction("GerirUtilizadores");
        }

        // -------------------- APAGAR PRODUTO --------------------
        [HttpPost]
        public IActionResult ApagarProduto(int id)
        {
            var produto = _context.Produtos.Find(id);
            if (produto == null)
                return NotFound();

            _context.Produtos.Remove(produto);
            _context.SaveChanges();

            TempData["Sucesso"] = "Produto apagado com sucesso.";
            return RedirectToAction("GerirUtilizadores");
        }

        // -------------------- APAGAR UTILIZADOR --------------------
        public IActionResult ApagarUtilizador(int id)
        {
            // Impede apagar a própria conta admin
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var atualId))
                return Unauthorized();

            if (atualId == id)
            {
                TempData["Erro"] = "Não podes apagar a tua própria conta de administrador.";
                return RedirectToAction("GerirUtilizadores");
            }

            // Busca utilizador com os produtos
            var utilizador = _context.Registars
                .Include(u => u.Produtos)
                .FirstOrDefault(u => u.Id == id);

            if (utilizador == null)
                return NotFound();

            // Remove produtos e depois o utilizador
            _context.Produtos.RemoveRange(utilizador.Produtos ?? new List<Produto>());
            _context.Registars.Remove(utilizador);
            _context.SaveChanges();

            TempData["Sucesso"] = "Utilizador apagado com sucesso.";
            return RedirectToAction("GerirUtilizadores");
        }
    }
}
