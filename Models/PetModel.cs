
namespace SistemaDeAnimaisMVC.Models
{
    public class PetModel
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Raca { get; set; }
        public string Cor { get; set; }
        public string Porte { get; set; }
        public int UsuarioId { get; set; }
        public UsuarioModel UsuarioModel { get; set; }
    }
}
