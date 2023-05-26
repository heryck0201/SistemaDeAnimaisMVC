using SistemaDeAnimaisMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaDeAnimaisMVC.Servico.Interfaces
{
   public interface ISessaoLogin
    {
        void CriarSessaoDoUsuario(LoginModel login);
        void RemoverSessaoDoUsuario();
        LoginModel BuscarSessaoDoUsuario();
    }
}
