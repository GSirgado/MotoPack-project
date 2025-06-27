namespace MotoPack_project.Models
{
    public class EditarPerfil
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public string? NovaPass { get; set; }
        public string? ConfNovaPass { get; set; }
        public string PasswordAtual { get; set; }
    }
}
