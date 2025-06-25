<<<<<<< HEAD
﻿using System.ComponentModel.DataAnnotations.Schema;
=======
﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
>>>>>>> de178ab14944c736a2c455ac24c31151131d2a97

namespace MotoPack_project.Models
{
    public class Produto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public decimal Preco { get; set; }
        public string Categoria { get; set; } = string.Empty;
<<<<<<< HEAD
        public string Descricao { get; set; } = string.Empty; 

        public int? UtilizadorId { get; set; }
        public Registar? Utilizador { get; set; }

        public DateTime DataCriacao { get; set; } = DateTime.Now;


        [NotMapped]
        public IFormFile? Imagem { get; set; }
=======
        public string? Descricao { get; set; }
        public string? ImageUrl { get; set; } // link para a imagem principal

        public DateTime DataCriacao { get; set; }

        public int UtilizadorId { get; set; }
        public required Registar Utilizador { get; set; }

        // Para suporte a múltiplas imagens
        [NotMapped]
        public IFormFile? Imagem { get; set; } // apenas uma imagem a enviar

>>>>>>> de178ab14944c736a2c455ac24c31151131d2a97
    }

}
