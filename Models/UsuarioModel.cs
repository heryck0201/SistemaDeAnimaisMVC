using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SistemaDeAnimaisMVC.Models
{
    public class UsuarioModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage ="Digite o nome do contato")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Digite o nome Sobrenome")]
        public string Sobrenome { get; set; }

        [Required(ErrorMessage = "Digite o Email")]
        [EmailAddress(ErrorMessage ="O e-mail informado não é válido")]
        public string Email { get; set; }
        public virtual IEnumerable<PetModel>? PetModel { get; set; }
    }
}