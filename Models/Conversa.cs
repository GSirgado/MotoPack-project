using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MotoPack_project.Models
{
    public class Mensagem
    {
        public int Id { get; set; }

        [Required]
        public string Texto { get; set; }

        public DateTime DataEnvio { get; set; } = DateTime.Now;

        public int ChatId { get; set; }
        public Chat Chat { get; set; }

        public int RemetenteId { get; set; }
        public Registar Remetente { get; set; }

        public bool Lida { get; set; } = false; // <- novo campo
    }

}
