using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PrimeCollaborationManager.Models;
using Studio.Api.Client;

namespace PrimeCollaborationManager.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly StudioApplicationConfig _Config;

        public HomeController(ILogger<HomeController> logger, StudioApplicationConfig config)
        {
            _logger = logger;
            _Config = config;
        }

        public IActionResult Index()
        {
            return View(model: _Config.ClientId);
        }

        public IActionResult Help()
        {
            return View(model: _Config.ClientId); ;
        }

        public IActionResult Features()
        {
            return View();
        }

        public IActionResult Denied()
        {
            return View();
        }

        [HttpPost]
        public IActionResult UserLogin(string returnUrl = "/")
        {
            return Challenge(new AuthenticationProperties() { RedirectUri = returnUrl });
        }

        [HttpPost]
        public async Task<IActionResult> UserLogout()
        {
            await this.HttpContext.SignOutAsync();
            return Redirect("/");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
