using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PrimeCollaborationManager.Models;
using PrimeCollaborationManager.Services;
using Studio.Api.Client;

namespace PrimeCollaborationManager.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        protected StudioClient _Client { get; set; }
        protected ICollaborationService _ProjectsService { get; set; }
        public ProjectsController(StudioApplicationConfig config)
        {
            _Client = new StudioClient(config);
        }

        protected async Task InitClient()
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            _Client.SetAuthHeader(token);
            _ProjectsService = new ProjectsCollabService(_Client);
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            await InitClient();
            var projects = await _ProjectsService.GetListAsync(page);
            return View(projects);
        }

        public async Task<IActionResult> Details(string collabId)
        {
            await InitClient();
            var project = await _ProjectsService.GetDetailsAsync(collabId);
            var model = new CollaborationDetails { Collab = project };
            return View(model);
        }

        public async Task<IActionResult> PermissionDetails(string collabId)
        {
            await InitClient();
            var detail = await _ProjectsService.GetDetailsAsync(collabId);
            var perms = await _ProjectsService.GetPermissionsAsync(collabId);
            var model = new CollaborationDetails { Collab = detail, Permissions = perms };
            return View(model);
        }

        public async Task<IActionResult> UserDetails(string collabId)
        {
            await InitClient();
            var detail = await _ProjectsService.GetDetailsAsync(collabId);
            var users = await _ProjectsService.GetUsersAsync(collabId);
            var model = new CollaborationDetails { Collab = detail, Users = users };
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await InitClient();
            var model = new CreateCollaboration()
            {
                Restricted = true,
                InitialPermissionTypes = _ProjectsService.GetPermissionTypes()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSubmit(IFormCollection form)
        {
            await InitClient();
            var id = await _ProjectsService.CreateAsync(form);
            return RedirectToAction("Details", new { collabId = id });
        }
    }
}