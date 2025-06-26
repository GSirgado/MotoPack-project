using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MotoPack_project.Models
{
    public class Chat
    {
        public int Id { get; set; }

        [ForeignKey("Remetente")]
        public int RemetenteId { get; set; }
        public Registar Remetente { get; set; }

        [ForeignKey("Destinatario")]
        public int DestinatarioId { get; set; }
        public Registar Destinatario { get; set; }

        public List<Mensagem> Mensagens { get; set; } = new List<Mensagem>();
    }
}
