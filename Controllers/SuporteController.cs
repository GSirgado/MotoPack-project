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

        // Injeta o contexto da base de dados no controller
        public SuporteController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Suporte()
        {
            var suporte = new Suporte(); // Cria um novo objeto Suporte

            // Verifica se o utilizador está autenticado
            if (User.Identity?.IsAuthenticated ?? false)
            {
                // Obtém o ID do utilizador autenticado
                var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Tenta converter o ID para int
                if (int.TryParse(userIdStr, out var userId))
                {
                    // Procura o utilizador na base de dados
                    var user = _context.Registars.Find(userId);
                    if (user != null)
                    {
                        // Preenche automaticamente os campos Nome e Email no formulário
                        suporte.Nome = user.Nome;
                        suporte.Email = user.Email;
                    }
                }
            }

            return View(suporte); // Retorna a view com o modelo preenchido (ou vazio)
        }

        [HttpPost]
        public IActionResult EnviarPedido(Suporte model)
        {
            // Se os dados do formulário não forem válidos, retorna à view com os erros
            if (!ModelState.IsValid)
                return View("Suporte", model);

            // Define a data e hora atual no pedido
            model.DataHora = DateTime.Now;

            // Adiciona o pedido de suporte à base de dados
            _context.Suportes.Add(model);
            _context.SaveChanges();

            // Armazena uma mensagem de sucesso temporariamente
            TempData["SuporteSucesso"] = "Pedido de ajuda enviado com sucesso!";

            // Redireciona novamente para o formulário (poderia ser uma página de confirmação também)
            return RedirectToAction("Suporte");
        }
    }
}
