using System.Collections.Generic;

namespace SistemaDeAnimaisMVC.Models
{
    public class UsuarioModel
    {
        public string Id { get; set; }
        public string Nome { get; set; }
        public string Sobrenome { get; set; }
        public string Email { get; set; }
        public int PetModelId { get; set; }
        public IEnumerable<PetModel> PetModel { get; set; } 
    }
}
