using System.Collections.Generic;

namespace SistemaDeAnimaisMVC.Models
{
    public class ReturnUsuarioModel
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Sobrenome { get; set; }
        public string? Email { get; set; }
        public int PetModelId { get; set; }
        public IEnumerable<PetModel> PetModel { get; set; }
    }
}
