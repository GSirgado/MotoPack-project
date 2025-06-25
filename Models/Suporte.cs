using System.ComponentModel.DataAnnotations;

namespace MotoPack_project.Models
{
    public class Suporte
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Mensagem { get; set; } = string.Empty;

        public DateTime DataHora { get; set; } = DateTime.Now;
    }
}
