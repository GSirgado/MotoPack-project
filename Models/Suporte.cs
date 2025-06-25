using System.ComponentModel.DataAnnotations;

namespace MotoPack_project.Models
{
    public class Suporte
    {
<<<<<<< HEAD
        [Key] // Garante que essa é a chave primária
=======
        [Key]
>>>>>>> de178ab14944c736a2c455ac24c31151131d2a97
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Mensagem { get; set; } = string.Empty;

        public DateTime DataHora { get; set; } = DateTime.Now;
    }
<<<<<<< HEAD
}
=======
}
>>>>>>> de178ab14944c736a2c455ac24c31151131d2a97
