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
    public class BooksController : Controller
    {
        private HttpClient _client;
        private readonly string BookApiUrl = "https://localhost:44303/odata/books";
        private readonly string PublisherApiUrl = "https://localhost:44303/odata/publishers";

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

        public async Task<IActionResult> Index(string searchValue)
        {
            if (!SetUpHttpClient())
            {
                return RedirectToAction("Index", "Unauthorized");
            }


            HttpResponseMessage respone = new();
            List<Book> items = new();
            if (!string.IsNullOrWhiteSpace(searchValue))
            {
                respone = double.TryParse(searchValue, out double value)
                    ? await _client.GetAsync($"{BookApiUrl}?$filter=Price eq {value}")
                    : await _client.GetAsync($"{BookApiUrl}?$filter=contains(Title, '{searchValue}'");
            }
            else
            {
                respone = await _client.GetAsync($"{BookApiUrl}");
            }

            string strData = await respone.Content.ReadAsStringAsync();

            dynamic temp = JObject.Parse(strData);
            if (((JArray)temp.value) != null)
            {
                items = ((JArray)temp.value).Select(x => new Book
                {
                    BookId = (int)x["BookId"],
                    Title = (string)x["Title"],
                    Type = (string)x["Type"],
                    PubId = (int)x["PubId"],
                    Price = (double)x["Price"],
                    Advance = (double)x["Advance"],
                    Royalty = (double)x["Royalty"],
                    YtdSales = (double)x["YtdSales"],
                    Notes = (string)x["Notes"],
                    PublishedDate = DateTime.Parse(x["PublishedDate"].ToString()).Date
                }).ToList();
            }
            return View(items);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (!SetUpHttpClient())
            {
                return RedirectToAction("Index", "Unauthorized");
            }
            var response = await _client.GetAsync($"{BookApiUrl}({id})");
            string strData = await response.Content.ReadAsStringAsync();

            var temp = JObject.Parse(strData);

            if (temp == null)
            {
                return View("NotFound");
            }
            var book = new Book
            {
                BookId = (int)temp["BookId"],
                Title = (string)temp["Title"],
                Type = (string)temp["Type"],
                PubId = (int)temp["PubId"],
                Price = (double)temp["Price"],
                Advance = (double)temp["Advance"],
                Royalty = (double)temp["Royalty"],
                YtdSales = (double)temp["YtdSales"],
                Notes = (string)temp["Notes"],
                PublishedDate = DateTime.Parse(temp["PublishedDate"].ToString()).Date
            };

            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        public async Task<IActionResult> Create()
        {
            if (!SetUpHttpClient())
            {
                return RedirectToAction("Index", "Unauthorized");
            }

            List<Publisher> publishers = new();
            var publishersResponse = await _client.GetAsync($"{PublisherApiUrl}?$Select=PubId,PublisherName");

            string publishersStrData = await publishersResponse.Content.ReadAsStringAsync();
            dynamic publishersTemp = JObject.Parse(publishersStrData);
            if ((JArray)publishersTemp.value != null)
            {
                publishers = ((JArray)publishersTemp.value).Select(x => new Publisher
                {
                    PubId = (int)x["PubId"],
                    PublisherName = (string)x["PublisherName"]
                }).ToList();
            }

            ViewData["PubId"] = new SelectList(publishers, "PubId", "PublisherName");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Type,PubId,Price,Advance,Royalty,YtdSales,Notes,PublishedDate")] Book book)
        {
            if (!SetUpHttpClient())
            {
                return RedirectToAction("Index", "Unauthorized");
            }
            if (ModelState.IsValid)
            {
                string content = JsonSerializer.Serialize(new
                {
                    Title = book.Title,
                    Type = book.Type,
                    PubId = book.PubId,
                    Price = book.Price,
                    Advance = book.Advance,
                    Royalty = book.Royalty,
                    YtdSales = book.YtdSales,
                    Notes = book.Notes,
                    PublishedDate = book.PublishedDate.Value.ToString("yyyy-MM-dd")
                });
                var data = new StringContent(content, Encoding.UTF8, "application/json");

                var response = await _client.PostAsync($"{BookApiUrl}", data);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.Error = await response.Content.ReadAsStringAsync();

                List<Publisher> publishers = new();
                var publishersResponse = await _client.GetAsync($"{PublisherApiUrl}?$Select=PubId,PublisherName");

                string publishersStrData = await publishersResponse.Content.ReadAsStringAsync();
                dynamic publishersTemp = JObject.Parse(publishersStrData);
                if ((JArray)publishersTemp.value != null)
                {
                    publishers = ((JArray)publishersTemp.value).Select(x => new Publisher
                    {
                        PubId = (int)x["PubId"],
                        PublisherName = (string)x["PublisherName"]
                    }).ToList();
                }

                ViewData["PubId"] = new SelectList(publishers, "PubId", "PublisherName");
            }

            return View(book);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (!SetUpHttpClient())
            {
                return RedirectToAction("Index", "Unauthorized");
            }
            if (id == null)
            {
                return NotFound();
            }

            var response = await _client.GetAsync($"{BookApiUrl}({id})");
            string strData = await response.Content.ReadAsStringAsync();

            var temp = JObject.Parse(strData);

            if (temp == null)
            {
                return View("NotFound");
            }

            var book = new Book
            {
                BookId = (int)temp["BookId"],
                Title = (string)temp["Title"],
                Type = (string)temp["Type"],
                PubId = (int)temp["PubId"],
                Price = (double)temp["Price"],
                Advance = (double)temp["Advance"],
                Royalty = (double)temp["Royalty"],
                YtdSales = (double)temp["YtdSales"],
                Notes = (string)temp["Notes"],
                PublishedDate = DateTime.Parse(temp["PublishedDate"].ToString())
            };

            var publishersResponse = await _client.GetAsync($"{PublisherApiUrl}?$Select=PubId,PublisherName");

            string publishersStrData = await publishersResponse.Content.ReadAsStringAsync();
            dynamic publishersTemp = JObject.Parse(publishersStrData);
            var publishers = ((JArray)publishersTemp.value).Select(x => new Publisher
            {
                PubId = (int)x["PubId"],
                PublisherName = (string)x["PublisherName"]
            }).ToList();
            ViewData["PubId"] = new SelectList(publishers, "PubId", "PublisherName");

            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookId,Title,Type,PubId,Price,Advance,Royalty,YtdSales,Notes,PublishedDate")] Book book)
        {
            if (!SetUpHttpClient())
            {
                return RedirectToAction("Index", "Unauthorized");
            }
            if (id != book.BookId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                string content = JsonSerializer.Serialize(new
                {
                    BookId = book.BookId,
                    Title = book.Title,
                    Type = book.Type,
                    PubId = book.PubId,
                    Price = book.Price,
                    Advance = book.Advance,
                    Royalty = book.Royalty,
                    YtdSales = book.YtdSales,
                    Notes = book.Notes,
                    PublishedDate = book.PublishedDate.Value.ToString("yyyy-MM-dd")
                });
                var data = new StringContent(content, Encoding.UTF8, "application/json");

                var response = await _client.PutAsync($"{BookApiUrl}({id})", data);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.Error = await response.Content.ReadAsStringAsync();
            }

            var publishersResponse = await _client.GetAsync($"{PublisherApiUrl}?$Select=PubId,PublisherName");

            string publishersStrData = await publishersResponse.Content.ReadAsStringAsync();
            dynamic publishersTemp = JObject.Parse(publishersStrData);
            var publishers = ((JArray)publishersTemp.value).Select(x => new Publisher
            {
                PubId = (int)x["PubId"],
                PublisherName = (string)x["PublisherName"]
            }).ToList();
            ViewData["PubId"] = new SelectList(publishers, "PubId", "PublisherName");

            return View(book);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (!SetUpHttpClient())
            {
                return RedirectToAction("Index", "Unauthorized");
            }
            if (id == null)
            {
                return NotFound();
            }

            var response = await _client.GetAsync($"{BookApiUrl}({id})");
            string strData = await response.Content.ReadAsStringAsync();

            var temp = JObject.Parse(strData);

            if (temp == null)
            {
                return View("NotFound");
            }

            var book = new Book
            {
                BookId = (int)temp["BookId"],
                Title = (string)temp["Title"],
                Type = (string)temp["Type"],
                PubId = (int)temp["PubId"],
                Price = (double)temp["Price"],
                Advance = (double)temp["Advance"],
                Royalty = (double)temp["Royalty"],
                YtdSales = (double)temp["YtdSales"],
                Notes = (string)temp["Notes"],
                PublishedDate = DateTime.Parse(temp["PublishedDate"].ToString()).Date
            };

            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (!SetUpHttpClient())
            {
                return RedirectToAction("Index", "Unauthorized");
            }
            if (id == null)
            {
                return NotFound();
            }

            var response = await _client.DeleteAsync($"{BookApiUrl}({id})");

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = await response.Content.ReadAsStringAsync();
                return RedirectToAction(nameof(Delete), new { id = id });
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
