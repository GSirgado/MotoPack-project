using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace MotoPack_project.Models
{
    public class Registar
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public string Pass { get; set; } = string.Empty; // Esta será a hash

        [NotMapped] // Não será guardada na base de dados
        public string ConfPass { get; set; } = string.Empty;

        public bool IsAdmin { get; set; }

        public List<Produto>? Produtos { get; set; }
    }
}
