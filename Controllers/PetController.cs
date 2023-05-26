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

namespace TesteNetMoveis.Controllers
{
    public class PetController : Controller
    {
        private readonly ILogger<PetController> _logger;
        private readonly string _uri = "https://localhost:44367/";
        public IHttpContextAccessor _httpContextAccessor { get; set; }

        public PetController(ILogger<PetController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;

        }

        [Authorize]
        [HttpGet]
        public IActionResult IndexPet()
        {
            try
            {
                string token = User.FindFirstValue("access_token");
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                client.DefaultRequestHeaders.Accept.Clear();
                client.BaseAddress = new Uri(_uri);

                var returnRequest = client.GetAsync("/api/Pet/BuscarTodosPet").GetAwaiter().GetResult();

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
                List<PetModel> baseResult = JsonConvert.DeserializeObject<List<PetModel>>(returnContent);

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
        public IActionResult CriarPet()
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
                List<UsuarioModel> baseResult = JsonConvert.DeserializeObject<List<UsuarioModel>>(returnContent,
                                                new JsonSerializerSettings()
                                                {
                                                    Culture = System.Globalization.CultureInfo.CurrentCulture
                                                });

                ViewBag.UserList = baseResult;

                //return Json(baseResult);
                return View();
            }
            catch (Exception ex)
            {
                TempData["MensagemSucesso"] = ex.Message;
                return View();
            }
        }

        [Authorize]
        [HttpPost]
        public IActionResult CriarPet(PetModel petModel)
        {
            try
            {
                var serializerModelPet = JsonConvert.SerializeObject(petModel);
                string token = User.FindFirstValue("access_token");
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                client.DefaultRequestHeaders.Accept.Clear();
                client.BaseAddress = new Uri(_uri);

                var stringContent = new StringContent(serializerModelPet, Encoding.UTF8, "application/json");
                var returnRequest = client.PostAsync("/api/Pet/CriarPet", stringContent).GetAwaiter().GetResult();

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
                PetModel baseResult = JsonConvert.DeserializeObject<PetModel>(returnContent);

                return RedirectToAction("IndexPet", "Pet");
            }
            catch (Exception ex)
            {

                TempData["MensagemSucesso"] = ex.Message;
                return View();
            }
        }

        [Authorize]
        [HttpGet]
        public IActionResult EditarPet(int id)
        {
            try
            {
                string token = User.FindFirstValue("access_token");
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                client.DefaultRequestHeaders.Accept.Clear();
                client.BaseAddress = new Uri(_uri);

                var returnRequest = client.GetAsync($"api/Pet/{id}").GetAwaiter().GetResult();

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
                PetModel baseResultPet = JsonConvert.DeserializeObject<PetModel>(returnContent, new JsonSerializerSettings()
                { Culture = System.Globalization.CultureInfo.CurrentCulture });

                var desUsuario = client.GetAsync("/api/Usuario/BuscarTodosUsuarios").GetAwaiter().GetResult();
                var usuarioContent = desUsuario.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                List<UsuarioModel> baseResult = JsonConvert.DeserializeObject<List<UsuarioModel>>(usuarioContent,
                                                new JsonSerializerSettings()
                                                {
                                                    Culture = System.Globalization.CultureInfo.CurrentCulture
                                                });

                ViewBag.UserList = baseResult;

                return View(baseResultPet);
            }
            catch (Exception ex)
            {
                TempData["MensagemSucesso"] = ex.Message;
                return View();
            }
        }

        [Authorize]
        [HttpPost]
        public IActionResult EditarPet(PetModel petModel, int id)
        {
            try
            {
                var serializerModel = JsonConvert.SerializeObject(petModel);
                string token = User.FindFirstValue("access_token");
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                client.DefaultRequestHeaders.Accept.Clear();
                client.BaseAddress = new Uri(_uri);

                var stringContent = new StringContent(serializerModel, Encoding.UTF8, "application/json");
                var returnRequest = client.PutAsync($"/api/Pet/{id}", stringContent).GetAwaiter().GetResult();

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
                PetModel baseResult = JsonConvert.DeserializeObject<PetModel>(returnContent);

                //ViewBag.UserList = client.GetAsync("/api/Usuario/BuscarTodosUsuarios").GetAwaiter().GetResult();

                //return View(baseResult);
                return RedirectToAction("IndexPet", "Pet");
            }
            catch (Exception ex)
            {
                TempData["MensagemSucesso"] = ex.Message;
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

                var returnRequest = client.DeleteAsync($"api/Pet/{id}").GetAwaiter().GetResult();

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
                PetModel baseResult = JsonConvert.DeserializeObject<PetModel>(returnContent, new JsonSerializerSettings()
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

                var returnRequest = client.GetAsync($"/api/Pet/{id}").GetAwaiter().GetResult();
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
                PetModel baseResult = JsonConvert.DeserializeObject<PetModel>(returnContent, new JsonSerializerSettings()
                { Culture = System.Globalization.CultureInfo.CurrentCulture });

                return View("IndexPet");
            }
            catch (Exception ex)
            {
                TempData["MensagemSucesso"] = ex.Message;
                return Redirect("Apagar");
            }
        }
    }
}
