using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MotoPack_project.Models
{
    public class Produto
    {
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; } = string.Empty;

        [Required]
        public decimal Preco { get; set; }

        [Required]
        public string Categoria { get; set; } = string.Empty;

        public string? Descricao { get; set; }

        public string? ImageUrl { get; set; }

        public DateTime DataCriacao { get; set; } = DateTime.Now;

        public int UtilizadorId { get; set; }

        public Registar? Utilizador { get; set; }

        [NotMapped]
        public IFormFile? Imagem { get; set; }
    }
}
