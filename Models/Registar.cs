<<<<<<< HEAD
﻿using Microsoft.AspNetCore.Identity;
=======
﻿using System.ComponentModel.DataAnnotations;
>>>>>>> de178ab14944c736a2c455ac24c31151131d2a97
using System.ComponentModel.DataAnnotations.Schema;

namespace MotoPack_project.Models
{
    public class Registar
    {
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; } = string.Empty;
<<<<<<< HEAD
        public string Email { get; set; } = string.Empty;

        public string Pass { get; set; } = string.Empty; // Esta será a hash

        [NotMapped] // Não será guardada na base de dados
        public string ConfPass { get; set; } = string.Empty;

        public bool IsAdmin { get; set; }
=======

        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Pass { get; set; } = string.Empty;

        [NotMapped]
        public string ConfPass { get; set; } = string.Empty;

        public bool IsAdmin { get; set; } = false;
>>>>>>> de178ab14944c736a2c455ac24c31151131d2a97

        public List<Produto>? Produtos { get; set; }
    }
}
