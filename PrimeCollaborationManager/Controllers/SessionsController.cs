using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PrimeCollaborationManager.Models;
using PrimeCollaborationManager.Services;
using Studio.Api.Client;

namespace PrimeCollaborationManager.Controllers
{
    public class SessionsController : Controller
    {
        protected StudioClient _Client { get; set; }
        protected ICollaborationService _SessionsService { get; set; }
        public SessionsController(StudioApplicationConfig config)
        {
            _Client = new StudioClient(config);
        }

        protected async Task InitClient()
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            _Client.SetAuthHeader(token);
            _SessionsService = new SessionsCollabService(_Client);
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            await InitClient();
            var collabs = await _SessionsService.GetListAsync(page);
            return View(collabs);
        }

        public async Task<IActionResult> Details(string collabId)
        {
            await InitClient();
            var collab = await _SessionsService.GetDetailsAsync(collabId);
            var model = new CollaborationDetails { Collab = collab };
            return View(model);
        }

        public async Task<IActionResult> PermissionDetails(string collabId)
        {
            await InitClient();
            var detail = await _SessionsService.GetDetailsAsync(collabId);
            var perms = await _SessionsService.GetPermissionsAsync(collabId);
            var model = new CollaborationDetails { Collab = detail, Permissions = perms };
            return View(model);
        }

        public async Task<IActionResult> UserList(string collabId, int page = 1)
        {
            await InitClient();
            var detail = await _SessionsService.GetDetailsAsync(collabId);
            var users = await _SessionsService.GetUsersAsync(collabId, page);
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
                InitialPermissionTypes = _SessionsService.GetPermissionTypes()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSubmit(IFormCollection form)
        {
            await InitClient();
            var id = await _SessionsService.CreateAsync(form);
            return RedirectToAction("Details", new { collabId = id });
        }
    }
}