using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SistemaDeAnimaisMVC.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

namespace SistemaDeAnimaisMVC.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly ILogger<UsuarioController> _logger;
        private readonly string _uri = "https://localhost:44367/";
        public IHttpContextAccessor _httpContextAccessor { get; set; }

        public UsuarioController(ILogger<UsuarioController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;

        }

        [Authorize]
        [AllowAnonymous]
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

        [Authorize]
        [AllowAnonymous]
        [HttpGet]
        public IActionResult LoginIn(LoginModel login)
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

                //return View("IndexUsuario");
                return RedirectToAction("IndexUsuario");

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
            return View();
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
        public IActionResult CriarUsuario(string nome)
        {
            if (nome == "")
            {
                ViewBag.Error = "nome não informado";
                return View(new UsuarioModel());
            }

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(_uri);

            var returnRequest = client.GetAsync("/api/Usuario/BuscarId" + nome).GetAwaiter().GetResult();

            if (returnRequest.StatusCode.Equals(System.Net.HttpStatusCode.BadRequest))
            {
                ViewBag.Error = "Erro na API";
                return View(new UsuarioModel());
            }

            if (returnRequest.StatusCode.Equals(System.Net.HttpStatusCode.Unauthorized) ||
                returnRequest.StatusCode.Equals(System.Net.HttpStatusCode.NotFound))
            {
                ViewBag.Error = "Acesso não autorizado ou caminho não existe";
                return View(new UsuarioModel());
            }

            var returnContent = returnRequest.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            UsuarioModel baseResult = JsonConvert.DeserializeObject<UsuarioModel>(returnContent);

            return View(baseResult);
        }

        [Authorize]
        [HttpPost]
        public IActionResult CriarUsuario(UsuarioModel usuarioModel)
        {
            if (usuarioModel.Nome == "" || usuarioModel.Email == "" || usuarioModel.Nome == null || usuarioModel.Email == null
                || usuarioModel.Id == null)
            {
                return BadRequest(new ReturnLoginModel { Msg = "Nome e/ou Email", Status = true });
            }
            else
            {
                try
                {
                    //instância do modelo de dados do usuario
                    var serializerModelLogin = JsonConvert.SerializeObject(new
                    {
                        id = usuarioModel.Id,
                        nome = usuarioModel.Nome,
                        sobrenome = usuarioModel.Sobrenome,
                        petModelId = usuarioModel.PetModelId,
                        email = usuarioModel.Email,
                    });

                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri(_uri);

                    var stringContent = new StringContent(serializerModelLogin, Encoding.UTF8, "application/json");
                    var returnRequest = client.PostAsync("/api/Usuario/CriarUsuario", stringContent).GetAwaiter().GetResult();

                    if (returnRequest.StatusCode.Equals(System.Net.HttpStatusCode.BadRequest))
                    {
                        return BadRequest(new ReturnLoginModel { Msg = "Erro na API" });
                    }

                    if (returnRequest.StatusCode.Equals(System.Net.HttpStatusCode.Unauthorized) ||
                        returnRequest.StatusCode.Equals(System.Net.HttpStatusCode.NotFound))
                    {
                        return BadRequest(new ReturnLoginModel { Msg = "Acesso não autorizado ou caminho não existe", Status = true });
                    }

                    var returnContent = returnRequest.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    ReturnLoginModel baseResult = new ReturnLoginModel();
                    try
                    {
                        baseResult = JsonConvert.DeserializeObject<ReturnLoginModel>(returnContent);
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(new ReturnLoginModel { Msg = returnContent, Status = true });
                    }

                    if (!baseResult.Authenticated)
                    {
                        return BadRequest(baseResult);
                    }

                    return Redirect("IndexUsuario");
                }
                catch (Exception ex)
                {
                    return BadRequest(new ReturnLoginModel { Msg = ex.Message, Status = true });
                }
            }
        }

        //[Authorize]
        //[AllowAnonymous]
        //[HttpGet]
        //public IActionResult CriarUsuario()
        //{
        //    return View();
        //}

        //[Authorize]
        //[AllowAnonymous]
        //[HttpPost]
        //public IActionResult Criar(UsuarioModel usuarioModel)
        //{
        //    try
        //    {
        //        var serializerModelImovel = JsonConvert.SerializeObject(usuarioModel);

        //        HttpClient client = new HttpClient();
        //        client.BaseAddress = new Uri("https://62a0e2547b9345bcbe416e45.mockapi.io");

        //        var stringContent = new StringContent(serializerModelImovel, Encoding.UTF8, "application/json");
        //        var returnRequest = client.PostAsync("/api/v1/Imoveis", stringContent).GetAwaiter().GetResult();

        //        if (returnRequest.StatusCode.Equals(System.Net.HttpStatusCode.BadRequest))
        //        {
        //            TempData["MensagemSucesso"] = returnRequest.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        //            return View();
        //        }

        //        if (returnRequest.StatusCode.Equals(System.Net.HttpStatusCode.Unauthorized) ||
        //            returnRequest.StatusCode.Equals(System.Net.HttpStatusCode.NotFound))
        //        {
        //            TempData["MensagemSucesso"] = returnRequest.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        //            return View();
        //        }

        //        var returnContent = returnRequest.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        //        UsuarioModel baseResult = JsonConvert.DeserializeObject<UsuarioModel>(returnContent);

        //        return Redirect("IndexUsuario");
        //    }
        //    catch (Exception ex)
        //    {

        //        TempData["MensagemSucesso"] = ex.Message;
        //        return View();
        //    }

        //}

        [Authorize]
        [HttpGet]
        public IActionResult EditUsuario(string nome)
        {
            if (nome == "")
            {
                ViewBag.Error = "nome não informado";
                return View(new UsuarioModel());
            }

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(_uri);

            var returnRequest = client.GetAsync("/api/Usuario" + nome).GetAwaiter().GetResult();

            if (returnRequest.StatusCode.Equals(System.Net.HttpStatusCode.BadRequest))
            {
                ViewBag.Error = "Erro na API";
                return View(new UsuarioModel());
            }

            if (returnRequest.StatusCode.Equals(System.Net.HttpStatusCode.Unauthorized) ||
                returnRequest.StatusCode.Equals(System.Net.HttpStatusCode.NotFound))
            {
                ViewBag.Error = "Acesso não autorizado ou caminho não existe";
                return View(new UsuarioModel());
            }

            var returnContent = returnRequest.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            UsuarioModel baseResult = JsonConvert.DeserializeObject<UsuarioModel>(returnContent);

            return View(baseResult);

        }

        [Authorize]
        [HttpPost]
        public IActionResult EditUsuario(UsuarioModel usuarioModel)
        {
            if (usuarioModel.Nome == "" || usuarioModel.Email == "" || usuarioModel.Nome == null || usuarioModel.Email == null || usuarioModel.Id == null)
            {
                return BadRequest(new ReturnLoginModel { Msg = "Nome e/ou Email" });
            }
            else
            {
                try
                {
                    var serializerModelLogin = JsonConvert.SerializeObject(new
                    {
                        id = usuarioModel.Id,
                        nome = usuarioModel.Nome,
                        sobrenome = usuarioModel.Sobrenome,
                        petModelId = usuarioModel.PetModelId,
                        email = usuarioModel.Email,
                    });

                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri(_uri);

                    var stringContent = new StringContent(serializerModelLogin, Encoding.UTF8, "application/json");
                    var returnRequest = client.PutAsync("/api/Usuario/{AtualizarId}", stringContent).GetAwaiter().GetResult();

                    if (returnRequest.StatusCode.Equals(System.Net.HttpStatusCode.BadRequest))
                    {
                        return BadRequest(new ReturnLoginModel { Msg = "Erro na API" });
                    }

                    if (returnRequest.StatusCode.Equals(System.Net.HttpStatusCode.Unauthorized) ||
                        returnRequest.StatusCode.Equals(System.Net.HttpStatusCode.NotFound))
                    {
                        return BadRequest(new ReturnLoginModel { Msg = "Acesso não autorizado ou caminho não existe", Status = false });
                    }

                    var returnContent = returnRequest.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    ReturnLoginModel baseResult = new ReturnLoginModel();
                    try
                    {
                        baseResult = JsonConvert.DeserializeObject<ReturnLoginModel>(returnContent);
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(new ReturnLoginModel { Msg = returnContent, Status = false });
                    }

                    if (!baseResult.Authenticated)
                    {
                        return BadRequest(baseResult);
                    }

                    return Redirect("IndexUsuario");
                }
                catch (Exception ex)
                {
                    return BadRequest(new ReturnLoginModel { Msg = ex.Message, Status = false });
                }
            }
        }

        //[Authorize]
        //[HttpGet]
        //public IActionResult Detalhe(int id)
        //{

        //    try
        //    {
        //        HttpClient client = new HttpClient();
        //        client.BaseAddress = new Uri(_uri);

        //        var returnRequest = client.GetAsync("/api/Usuario/{id}" + id).GetAwaiter().GetResult();

        //        if (returnRequest.StatusCode.Equals(System.Net.HttpStatusCode.BadRequest))
        //        {
        //            TempData["MensagemSucesso"] = returnRequest.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        //            return View();
        //        }

        //        if (returnRequest.StatusCode.Equals(System.Net.HttpStatusCode.Unauthorized) ||
        //            returnRequest.StatusCode.Equals(System.Net.HttpStatusCode.NotFound))
        //        {
        //            TempData["MensagemSucesso"] = returnRequest.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        //            return View();
        //        }

        //        var returnContent = returnRequest.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        //        UsuarioModel baseResult = JsonConvert.DeserializeObject<UsuarioModel>(returnContent,
        //                                  new JsonSerializerSettings()
        //                                  { Culture = System.Globalization.CultureInfo.CurrentCulture });

        //        return View(baseResult);
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["MensagemSucesso"] = ex.Message;
        //        return Redirect("Detalhe");
        //    }
        //}

        //[Authorize]
        //[HttpGet]
        //public IActionResult MeuPet(int id)
        //{

        //    try
        //    {
        //        HttpClient client = new HttpClient();
        //        client.BaseAddress = new Uri(_uri);

        //        var returnRequest = client.GetAsync("/api/Usuario/{id}" + id).GetAwaiter().GetResult();

        //        if (returnRequest.StatusCode.Equals(System.Net.HttpStatusCode.BadRequest))
        //        {
        //            TempData["MensagemSucesso"] = returnRequest.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        //            return View();
        //        }

        //        if (returnRequest.StatusCode.Equals(System.Net.HttpStatusCode.Unauthorized) ||
        //            returnRequest.StatusCode.Equals(System.Net.HttpStatusCode.NotFound))
        //        {
        //            TempData["MensagemSucesso"] = returnRequest.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        //            return View();
        //        }

        //        var returnContent = returnRequest.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        //        PetModel baseResult = JsonConvert.DeserializeObject<PetModel>(returnContent,
        //                                  new JsonSerializerSettings()
        //                                  { Culture = System.Globalization.CultureInfo.CurrentCulture });

        //        return View(baseResult);
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["MensagemSucesso"] = ex.Message;
        //        return Redirect("Detalhe");
        //    }
        //}

        [Authorize]
        [HttpGet]
        public IActionResult Apagar(int id)
        {

            return View(id);
        }

        [Authorize]
        [HttpGet]
        public IActionResult ApagarPost(int id)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(_uri);

                var returnRequest = client.DeleteAsync("/api/Usuario/{ApagarId}" + id).GetAwaiter().GetResult();
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
                UsuarioModel baseResult = JsonConvert.DeserializeObject<UsuarioModel>
                                          (returnContent, new JsonSerializerSettings() { Culture = System.Globalization.CultureInfo.CurrentCulture });

                return View("IndexUsuario");
            }
            catch (Exception ex)
            {
                TempData["MensagemSucesso"] = ex.Message;
                return Redirect("Apagar");
            }

        }
    }
}
