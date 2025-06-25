using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MotoPack_project.Models;
using MotoPack_project.Data;
using System.Security.Claims;

namespace MotoPack_project.Controllers
{
    public class SuporteController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SuporteController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Suporte()
        {
            var suporte = new Suporte();

            if (User.Identity?.IsAuthenticated ?? false)
            {
                var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (int.TryParse(userIdStr, out var userId))
                {
                    var user = _context.Registars.Find(userId);
                    if (user != null)
                    {
                        suporte.Nome = user.Nome;
                        suporte.Email = user.Email;
                    }
                }
            }

            return View(suporte);
        }

        [HttpPost]
        public IActionResult EnviarPedido(Suporte model)
        {
            if (!ModelState.IsValid)
                return View("Suporte", model);

            model.DataHora = DateTime.Now;
            _context.Suportes.Add(model);
            _context.SaveChanges();

            TempData["SuporteSucesso"] = "Pedido de ajuda enviado com sucesso!";
            return RedirectToAction("Suporte");
        }
    }
}
