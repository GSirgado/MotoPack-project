using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MotoPack_project.Models
{
    public class Produto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public decimal Preco { get; set; }
        public string Categoria { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public string? ImageUrl { get; set; } // link para a imagem principal

        public DateTime DataCriacao { get; set; }

        public int UtilizadorId { get; set; }
        public required Registar Utilizador { get; set; }

        // Para suporte a múltiplas imagens
        [NotMapped]
        public IFormFile? Imagem { get; set; } // apenas uma imagem a enviar

    }

}
