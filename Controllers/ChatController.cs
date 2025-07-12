using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotoPack_project.Data;
using MotoPack_project.Models;
using System.Security.Claims;

namespace MotoPack_project.Controllers
{
    [Authorize] // Garante que apenas utilizadores autenticados acedem
    public class ChatController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ChatController(ApplicationDbContext context)
        {
            _context = context;
        }

        // -------------------- LISTA DE CONVERSAS RECENTES --------------------
        public IActionResult ConversasRecentes()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            // Recupera todas as conversas onde o utilizador é remetente ou destinatário
            var conversas = _context.Chats
                .Include(c => c.Mensagens)
                .Include(c => c.Remetente)
                .Include(c => c.Destinatario)
                .Where(c => c.RemetenteId == userId || c.DestinatarioId == userId)
                .ToList();

            // Ordena conversas por data da última mensagem (em memória)
            conversas = conversas
                .OrderByDescending(c => c.Mensagens.Any() ? c.Mensagens.Max(m => m.DataEnvio) : DateTime.MinValue)
                .ToList();

            return View("ConversasRecentes", conversas); // Garante que chama a view correta
        }

        // -------------------- VER CONVERSA ENTRE UTILIZADORES --------------------
        public IActionResult Conversa(int destinatarioId)
        {
            var remetenteId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (remetenteId == destinatarioId)
                return RedirectToAction("Index", "Home");

            var destinatario = _context.Registars.FirstOrDefault(u => u.Id == destinatarioId);
            if (destinatario == null)
                return NotFound();

            var chat = _context.Chats
                .Include(c => c.Mensagens)
                .FirstOrDefault(c =>
                    (c.RemetenteId == remetenteId && c.DestinatarioId == destinatarioId) ||
                    (c.RemetenteId == destinatarioId && c.DestinatarioId == remetenteId));

            var mensagens = chat?.Mensagens
                .OrderBy(m => m.DataEnvio)
                .ToList() ?? new List<Models.Mensagem>();

            // ✅ MARCAR COMO LIDAS (só mensagens recebidas pelo utilizador atual)
            var mensagensPorLer = mensagens
                .Where(m => m.RemetenteId == destinatarioId && !m.Lida)
                .ToList();

            foreach (var msg in mensagensPorLer)
            {
                msg.Lida = true;
            }

            if (mensagensPorLer.Count > 0)
            {
                _context.SaveChanges();
            }

            // Passar info para a View
            ViewBag.DestinatarioId = destinatarioId;
            ViewBag.DestinatarioNome = destinatario.Nome;
            ViewBag.Mensagens = mensagens;

            return View();
        }


        // -------------------- ENVIAR MENSAGEM --------------------
        [HttpPost]
        public IActionResult EnviarMensagem(int destinatarioId, string texto)
        {
            var remetenteId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            // Verifica se já existe um chat entre os utilizadores
            var chat = _context.Chats.FirstOrDefault(c =>
                (c.RemetenteId == remetenteId && c.DestinatarioId == destinatarioId) ||
                (c.RemetenteId == destinatarioId && c.DestinatarioId == remetenteId));

            // Cria novo chat se ainda não existir
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

            // Cria e guarda a nova mensagem
            var mensagem = new Models.Mensagem
            {
                ChatId = chat.Id,
                RemetenteId = remetenteId,
                Texto = texto,
                DataEnvio = DateTime.Now
            };

            _context.Mensagens.Add(mensagem);
            _context.SaveChanges();

            // Redireciona para a conversa atualizada
            return RedirectToAction("Conversa", new { destinatarioId });
        }
        [HttpGet]
        [HttpGet]
        public JsonResult ContarNaoLidas()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            // Buscar todas as mensagens não lidas recebidas pelo utilizador
            var mensagensPorLer = _context.Mensagens
                .Include(m => m.Chat)
                .Where(m =>
                    m.RemetenteId != userId &&
                    ((m.Chat.RemetenteId == userId) || (m.Chat.DestinatarioId == userId)) &&
                    !m.Lida)
                .ToList();

            int naoLidas = mensagensPorLer.Count;

            return Json(new { naoLidas });
        }


    }
}
