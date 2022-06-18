using eBookStore.Filters;
using eBookStore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace eBookStore.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly HttpClient _client;
        private readonly string AuthenticationApiUrl = "https://localhost:44303/api/Authentication";
        public AuthenticationController()
        {
            _client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (string.IsNullOrWhiteSpace(HttpContext.Session.GetString("jwtToken")))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(AuthenticateRequest authenticateRequest)
        {
            if (ModelState.IsValid)
            {
                string content = System.Text.Json.JsonSerializer.Serialize(authenticateRequest);
                var data = new StringContent(content, Encoding.UTF8, "application/json");
                var response = await _client.PostAsync($"{AuthenticationApiUrl}", data);
                string strData = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.Error = strData;
                }
                else
                {
                    var authenticationResponse = JsonConvert.DeserializeObject<AuthenticateResponse>(strData);
                    if (authenticationResponse != null)
                    {
                        HttpContext.Session.SetString("jwtToken", authenticationResponse.Token);
                        HttpContext.Session.SetString("email", authenticationResponse.EmailAddress);
                        HttpContext.Session.SetInt32("id", authenticationResponse.UserId);
                        HttpContext.Session.SetString("role", authenticationResponse.Role.RoleDesc);
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            return View("Login", authenticateRequest);

        }

        [Authorize]
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
