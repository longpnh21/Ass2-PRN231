using BusinessObject;
using eBookStore.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
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
    public class AuthorsController : Controller
    {
        private readonly string AuthorApiUrl = "https://localhost:44303/odata/authors";
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

            List<Author> items = new();

            var respone = await _client.GetAsync($"{AuthorApiUrl}");
            string strData = await respone.Content.ReadAsStringAsync();

            dynamic temp = JObject.Parse(strData);
            if ((JArray)temp.value != null)
            {
                items = ((JArray)temp.value).Select(x => new Author
                {
                    AuthorId = (int)x["AuthorId"],
                    LastName = (string)x["LastName"],
                    FirstName = (string)x["FirstName"],
                    Phone = (string)x["Phone"],
                    Address = (string)x["Address"],
                    City = (string)x["City"],
                    Zip = (int)x["Zip"],
                    EmailAddress = (string)x["EmailAddress"]
                }).ToList();
            }

            return View(items);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (!SetUpHttpClient())
            {
                return View("Unauthorized");
            }

            var response = await _client.GetAsync($"{AuthorApiUrl}({id})");
            string strData = await response.Content.ReadAsStringAsync();

            var temp = JObject.Parse(strData);

            if (temp == null)
            {
                return View("NotFound");
            }

            var author = new Author
            {
                AuthorId = (int)temp["AuthorId"],
                LastName = (string)temp["LastName"],
                FirstName = (string)temp["FirstName"],
                Phone = (string)temp["Phone"],
                Address = (string)temp["Address"],
                City = (string)temp["City"],
                Zip = (int)temp["Zip"],
                EmailAddress = (string)temp["EmailAddress"]
            };

            if (author == null)
            {
                return NotFound();
            }

            return View(author);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AuthorName,City,State,Country")] Author author)
        {
            if (!SetUpHttpClient())
            {
                return View("Unauthorized");
            }

            if (!ModelState.IsValid)
            {
                return View(author);
            }

            string content = JsonSerializer.Serialize(new
            {
                AuthorId = author.AuthorId,
                LastName = author.LastName,
                FirstName = author.FirstName,
                Phone = author.Phone,
                Address = author.Address,
                City = author.City,
                Zip = author.Zip,
                EmailAddress = author.EmailAddress
            });
            var data = new StringContent(content, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync($"{AuthorApiUrl}", data);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Error = await response.Content.ReadAsStringAsync();

            return View(author);
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

            var response = await _client.GetAsync($"{AuthorApiUrl}({id})");
            string strData = await response.Content.ReadAsStringAsync();

            var temp = JObject.Parse(strData);

            if (temp == null)
            {
                return View("NotFound");
            }

            var author = new Author
            {
                AuthorId = (int)temp["AuthorId"],
                LastName = (string)temp["LastName"],
                FirstName = (string)temp["FirstName"],
                Phone = (string)temp["Phone"],
                Address = (string)temp["Address"],
                City = (string)temp["City"],
                Zip = (int)temp["Zip"],
                EmailAddress = (string)temp["EmailAddress"]
            };

            if (author == null)
            {
                return NotFound();
            }

            return View(author);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PubId,AuthorName,City,State,Country")] Author author)
        {
            if (!SetUpHttpClient())
            {
                return View("Unauthorized");
            }

            if (id != author.AuthorId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(author);
            }

            string content = JsonSerializer.Serialize(new
            {
                AuthorId = author.AuthorId,
                LastName = author.LastName,
                FirstName = author.FirstName,
                Phone = author.Phone,
                Address = author.Address,
                City = author.City,
                Zip = author.Zip,
                EmailAddress = author.EmailAddress
            });
            var data = new StringContent(content, Encoding.UTF8, "application/json");

            var response = await _client.PutAsync($"{AuthorApiUrl}({id})", data);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Error = await response.Content.ReadAsStringAsync();

            return View(author);
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

            var response = await _client.GetAsync($"{AuthorApiUrl}({id})");
            string strData = await response.Content.ReadAsStringAsync();

            var temp = JObject.Parse(strData);

            if (temp == null)
            {
                return View("NotFound");
            }

            var author = new Author
            {
                AuthorId = (int)temp["AuthorId"],
                LastName = (string)temp["LastName"],
                FirstName = (string)temp["FirstName"],
                Phone = (string)temp["Phone"],
                Address = (string)temp["Address"],
                City = (string)temp["City"],
                Zip = (int)temp["Zip"],
                EmailAddress = (string)temp["EmailAddress"]
            };

            if (author == null)
            {
                return NotFound();
            }

            return View(author);
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

            var response = await _client.DeleteAsync($"{AuthorApiUrl}({id})");

            if (!response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Delete), new { id = id });
            }

            ViewBag.Error = await response.Content.ReadAsStringAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
