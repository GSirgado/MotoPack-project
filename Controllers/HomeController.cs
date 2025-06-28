using Microsoft.AspNetCore.Mvc;
using MotoPack_project.Models;
using MotoPack_project.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

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
            var produtosRecentes = _context.Produtos
                .OrderByDescending(p => p.DataCriacao)
                .Take(3)
                .ToList();

            return View(produtosRecentes);
        }

        public IActionResult Privacy()
        {
            return View();
        }


        [HttpGet]
        public IActionResult Suporte()
        {
            var suporte = new Suporte();

            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = _context.Registars.FirstOrDefault(u => u.Id == int.Parse(userId));

                if (user != null)
                {
                    suporte.Nome = user.Nome;
                    suporte.Email = user.Email;
                }
            }

            return View(suporte);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EnviarPedido(Suporte model)
        {
            if (!ModelState.IsValid)
            {
                return View("Suporte", model);
            }

            try
            {
                model.DataHora = DateTime.Now;
                _context.Suportes.Add(model);
                _context.SaveChanges();

                TempData["SuporteSucesso"] = "Pedido de ajuda enviado com sucesso!";
            }
            catch (Exception ex)
            {
                TempData["SuporteSucesso"] = "Ocorreu um erro ao enviar o pedido.";
                return View("Suporte", model);
            }

            return RedirectToAction("Suporte");
        }

        
    }
}
