using System.ComponentModel.DataAnnotations;

namespace MotoPack_project.Models
{
    public class Suporte
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O email é obrigatório.")]
        [EmailAddress(ErrorMessage = "O email fornecido não é válido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A mensagem é obrigatória.")]
        [StringLength(1000, ErrorMessage = "A mensagem não pode exceder 1000 caracteres.")]
        public string Mensagem { get; set; } = string.Empty;

        public DateTime DataHora { get; set; } = DateTime.Now;
    }
}
