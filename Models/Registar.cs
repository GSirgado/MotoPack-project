using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MotoPack_project.Models
{
    public class Registar
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O email é obrigatório.")]
        [EmailAddress(ErrorMessage = "Email inválido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A palavra-passe é obrigatória.")]
        public string Pass { get; set; } = string.Empty; // Guardará o hash da password

        [NotMapped]
        [Compare("Pass", ErrorMessage = "As palavras-passe não coincidem.")]
        public string ConfPass { get; set; } = string.Empty;


        [NotMapped]
        public string? NovaPass { get; set; }

        [NotMapped]
        [Compare("NovaPass", ErrorMessage = "As passwords não coincidem.")]
        public string? ConfNovaPass { get; set; }


        public bool IsAdmin { get; set; } = false;

        public string? FotoPerfil { get; set; } // Caminho da imagem

        public List<Produto>? Produtos { get; set; }
    }
}
