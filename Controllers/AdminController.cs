using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MotoPack_project.Models;
using MotoPack_project.Data;
using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;

namespace MotoPack_project.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult GerirUtilizadores()
        {
            var utilizadores = _context.Registars.Include(u => u.Produtos).ToList();
            return View(utilizadores);
        }

        [HttpGet]
        public IActionResult EditarUtilizador(int id)
        {
            var utilizador = _context.Registars.Find(id);
            if (utilizador == null)
                return NotFound();

            return View(utilizador);
        }

        [HttpPost]
        public IActionResult EditarUtilizador(Registar model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var utilizador = _context.Registars.Find(model.Id);
            if (utilizador == null)
                return NotFound();

            utilizador.Nome = model.Nome;
            utilizador.Email = model.Email;
            // Atualize mais campos se necessário

            _context.SaveChanges();
            TempData["Sucesso"] = "Utilizador atualizado.";
            return RedirectToAction("GerirUtilizadores");
        }

        [HttpPost]
        public IActionResult ApagarProduto(int id)
        {
            var produto = _context.Produtos.Find(id);
            if (produto != null)
            {
                _context.Produtos.Remove(produto);
                _context.SaveChanges();
            }

            return RedirectToAction("GerirUtilizadores");
        }

        public IActionResult ApagarUtilizador(int id)
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var atualId))
                return Unauthorized();

            if (atualId == id)
            {
                TempData["Erro"] = "Não podes apagar a tua própria conta de administrador.";
                return RedirectToAction("GerirUtilizadores");
            }

            var utilizador = _context.Registars.Include(u => u.Produtos).FirstOrDefault(u => u.Id == id);
            if (utilizador == null)
                return NotFound();

            _context.Produtos.RemoveRange(utilizador.Produtos ?? new List<Produto>());
            _context.Registars.Remove(utilizador);
            _context.SaveChanges();

            TempData["Sucesso"] = "Utilizador apagado com sucesso.";
            return RedirectToAction("GerirUtilizadores");
        }
    }
}
