using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SistemaDeAnimaisMVC.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace TesteNetMoveis.Controllers
{
    public class PetController : Controller
    {
        [Authorize]
        [HttpGet]
        public IActionResult Index()
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("https://localhost:7266/swagger/index.html");

                var returnRequest = client.GetAsync("/api/login").GetAwaiter().GetResult();

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
            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult Criar(PetModel obj)
        {
            try
            {
                var serializerModelImovel = JsonConvert.SerializeObject(obj);

                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("https://localhost:7266/swagger/index.html");

                var stringContent = new StringContent(serializerModelImovel, Encoding.UTF8, "application/json");
                var returnRequest = client.PostAsync(" / api / login", stringContent).GetAwaiter().GetResult();

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

                return Redirect("Index");
            }
            catch (Exception ex)
            {

                TempData["MensagemSucesso"] = ex.Message;
                return View();
            }

        }

        [Authorize]
        [HttpGet]
        public IActionResult Editar(int id)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("https://localhost:7266/swagger/index.html");

                var returnRequest = client.GetAsync("/api/login" + id).GetAwaiter().GetResult();

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
        [HttpPost]
        public IActionResult EditarPet(PetModel obj)
        {
            try
            {
                var serializerModelImovel = JsonConvert.SerializeObject(obj);

                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("https://localhost:7266/swagger/index.html");

                var stringContent = new StringContent(serializerModelImovel, Encoding.UTF8, "application/json");
                var returnRequest = client.PutAsync("/api/login" + obj.Id, stringContent).GetAwaiter().GetResult();

                if (returnRequest.StatusCode.Equals(System.Net.HttpStatusCode.BadRequest))
                {
                    TempData["MensagemSucesso"] = returnRequest.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    return Redirect("Editar?id=" + obj.Id);
                }

                if (returnRequest.StatusCode.Equals(System.Net.HttpStatusCode.Unauthorized) ||
                    returnRequest.StatusCode.Equals(System.Net.HttpStatusCode.NotFound))
                {
                    TempData["MensagemSucesso"] = returnRequest.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    return Redirect("Editar?id=" + obj.Id);
                }

                var returnContent = returnRequest.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                PetModel baseResult = JsonConvert.DeserializeObject<PetModel>(returnContent);

                return Redirect("Index");
            }
            catch (Exception ex)
            {
                TempData["MensagemSucesso"] = ex.Message;
                return View();
            }
        }

        [Authorize]
        [HttpGet]
        public IActionResult Detalhe(int id)
        {

            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("https://localhost:7266/swagger/index.html");

                var returnRequest = client.GetAsync("/api/login" + id).GetAwaiter().GetResult();

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
                PetModel baseResult = JsonConvert.DeserializeObject<PetModel>(returnContent, new JsonSerializerSettings() { Culture = System.Globalization.CultureInfo.CurrentCulture });

                return View(baseResult);
            }
            catch (Exception ex)
            {
                TempData["MensagemSucesso"] = ex.Message;
                return Redirect("Detalhe");
            }
        }

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
                client.BaseAddress = new Uri("https://localhost:7266/swagger/index.html");

                var returnRequest = client.DeleteAsync("/api/login" + id).GetAwaiter().GetResult();
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
                PetModel baseResult = JsonConvert.DeserializeObject<PetModel>(returnContent, new JsonSerializerSettings() { Culture = System.Globalization.CultureInfo.CurrentCulture });

                return View("Index");
            }
            catch (Exception ex)
            {

                TempData["MensagemSucesso"] = ex.Message;
                return Redirect("Apagar");
            }

        }
    }
}
