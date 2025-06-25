using System.ComponentModel.DataAnnotations.Schema;

namespace MotoPack_project.Models
{
    public class Produto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public decimal Preco { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty; 

        public int? UtilizadorId { get; set; }
        public Registar? Utilizador { get; set; }

        public DateTime DataCriacao { get; set; } = DateTime.Now;


        [NotMapped]
        public IFormFile? Imagem { get; set; }
    }
}
