using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Studio.Api.Client;

namespace PrimeCollaborationManager.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        protected StudioClient _Client { get; set; }
        public ProjectsController(StudioApplicationConfig config)
        {
            _Client = new StudioClient(config);
        }

        protected async Task InitClient()
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            _Client.SetAuthHeader(token);
        }

        public async Task<IActionResult> Index()
        {
            await InitClient();
            var projects = await _Client.GetProjectsList();
            return View(projects.Projects);
        }

        public async Task<IActionResult> Details(string projectId)
        {
            await InitClient();
            var projects = await _Client.GetProjectDetails(projectId);
            return View(projects);
        }

        public async Task<IActionResult> Create()
        {
            await InitClient();
            var projects = await _Client.GetProjectDetails("");
            return View(projects);
        }
    }
}