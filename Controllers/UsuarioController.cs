using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SistemaDeAnimaisMVC.Models;
using SistemaDeAnimaisMVC.Servico.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeAnimaisMVC.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly ILogger<UsuarioController> _logger;
        private readonly string _uri = "https://localhost:44367/";
        private readonly ISessaoLogin _sessaoLogin;
        public IHttpContextAccessor _httpContextAccessor { get; set; }

        public UsuarioController(ILogger<UsuarioController> logger, IHttpContextAccessor httpContextAccessor, ISessaoLogin sessaoLogin)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _sessaoLogin = sessaoLogin;
        }

        [AllowAnonymous]
        //[Authorize]
        [HttpGet]
        public IActionResult IndexLogin()
        {

            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("IndexUsuario", "Usuario");

            }
            else
                return View();
        }


        [HttpGet]
        public async Task<IActionResult> LoginSair()
        {
               await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("IndexLogin", "Usuario");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult LoginIn(LoginModel login, string returnUrl)
        {
            try
            {
                var serializerModelLogin = JsonConvert.SerializeObject(new
                {
                    email = login.Email,
                    Password = login.Password,
                    confirmPassword = login.Password
                });

                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(_uri);

                var stringContent = new StringContent(serializerModelLogin, Encoding.UTF8, "application/json");
                var returnRequest = client.PostAsync("/api/Autoriza/login", stringContent).GetAwaiter().GetResult();

                if (returnRequest.StatusCode.Equals(System.Net.HttpStatusCode.BadRequest))
                {
                    return BadRequest(new ReturnLoginModel { Msg = "Erro na API", Status = true });
                }

                if (returnRequest.StatusCode.Equals(System.Net.HttpStatusCode.Unauthorized) ||
                    returnRequest.StatusCode.Equals(System.Net.HttpStatusCode.NotFound))
                {
                    return BadRequest(new ReturnLoginModel { Msg = "Acesso não autorizado ou caminho não existe", Status = true });
                }

                var returnContent = returnRequest.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                ReturnLoginModel baseResult = JsonConvert.DeserializeObject<ReturnLoginModel>(returnContent);

                if (!baseResult.Authenticated)
                {
                    return BadRequest(baseResult);
                }

               var claims = new List<Claim>{
               new Claim(ClaimTypes.Name, login.Email),
               new Claim("LoginAtual", login.Email),
               new Claim("access_token",baseResult.Token ),
            };

                claims.Add(new Claim(ClaimTypes.Role, "Administrador"));

                var identidadeDeUsuario = new ClaimsIdentity(claims, "xsistemas");
                ClaimsPrincipal claimPrincipal = new ClaimsPrincipal(identidadeDeUsuario);
                var propriedadesDeAutenticacao = new AuthenticationProperties
                {
                    AllowRefresh = false,
                    ExpiresUtc = DateTime.Now.ToLocalTime().AddHours(02),
                    IsPersistent = true
                };
                _httpContextAccessor.HttpContext.SignInAsync(claimPrincipal, propriedadesDeAutenticacao);

                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return Redirect("IndexUsuario");

            }
            catch (Exception ex)
            {
                return BadRequest(new ReturnLoginModel { Msg = ex.Message, Status = true });
            }
        }

        [Authorize]
        [HttpGet]
        public IActionResult IndexUsuario()
        {
            try
            {
                string token = User.FindFirstValue("access_token");
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                client.DefaultRequestHeaders.Accept.Clear();
                client.BaseAddress = new Uri(_uri);

                var returnRequest = client.GetAsync("/api/Usuario/BuscarTodosUsuarios").GetAwaiter().GetResult();

                if (returnRequest.StatusCode.Equals(System.Net.HttpStatusCode.BadRequest))
                {
                    TempData["MensagemSucesso"] = returnRequest.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    return View();
                }

                if (returnRequest.StatusCode.Equals(System.Net.HttpStatusCode.Unauthorized) ||
                    returnRequest.StatusCode.Equals(System.Net.HttpStatusCode.NotFound))
                {
                    TempData["MensagemSucesso"] = returnRequest.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    return View();
                }

                var returnContent = returnRequest.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                List<UsuarioModel> baseResult = JsonConvert.DeserializeObject<List<UsuarioModel>>(returnContent);

                return View(baseResult);

            }
            catch (Exception ex)
            {
                TempData["MensagemSucesso"] = ex.Message;
                return View();
            }
        }


        [Authorize]
        [HttpGet]
        public IActionResult GetUsuario()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(_uri);

            var returnRequest = client.GetAsync("/api/Autoriza/register").GetAwaiter().GetResult();

            if (returnRequest.StatusCode.Equals(System.Net.HttpStatusCode.BadRequest))
            {
                return BadRequest(new ReturnLoginModel { Msg = "Erro na API", Status = true });
            }

            if (returnRequest.StatusCode.Equals(System.Net.HttpStatusCode.Unauthorized) ||
                returnRequest.StatusCode.Equals(System.Net.HttpStatusCode.NotFound))
            {
                return BadRequest(new ReturnLoginModel { Msg = "Acesso não autorizado ou caminho não existe", Status = true });
            }

            var returnContent = returnRequest.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            List<ReturnUsuarioModel> baseResult = JsonConvert.DeserializeObject<List<ReturnUsuarioModel>>(returnContent);

            return Ok(baseResult);
        }

        [Authorize]
        [HttpGet]
        public IActionResult CriarUsuario(int id)
        {
            try
            {
                string token = User.FindFirstValue("access_token");
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                client.DefaultRequestHeaders.Accept.Clear();
                client.BaseAddress = new Uri(_uri);

                var returnRequest = client.GetAsync($"/api/Usuario/{id}").GetAwaiter().GetResult();

                if (returnRequest.StatusCode.Equals(System.Net.HttpStatusCode.BadRequest))
                {
                    TempData["MensagemSucesso"] = returnRequest.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    return View();

                }

                if (returnRequest.StatusCode.Equals(System.Net.HttpStatusCode.Unauthorized) ||
                    returnRequest.StatusCode.Equals(System.Net.HttpStatusCode.NotFound))
                {
                    TempData["MensagemSucesso"] = returnRequest.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    return View();

                }

                var returnContent = returnRequest.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                UsuarioModel baseResult = JsonConvert.DeserializeObject<UsuarioModel>(returnContent, new JsonSerializerSettings()
                { Culture = System.Globalization.CultureInfo.CurrentCulture });

                return View(baseResult);
            }
            catch (Exception ex)
            {
                TempData["MensagemSucesso"] = ex.Message;
                return View();
            }
        }

        [Authorize]
        [HttpPost]
        public IActionResult CriarUsuario(UsuarioModel usuarioModel)
        {
            try
            {

                var serializerModelUsuario = JsonConvert.SerializeObject(usuarioModel);
                string token = User.FindFirstValue("access_token");
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                client.DefaultRequestHeaders.Accept.Clear();
                client.BaseAddress = new Uri(_uri);

                var stringContent = new StringContent(serializerModelUsuario, Encoding.UTF8, "application/json");
                var returnRequest = client.PostAsync("/api/Usuario/CriarUsuario", stringContent).GetAwaiter().GetResult();



                if (returnRequest.StatusCode.Equals(System.Net.HttpStatusCode.BadRequest))
                {
                    TempData["MensagemSucesso"] = returnRequest.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    return View();
                }

                if (returnRequest.StatusCode.Equals(System.Net.HttpStatusCode.Unauthorized) ||
                    returnRequest.StatusCode.Equals(System.Net.HttpStatusCode.NotFound))
                {
                    TempData["MensagemSucesso"] = returnRequest.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    return View();
                }

                //teste novo
                if (ModelState.IsValid)
                {
                    //SucessoMensagem

                    TempData["SucessoMensagem"] = "Contato cadastrado com sucesso";
                    //return View("IndexUsuario");
                    return View();
                }

                var returnContent = returnRequest.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                UsuarioModel baseResult = JsonConvert.DeserializeObject<UsuarioModel>(returnContent);

                return Redirect("IndexUsuario");
            }
            catch (Exception ex)
            {

                TempData["MensagemError"] = $"Não foi possível cadastrar o contato:{ex}";
                return View();
            }
        }
        
        [Authorize]
        [HttpGet]
        public IActionResult EditUsuarios(int id)
        {
            try
            {
                string token = User.FindFirstValue("access_token");
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                client.DefaultRequestHeaders.Accept.Clear();
                client.BaseAddress = new Uri(_uri);

                var returnRequest = client.GetAsync($"/api/Usuario/{id}").GetAwaiter().GetResult();

                if (returnRequest.StatusCode.Equals(System.Net.HttpStatusCode.BadRequest))
                {
                    TempData["MensagemSucesso"] = returnRequest.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    return View();
                }

                if (returnRequest.StatusCode.Equals(System.Net.HttpStatusCode.Unauthorized) ||
                    returnRequest.StatusCode.Equals(System.Net.HttpStatusCode.NotFound))
                {
                    TempData["MensagemSucesso"] = returnRequest.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    return View();
                }

                var returnContent = returnRequest.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                UsuarioModel baseResult = JsonConvert.DeserializeObject<UsuarioModel>(returnContent, new JsonSerializerSettings()
                { Culture = System.Globalization.CultureInfo.CurrentCulture });

                return View(baseResult);
            }
            catch (Exception ex)
            {
                TempData["MensagemSucesso"] = ex.Message;
                return View();
            }
        }

        [Authorize]
        [HttpPost]
        public IActionResult EditUsuarios(UsuarioModel usuarioModel, int id)
        {
            try
            {
                var serializerModel = JsonConvert.SerializeObject(usuarioModel);
                string token = User.FindFirstValue("access_token");
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                client.DefaultRequestHeaders.Accept.Clear();
                client.BaseAddress = new Uri(_uri);

                var stringContent = new StringContent(serializerModel, Encoding.UTF8, "application/json");
                var returnRequest = client.PutAsync($"/api/Usuario/{id}", stringContent).GetAwaiter().GetResult();

                if (returnRequest.StatusCode.Equals(System.Net.HttpStatusCode.BadRequest))
                {
                    TempData["MensagemSucesso"] = returnRequest.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    return View();
                }

                if (returnRequest.StatusCode.Equals(System.Net.HttpStatusCode.Unauthorized) ||
                    returnRequest.StatusCode.Equals(System.Net.HttpStatusCode.NotFound))
                {
                    TempData["MensagemSucesso"] = returnRequest.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    return View();
                }

                //teste novo
                if (returnRequest.StatusCode.Equals(System.Net.HttpStatusCode.BadRequest) && ModelState.IsValid)
                {
                    TempData["SucessoMensagem"] = "Contato cadastrado com sucesso";
                    //return View("IndexUsuario");
                    return View();
                }

                var returnContent = returnRequest.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                UsuarioModel baseResult = JsonConvert.DeserializeObject<UsuarioModel>(returnContent);

                return Redirect("IndexUsuario");
            }
            catch (Exception ex)
            {
                TempData["MensagemError"] = $"Não foi possível cadastrar o contato:{ex}";
                return View();
            }
        }


        [Authorize]
        [HttpGet]
        public IActionResult Apagar(int id)
        {
            try
            {
                string token = User.FindFirstValue("access_token");
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                client.DefaultRequestHeaders.Accept.Clear();
                client.BaseAddress = new Uri(_uri);

                var returnRequest = client.DeleteAsync($"api/Usuario/{id}").GetAwaiter().GetResult();

                if (returnRequest.StatusCode.Equals(System.Net.HttpStatusCode.BadRequest))
                {
                    TempData["MensagemSucesso"] = returnRequest.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    return View();
                }

                if (returnRequest.StatusCode.Equals(System.Net.HttpStatusCode.Unauthorized) ||
                returnRequest.StatusCode.Equals(System.Net.HttpStatusCode.NotFound))
                {
                    TempData["MensagemSucesso"] = returnRequest.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    return View();
                }

                var returnContent = returnRequest.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                UsuarioModel baseResult = JsonConvert.DeserializeObject<UsuarioModel>(returnContent, new JsonSerializerSettings()
                { Culture = System.Globalization.CultureInfo.CurrentCulture });

                return View(baseResult);
            }
            catch (Exception ex)
            {
                TempData["MensagemSucesso"] = ex.Message;
                return View();
            }

        }

        [Authorize]
        [HttpGet]
        public IActionResult ApagarPost(int id)
        {
            try
            {
                string token = User.FindFirstValue("access_token");
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                client.DefaultRequestHeaders.Accept.Clear();
                client.BaseAddress = new Uri(_uri);

                var returnRequest = client.GetAsync($"/api/Usuario/{id}").GetAwaiter().GetResult();
                if (returnRequest.StatusCode.Equals(System.Net.HttpStatusCode.BadRequest))
                {
                    TempData["MensagemSucesso"] = returnRequest.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    return View();
                }

                if (returnRequest.StatusCode.Equals(System.Net.HttpStatusCode.Unauthorized) ||
                    returnRequest.StatusCode.Equals(System.Net.HttpStatusCode.NotFound))
                {
                    TempData["MensagemSucesso"] = returnRequest.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    return View();
                }

                //teste novo
                if (returnRequest.StatusCode.Equals(System.Net.HttpStatusCode.BadRequest) && ModelState.IsValid)
                {
                    TempData["SucessoMensagem"] = "Contato apagado com sucesso";
                    //return View("IndexUsuario");
                    return View();
                }

                var returnContent = returnRequest.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                UsuarioModel baseResult = JsonConvert.DeserializeObject<UsuarioModel>(returnContent, new JsonSerializerSettings()
                { Culture = System.Globalization.CultureInfo.CurrentCulture });

                return View("IndexUsuario");
            }
            catch (Exception ex)
            {
                TempData["MensagemError"] = $"Não foi possível cadastrar o contato:{ex}";
                //return View();
                //TempData["MensagemSucesso"] = ex.Message;
                return Redirect("IndexUsuario");
            }
        }
    }
}
