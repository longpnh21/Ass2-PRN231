using BusinessObject;
using eBookStore.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace eBookStore.Controllers
{
    [Authorize("Admin")]
    public class UsersController : Controller
    {
        private readonly string UserApiUrl = "https://localhost:44303/odata/users";
        private readonly string PublisherApiUrl = "https://localhost:44303/odata/publishers";
        private readonly string RoleApiUrl = "https://localhost:44303/odata/roles";
        private HttpClient _client;

        private bool SetUpHttpClient()
        {
            _client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            string token = HttpContext.Session.GetString("jwtToken");

            if (string.IsNullOrEmpty(token))
            {
                return false;
            }

            _client.DefaultRequestHeaders.Accept.Add(contentType);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return true;
        }

        public async Task<IActionResult> Index()
        {
            if (!SetUpHttpClient())
            {
                return View("Unauthorized");
            }

            var respone = await _client.GetAsync($"{UserApiUrl}");
            string strData = await respone.Content.ReadAsStringAsync();

            List<User> items = new();

            dynamic temp = JObject.Parse(strData);

            if ((JArray)temp.value == null)
            {
                return View("NotFound");
            }

            items = ((JArray)temp.value).Select(x => new User
            {
                UserId = (int)x["UserId"],
                EmailAddress = (string)x["EmailAddress"],
                Source = (string)x["Source"],
                FirstName = (string)x["FirstName"],
                MiddleName = (string)x["MiddleName"],
                LastName = (string)x["LastName"],
                RoleId = (int)x["RoleId"],
                PubId = (int?)x["PubId"],
                HireDate = (DateTime?)x["HireDate"]
            }).ToList();
            return View(items);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (!SetUpHttpClient())
            {
                return View("Unauthorized");
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

        public async Task<IActionResult> Create()
        {
            if (!SetUpHttpClient())
            {
                return View("Unauthorized");
            }

            var publishersResponse = await _client.GetAsync($"{PublisherApiUrl}?$Select=PubId,PublisherName");
            var rolesResponse = await _client.GetAsync($"{RoleApiUrl}");

            string publishersStrData = await publishersResponse.Content.ReadAsStringAsync();
            dynamic publishersTemp = JObject.Parse(publishersStrData);
            var publishers = ((JArray)publishersTemp.value).Select(x => new Publisher
            {
                PubId = (int)x["PubId"],
                PublisherName = (string)x["PublisherName"]
            }).ToList();
            ViewData["PubId"] = new SelectList(publishers, "PubId", "PublisherName");

            string rolesStrData = await rolesResponse.Content.ReadAsStringAsync();
            dynamic rolesTemp = JObject.Parse(rolesStrData);
            var roles = ((JArray)rolesTemp.value).Select(x => new Role
            {
                RoleId = (int)x["RoleId"],
                RoleDesc = (string)x["RoleDesc"]
            }).ToList();
            ViewData["RoleId"] = new SelectList(roles, "RoleId", "RoleDesc");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmailAddress,Password,Source,FirstName,MiddleName,LastName,RoleId,PubId,HireDate")] User user)
        {
            if (!SetUpHttpClient())
            {
                return View("Unauthorized");
            }

            if (ModelState.IsValid)
            {
                string content = JsonSerializer.Serialize(new
                {
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

                var response = await _client.PostAsync($"{UserApiUrl}", data);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.Error = await response.Content.ReadAsStringAsync();
            }

            var publishersResponse = await _client.GetAsync($"{PublisherApiUrl}?$Select=PubId,PublisherName");
            var rolesResponse = await _client.GetAsync($"{RoleApiUrl}");

            string publishersStrData = await publishersResponse.Content.ReadAsStringAsync();
            dynamic publishersTemp = JObject.Parse(publishersStrData);
            var publishers = ((JArray)publishersTemp.value).Select(x => new Publisher
            {
                PubId = (int)x["PubId"],
                PublisherName = (string)x["PublisherName"]
            }).ToList();
            ViewData["PubId"] = new SelectList(publishers, "PubId", "PublisherName");

            string rolesStrData = await rolesResponse.Content.ReadAsStringAsync();
            dynamic rolesTemp = JObject.Parse(rolesStrData);
            var roles = ((JArray)rolesTemp.value).Select(x => new Role
            {
                RoleId = (int)x["RoleId"],
                RoleDesc = (string)x["RoleDesc"]
            }).ToList();
            ViewData["RoleId"] = new SelectList(roles, "RoleId", "RoleDesc");
            return View(user);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (!SetUpHttpClient())
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

            var publishersResponse = await _client.GetAsync($"{PublisherApiUrl}?$Select=PubId,PublisherName");
            var rolesResponse = await _client.GetAsync($"{RoleApiUrl}");

            string publishersStrData = await publishersResponse.Content.ReadAsStringAsync();
            dynamic publishersTemp = JObject.Parse(publishersStrData);
            var publishers = ((JArray)publishersTemp.value).Select(x => new Publisher
            {
                PubId = (int)x["PubId"],
                PublisherName = (string)x["PublisherName"]
            }).ToList();
            ViewData["PubId"] = new SelectList(publishers, "PubId", "PublisherName");

            string rolesStrData = await rolesResponse.Content.ReadAsStringAsync();
            dynamic rolesTemp = JObject.Parse(rolesStrData);
            var roles = ((JArray)rolesTemp.value).Select(x => new Role
            {
                RoleId = (int)x["RoleId"],
                RoleDesc = (string)x["RoleDesc"]
            }).ToList();
            ViewData["RoleId"] = new SelectList(roles, "RoleId", "RoleDesc");

            return View(user);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,EmailAddress,Password,Source,FirstName,MiddleName,LastName,RoleId,PubId,HireDate")] User user)
        {
            if (!SetUpHttpClient())
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

            var publishersResponse = await _client.GetAsync($"{PublisherApiUrl}?$Select=PubId,PublisherName");
            var rolesResponse = await _client.GetAsync($"{RoleApiUrl}");

            string publishersStrData = await publishersResponse.Content.ReadAsStringAsync();
            dynamic publishersTemp = JObject.Parse(publishersStrData);
            var publishers = ((JArray)publishersTemp.value).Select(x => new Publisher
            {
                PubId = (int)x["PubId"],
                PublisherName = (string)x["PublisherName"]
            }).ToList();
            ViewData["PubId"] = new SelectList(publishers, "PubId", "PublisherName");

            string rolesStrData = await rolesResponse.Content.ReadAsStringAsync();
            dynamic rolesTemp = JObject.Parse(rolesStrData);
            var roles = ((JArray)rolesTemp.value).Select(x => new Role
            {
                RoleId = (int)x["RoleId"],
                RoleDesc = (string)x["RoleDesc"]
            }).ToList();

            ViewData["RoleId"] = new SelectList(roles, "RoleId", "RoleDesc");
            return View(user);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (!SetUpHttpClient())
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

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (!SetUpHttpClient())
            {
                return View("Unauthorized");
            }

            if (id == null)
            {
                return NotFound();
            }

            if (HttpContext.Session.GetInt32("id").Value == id)
            {
                ViewBag.Error = "Cannot remove yourself";
            }

            var response = await _client.DeleteAsync($"{UserApiUrl}({id})");

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = await response.Content.ReadAsStringAsync();
                return RedirectToAction(nameof(Delete), new { id = id });
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
