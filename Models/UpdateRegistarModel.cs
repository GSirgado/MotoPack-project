using System.ComponentModel.DataAnnotations;

namespace MotoPack_project.Models
{
    public class UpdateRegistarModel
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O email é obrigatório.")]
        [EmailAddress(ErrorMessage = "Email inválido.")]
        public string Email { get; set; } = string.Empty;

        public bool IsAdmin { get; set; }

        public string? FotoPerfil { get; set; }

        // Password nova (opcional)
        public string? NovaPass { get; set; }

        [Compare("NovaPass", ErrorMessage = "As passwords não coincidem.")]
        public string? ConfNovaPass { get; set; }
    }
}