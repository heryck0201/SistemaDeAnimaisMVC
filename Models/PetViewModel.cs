using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaDeAnimaisMVC.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SistemaDeAnimaisMVC.Models
{
    public class PetViewModel
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Informe o nome")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Informe a Rança")]
        public string Raca { get; set; }

        [Required(ErrorMessage = "Informe a Cor")]
        public string Cor { get; set; }

        [Required(ErrorMessage = "Informe o Porte")]
        public string Porte { get; set; }

        [Required(ErrorMessage = "Informe o Dono do Pet")]
        public int? UsuarioId { get; set; }
        public virtual UsuarioModel UsuarioModel { get; set; }
    }
}
