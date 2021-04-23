using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SpotifyAPI.Web;
using SpotifyPoC.Models;

namespace SpotifyPoC.Controllers
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
            var loginRequest = new LoginRequest(
                new Uri("https://localhost:5001/Home/Callback"),
                "f4d565d844ca4379a69c93ce3e9df78e",
                LoginRequest.ResponseType.Code
            ) 
            {
                Scope = new[] { Scopes.PlaylistReadPrivate, Scopes.PlaylistReadCollaborative }
            };
            var uri = loginRequest.ToUri();

            return Redirect(uri.ToString());
        }

        public async Task<IActionResult> Callback(string code)
        {
            var response = await new OAuthClient().RequestToken(
                new AuthorizationCodeTokenRequest("f4d565d844ca4379a69c93ce3e9df78e", 
                                                "6c3ebfa872fc4b7db4c808cb03aff24c", code, 
                                                new Uri("https://localhost:5001/Home/Callback"))
            );
            var spotify = new SpotifyClient(response.AccessToken);
            var username = await spotify.UserProfile.Current();
            return Ok(username);
        }

        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}