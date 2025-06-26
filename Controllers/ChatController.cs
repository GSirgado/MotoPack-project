using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotoPack_project.Data;
using System.Security.Claims;

namespace MotoPack_project.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ChatController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult ConversasRecentes()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            // Incluir mensagens corretamente (sem o OrderBy dentro do Include)
            var conversas = _context.Chats
                .Include(c => c.Mensagens)
                .Include(c => c.Remetente)
                .Include(c => c.Destinatario)
                .Where(c => c.RemetenteId == userId || c.DestinatarioId == userId)
                .ToList();

            // Ordenar em memória com base na última mensagem (evita erro com LINQ to Entities)
            conversas = conversas
                .OrderByDescending(c => c.Mensagens.Any() ? c.Mensagens.Max(m => m.DataEnvio) : DateTime.MinValue)
                .ToList();

            return View("ConversasRecentes", conversas); // Garante que está a ir para a view correta
        }

        public IActionResult Conversa(int destinatarioId)
        {
            var remetenteId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (remetenteId == destinatarioId)
                return RedirectToAction("Index", "Home");

            var destinatario = _context.Registars.FirstOrDefault(u => u.Id == destinatarioId);
            if (destinatario == null)
                return NotFound();

            // Recuperar chat existente (qualquer direção)
            var chat = _context.Chats
                .Include(c => c.Mensagens)
                .FirstOrDefault(c =>
                    (c.RemetenteId == remetenteId && c.DestinatarioId == destinatarioId) ||
                    (c.RemetenteId == destinatarioId && c.DestinatarioId == remetenteId));

            var mensagens = chat?.Mensagens
                .OrderBy(m => m.DataEnvio)
                .ToList() ?? new List<Models.Mensagem>();

            ViewBag.DestinatarioId = destinatarioId;
            ViewBag.DestinatarioNome = destinatario.Nome;
            ViewBag.Mensagens = mensagens;

            return View();
        }

        [HttpPost]
        public IActionResult EnviarMensagem(int destinatarioId, string texto)
        {
            var remetenteId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var chat = _context.Chats.FirstOrDefault(c =>
                (c.RemetenteId == remetenteId && c.DestinatarioId == destinatarioId) ||
                (c.RemetenteId == destinatarioId && c.DestinatarioId == remetenteId));

            if (chat == null)
            {
                chat = new Models.Chat
                {
                    RemetenteId = remetenteId,
                    DestinatarioId = destinatarioId
                };
                _context.Chats.Add(chat);
                _context.SaveChanges();
            }

            var mensagem = new Models.Mensagem
            {
                ChatId = chat.Id,
                RemetenteId = remetenteId,
                Texto = texto,
                DataEnvio = DateTime.Now
            };

            _context.Mensagens.Add(mensagem);
            _context.SaveChanges();

            return RedirectToAction("Conversa", new { destinatarioId });
        }
    }
}
