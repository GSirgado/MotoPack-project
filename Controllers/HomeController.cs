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

        // Injeta o contexto da base de dados
        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Página inicial - mostra os 3 produtos mais recentes
        public IActionResult Index()
        {
            var produtosRecentes = _context.Produtos
                .OrderByDescending(p => p.DataCriacao)
                .Take(3)
                .ToList();

            return View(produtosRecentes);
        }

        // Página de política de privacidade
        public IActionResult Privacy()
        {
            return View();
        }

        // Formulário de suporte (GET)
        [HttpGet]
        public IActionResult Suporte()
        {
            var suporte = new Suporte();

            // Se o utilizador estiver autenticado, pré-preenche o nome e email
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

        // Envia o pedido de suporte (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EnviarPedido(Suporte model)
        {
            // Verifica se o modelo enviado é válido
            if (!ModelState.IsValid)
            {
                return View("Suporte", model);
            }

            try
            {
                // Define a data/hora atual e guarda o pedido na base de dados
                model.DataHora = DateTime.Now;
                _context.Suportes.Add(model);
                _context.SaveChanges();

                TempData["SuporteSucesso"] = "Pedido de ajuda enviado com sucesso!";
            }
            catch (Exception ex)
            {
                // Em caso de erro, mostra mensagem ao utilizador
                TempData["SuporteSucesso"] = "Ocorreu um erro ao enviar o pedido.";
                return View("Suporte", model);
            }

            // Redireciona de volta à página de suporte após envio
            return RedirectToAction("Suporte");
        }
    }
}
