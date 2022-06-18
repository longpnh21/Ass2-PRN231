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
    public class PublishersController : Controller
    {
        private readonly string PublisherApiUrl = "https://localhost:44303/odata/publishers";
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

            var respone = await _client.GetAsync($"{PublisherApiUrl}");
            string strData = await respone.Content.ReadAsStringAsync();

            List<Publisher> items = new();

            dynamic temp = JObject.Parse(strData);

            if ((JArray)temp.value == null)
            {
                return View("NotFound");
            }

            items = ((JArray)temp.value).Select(x => new Publisher
            {
                PubId = (int)x["PubId"],
                PublisherName = (string)x["PublisherName"],
                City = (string)x["City"],
                State = (string)x["State"],
                Country = (string)x["Country"]
            }).ToList();
            return View(items);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (!SetUpHttpClient())
            {
                return View("Unauthorized");
            }

            var response = await _client.GetAsync($"{PublisherApiUrl}({id})");
            string strData = await response.Content.ReadAsStringAsync();

            var temp = JObject.Parse(strData);

            if (temp == null)
            {
                return View("NotFound");
            }

            var publisher = new Publisher
            {
                PubId = (int)temp["PubId"],
                PublisherName = (string)temp["PublisherName"],
                City = (string)temp["City"],
                State = (string)temp["State"],
                Country = (string)temp["Country"]
            };

            if (publisher == null)
            {
                return NotFound();
            }

            return View(publisher);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PublisherName,City,State,Country")] Publisher publisher)
        {
            if (!SetUpHttpClient())
            {
                return View("Unauthorized");
            }

            if (ModelState.IsValid)
            {
                string content = JsonSerializer.Serialize(new
                {
                    PublisherName = publisher.PublisherName,
                    City = publisher.City,
                    State = publisher.State,
                    Country = publisher.Country
                });
                var data = new StringContent(content, Encoding.UTF8, "application/json");

                var response = await _client.PostAsync($"{PublisherApiUrl}", data);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.Error = await response.Content.ReadAsStringAsync();
            }

            return View(publisher);
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

            var response = await _client.GetAsync($"{PublisherApiUrl}({id})");
            string strData = await response.Content.ReadAsStringAsync();

            var temp = JObject.Parse(strData);

            if (temp == null)
            {
                return View("NotFound");
            }

            var publisher = new Publisher
            {
                PubId = (int)temp["PubId"],
                PublisherName = (string)temp["PublisherName"],
                City = (string)temp["City"],
                State = (string)temp["State"],
                Country = (string)temp["Country"]
            };

            if (publisher == null)
            {
                return NotFound();
            }

            return View(publisher);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PubId,PublisherName,City,State,Country")] Publisher publisher)
        {
            if (!SetUpHttpClient())
            {
                return View("Unauthorized");
            }

            if (id != publisher.PubId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                string content = JsonSerializer.Serialize(new
                {
                    PubId = publisher.PubId,
                    PublisherName = publisher.PublisherName,
                    City = publisher.City,
                    State = publisher.State,
                    Country = publisher.Country
                });
                var data = new StringContent(content, Encoding.UTF8, "application/json");

                var response = await _client.PutAsync($"{PublisherApiUrl}({id})", data);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.Error = await response.Content.ReadAsStringAsync();

            }

            return View(publisher);
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

            var response = await _client.GetAsync($"{PublisherApiUrl}({id})");
            string strData = await response.Content.ReadAsStringAsync();

            var temp = JObject.Parse(strData);

            if (temp == null)
            {
                return View("NotFound");
            }

            var publisher = new Publisher
            {
                PubId = (int)temp["PubId"],
                PublisherName = (string)temp["PublisherName"],
                City = (string)temp["City"],
                State = (string)temp["State"],
                Country = (string)temp["Country"]
            };

            if (publisher == null)
            {
                return NotFound();
            }

            return View(publisher);
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

            var response = await _client.DeleteAsync($"{PublisherApiUrl}({id})");

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = await response.Content.ReadAsStringAsync();
                return RedirectToAction(nameof(Delete), new { id = id });
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
