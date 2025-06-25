using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MotoPack_project.Models;
using MotoPack_project.Data;
using System.Security.Claims;

namespace MotoPack_project.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult GerirUtilizadores()
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var adminId))
                return Unauthorized();

            var admin = _context.Registars.Find(adminId);
            if (admin == null || !admin.IsAdmin) return Unauthorized();

            var utilizadores = _context.Registars.Include(u => u.Produtos).ToList();
            return View(utilizadores);
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
            if (utilizador == null) return NotFound();

            _context.Produtos.RemoveRange(utilizador.Produtos ?? new List<Produto>());
            _context.Registars.Remove(utilizador);
            _context.SaveChanges();

            TempData["Sucesso"] = "Utilizador apagado com sucesso.";
            return RedirectToAction("GerirUtilizadores");
        }

        [HttpGet]
        public IActionResult EditarUtilizador(int id)
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var adminId))
                return Unauthorized();

            var admin = _context.Registars.Find(adminId);
            if (admin == null || !admin.IsAdmin) return Unauthorized();

            var utilizador = _context.Registars.Find(id);
            return utilizador == null ? NotFound() : View(utilizador);
        }

        [HttpPost]
        public IActionResult EditarUtilizador(Registar model)
        {
            var utilizador = _context.Registars.Find(model.Id);
            if (utilizador == null) return NotFound();

            utilizador.Nome = model.Nome;
            utilizador.Email = model.Email;

            _context.SaveChanges();
            TempData["Sucesso"] = "Utilizador atualizado.";
            return RedirectToAction("GerirUtilizadores");
        }
    }
}
