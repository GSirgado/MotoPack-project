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

        // Novo nome da action: ConversasRecentes
        public IActionResult ConversasRecentes()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            // Buscar conversas onde o utilizador é remetente ou destinatário
            var conversas = _context.Chats
                .Include(c => c.Mensagens.OrderByDescending(m => m.DataEnvio).Take(1))
                .Include(c => c.Remetente)
                .Include(c => c.Destinatario)
                .Where(c => c.RemetenteId == userId || c.DestinatarioId == userId)
                .OrderByDescending(c => c.Mensagens.Max(m => m.DataEnvio))
                .ToList();

            return View(conversas); // View deve ser ConversasRecentes.cshtml
        }

        public IActionResult Conversa(int destinatarioId)
        {
            var remetenteId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (remetenteId == destinatarioId)
                return RedirectToAction("Index", "Home");

            var destinatario = _context.Registars.FirstOrDefault(u => u.Id == destinatarioId);
            if (destinatario == null)
                return NotFound();

            var mensagens = _context.Mensagens
                .Include(m => m.Chat)
                .Where(m =>
                    (m.RemetenteId == remetenteId && m.Chat.DestinatarioId == destinatarioId) ||
                    (m.RemetenteId == destinatarioId && m.Chat.RemetenteId == remetenteId))
                .OrderBy(m => m.DataEnvio)
                .ToList();

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
