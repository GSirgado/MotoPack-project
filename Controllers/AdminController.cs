using Microsoft.AspNetCore.Mvc;
using MotoPack_project.Data;
using MotoPack_project.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
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
            if (utilizador == null) return NotFound();

            return View(utilizador);
        }

        [HttpPost]
        public IActionResult EditarUtilizador(Registar model)
        {
            if (!ModelState.IsValid) return View(model);

            _context.Registars.Update(model);
            _context.SaveChanges();

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
    }
}