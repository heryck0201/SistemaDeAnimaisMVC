using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using SistemaDeAnimaisMVC.Models;
using SistemaDeAnimaisMVC.Servico.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaDeAnimaisMVC.Servico
{
    public class SessaoLogin : ISessaoLogin
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SessaoLogin(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public LoginModel BuscarSessaoDoUsuario()
        {
            string sessaoUsuario = _httpContextAccessor.HttpContext.Session.GetString("sessaoUsuarioLogado");

            if (string.IsNullOrEmpty(sessaoUsuario)) return null;

            return JsonConvert.DeserializeObject<LoginModel>(sessaoUsuario);
        }

        public void CriarSessaoDoUsuario(LoginModel login)
        {
            string valor = JsonConvert.SerializeObject(login);

            _httpContextAccessor.HttpContext.Session.SetString("sessaoUsuarioLogado", valor);
        }

        public void RemoverSessaoDoUsuario()
        {
            _httpContextAccessor.HttpContext.Session.Remove("sessaoUsuarioLogado");
        }
    }
}
