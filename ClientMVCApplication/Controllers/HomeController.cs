

using ClientMVCApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;

namespace ClientMVCApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public async Task<IActionResult> LoginUser(Trainee trainee)
        {
            HttpClientHandler clientHandler = new HttpClientHandler(); clientHandler.ServerCertificateCustomValidationCallback
            = (sender, cert, chain, sslPolicyErrors) => { return true; };
            using (var httpClient = new HttpClient(clientHandler))
            {
                StringContent stringContent

            = new StringContent(JsonConvert.SerializeObject(trainee), Encoding.UTF8, "application/json");
                using (var response = await httpClient.PostAsync("http://localhost:5123/api/Token", stringContent))
                {
                    string token = await response.Content.ReadAsStringAsync();
                    if (token == "Invalid credentials")
                    {
                        ViewBag.Message = "Incorrect Email and Password";
                        return Redirect("~/Home/Index");

                    }
                    HttpContext.Session.SetString("JWToken", token);


                    return Redirect("~/Trainees/Index");


                }
            }
        }
}
}