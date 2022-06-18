using BusinessObject;
using eBookStore.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace eBookStore.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly string UserApiUrl = "https://localhost:44303/odata/users";
        private HttpClient _client;

        private bool? SetUpHttpClient()
        {
            _client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            string token = HttpContext.Session.GetString("jwtToken");
            int? id = HttpContext.Session.GetInt32("id");

            if (string.IsNullOrEmpty(token) || id == null)
            {
                return false;
            }

            if (id.Value == 0)
            {
                return null;
            }

            _client.DefaultRequestHeaders.Accept.Add(contentType);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return true;
        }

        public async Task<IActionResult> Index()
        {
            if (SetUpHttpClient() == null)
            {
                return NotFound();
            }

            if (!SetUpHttpClient().Value)
            {
                return View("Unauthorized");
            }

            int? id = HttpContext.Session.GetInt32("id");

            var response = await _client.GetAsync($"{UserApiUrl}({id.Value})");
            string strData = await response.Content.ReadAsStringAsync();

            var temp = JObject.Parse(strData);

            if (temp == null)
            {
                return View("NotFound");
            }

            var user = new User
            {
                UserId = (int)temp["UserId"],
                EmailAddress = (string)temp["EmailAddress"],
                Source = (string)temp["Source"],
                FirstName = (string)temp["FirstName"],
                MiddleName = (string)temp["MiddleName"],
                LastName = (string)temp["LastName"],
                RoleId = (int)temp["RoleId"],
                PubId = (int?)temp["PubId"],
                HireDate = (DateTime?)temp["HireDate"]
            };

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (SetUpHttpClient() == null)
            {
                return NotFound();
            }

            if (!SetUpHttpClient().Value)
            {
                return View("Unauthorized");
            }

            if (id == null)
            {
                return NotFound();
            }

            var response = await _client.GetAsync($"{UserApiUrl}({id})");
            string strData = await response.Content.ReadAsStringAsync();

            var temp = JObject.Parse(strData);

            if (temp == null)
            {
                return View("NotFound");
            }

            var user = new User
            {
                UserId = (int)temp["UserId"],
                EmailAddress = (string)temp["EmailAddress"],
                Source = (string)temp["Source"],
                FirstName = (string)temp["FirstName"],
                MiddleName = (string)temp["MiddleName"],
                LastName = (string)temp["LastName"],
                RoleId = (int)temp["RoleId"],
                PubId = (int?)temp["PubId"],
                HireDate = (DateTime?)temp["HireDate"]
            };

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,EmailAddress,Password,Source,FirstName,MiddleName,LastName,RoleId,PubId,HireDate")] User user)
        {
            if (SetUpHttpClient() == null)
            {
                return NotFound();
            }
            if (!SetUpHttpClient().Value)
            {
                return View("Unauthorized");
            }

            if (id != user.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                string content = JsonSerializer.Serialize(new
                {
                    UserId = user.UserId,
                    EmailAddress = user.EmailAddress,
                    Password = user.Password,
                    Source = user.Source,
                    FirstName = user.FirstName,
                    MiddleName = user.MiddleName,
                    LastName = user.LastName,
                    RoleId = user.RoleId,
                    PubId = user.PubId,
                    HireDate = user.HireDate
                });
                var data = new StringContent(content, Encoding.UTF8, "application/json");

                var response = await _client.PutAsync($"{UserApiUrl}({id})", data);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.Error = await response.Content.ReadAsStringAsync();
            }

            return View(user);
        }
    }
}
