using ClientMVCApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
namespace ClientMVCApplication.Controllers
{
    public class TraineesController : Controller
    {
        private readonly HttpClient _client;

        public TraineesController(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient();
            _client.BaseAddress = new Uri("http://localhost:5123/api/");
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var trainees = await GetTrainees();
            return View(trainees);
        }


        private async Task<List<Trainee>> GetTrainees()
        {
            var BearerToken = HttpContext.Session.GetString("JWToken");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", BearerToken);

            var response = await _client.GetAsync("trainees");
            response.EnsureSuccessStatusCode();

            var jsonStr = await response.Content.ReadAsStringAsync();
            var trainees = JsonConvert.DeserializeObject<List<Trainee>>(jsonStr);

            return trainees;
        }

        [HttpGet]
        public IActionResult PostTrainee()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PostTrainee(Trainee trainee)
        {
            try
            {
                string data = JsonConvert.SerializeObject(trainee);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                var response = await _client.PostAsync("trainees", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Trainee created";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["errorMessage"] = "Failed to create trainee";
                    return View();
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> PutTrainee(int id)
        {
            var trainee = await GetTraineeById(id);

            if (trainee == null)
            {
                return NotFound();
            }

            return View(trainee);
        }

        [HttpPost]
        public async Task<IActionResult> PutTrainee(int id, Trainee trainee)
        {
            try
            {
                string data = JsonConvert.SerializeObject(trainee);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                var response = await _client.PutAsync($"trainees/{id}", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Trainee updated";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["errorMessage"] = "Failed to update trainee";
                    return View(trainee);
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View(trainee);
            }


        }
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var trainee = await GetTraineeById(id);

            if (trainee == null)
            {
                return NotFound();
            }

            return View(trainee);
        }
        [HttpGet]
        public async Task<IActionResult> DeleteTrainee(int id)
        {
            var trainee = await GetTraineeById(id);

            if (trainee == null)
            {
                return NotFound();
            }

            return View(trainee);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmDeleteTrainee(int id)
        {
            try
            {
                var response = await _client.DeleteAsync($"trainees/{id}");

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Trainee deleted";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["errorMessage"] = "Failed to delete trainee";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        private async Task<Trainee> GetTraineeById(int id)
        {
            try
            {
                var response = await _client.GetAsync($"trainees/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var jsonStr = await response.Content.ReadAsStringAsync();
                    var trainee = JsonConvert.DeserializeObject<Trainee>(jsonStr);

                    return trainee;
                }
                else
                {
                    TempData["errorMessage"] = "Failed to retrieve trainee details";
                    return null; // Or handle the error as needed
                }
            }
            catch (HttpRequestException ex)
            {
                // Handle the exception, e.g., log the error.
                TempData["errorMessage"] = $"Error retrieving Trainee: {ex.Message}";
                return null; // Or handle the error as needed
            }
        }


    }
}
