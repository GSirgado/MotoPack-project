using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MotoPack_project.Models
{
    public class Registar
    {
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Pass { get; set; } = string.Empty;

        [NotMapped]
        public string ConfPass { get; set; } = string.Empty;

        public bool IsAdmin { get; set; } = false;

        public List<Produto>? Produtos { get; set; }
    }
}
