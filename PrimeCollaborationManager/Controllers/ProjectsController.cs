using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        protected ICollaborationService _CollaborationService { get; set; }
        public ProjectsController(StudioApplicationConfig config)
        {
            _Client = new StudioClient(config);
        }

        protected virtual async Task InitClient()
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            _Client.SetAuthHeader(token);
            _CollaborationService = new ProjectsCollabService(_Client);
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            try
            {
                await InitClient();
                var projects = await _CollaborationService.GetListAsync(page);
                return View(projects);
            }
            catch (StudioApiException e)
            {
                return HandleError(e);
            }
        }

        public async Task<IActionResult> Details(string collabId)
        {
            try
            {
                await InitClient();
                var project = await _CollaborationService.GetDetailsAsync(collabId);
                var model = new CollaborationDetails { Collab = project };
                return View(model);
            }
            catch (StudioApiException e)
            {
                return HandleError(e);
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAccess(string collabId, bool newRestrictedStatus)
        {
            try
            {
                await InitClient();
                await _CollaborationService.UpdateCollaborationAccessAsync(collabId, newRestrictedStatus);

                return RedirectToAction("Details", new Dictionary<string, string> { { "collabId", collabId } });
            }
            catch (StudioApiException e)
            {
                return HandleError(e);
            }
        }

        public async Task<IActionResult> PermissionDetails(string collabId)
        {
            try
            {
                await InitClient();
                var detail = await _CollaborationService.GetDetailsAsync(collabId);
                var perms = await _CollaborationService.GetPermissionsAsync(collabId);
                var allPerms = _CollaborationService.GetPermissionTypes();
                var foundTypes = perms.Select(p => p.Type).ToList();
                foreach (var perm in allPerms)
                {
                    if (!foundTypes.Contains(perm))
                        perms.Add(new Studio.Api.Model.Permissions.Permission { Type = perm, Allow = "Default" });
                }
                var model = new CollaborationDetails { Collab = detail, Permissions = perms };
                return View(model);
            }
            catch (StudioApiException e)
            {
                return HandleError(e);
            }
        }

        public async Task<IActionResult> UserList(string collabId, int page = 1)
        {
            try
            {
                await InitClient();
                var detail = await _CollaborationService.GetDetailsAsync(collabId);
                var users = await _CollaborationService.GetUsersAsync(collabId, page);
                var model = new CollaborationDetails { Collab = detail, Users = users };
                return View(model);
            }
            catch (StudioApiException e)
            {
                return HandleError(e);
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUserPermission(string collabId, int page, int user, bool allow)
        {
            try
            {
                await InitClient();
                await _CollaborationService.UpdateUserRestrictedStatusAsync(collabId, user, allow ? "Allow" : "Deny");
                return RedirectToAction("UserList", new Dictionary<string, string> { { "collabId", collabId }, { "page", page.ToString() } });
            }
            catch (StudioApiException e)
            {
                return HandleError(e);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                await InitClient();
                var model = new CreateCollaboration()
                {
                    Restricted = true,
                    InitialPermissionTypes = _CollaborationService.GetPermissionTypes()
                };
                return View(model);
            }
            catch (StudioApiException e)
            {
                return HandleError(e);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateSubmit(IFormCollection form)
        {
            try
            {
                await InitClient();
                var id = await _CollaborationService.CreateAsync(form);
                return RedirectToAction("Details", new { collabId = id });
            }
            catch (StudioApiException e)
            {
                return HandleError(e);
            }
        }

        private IActionResult HandleError(StudioApiException e)
        {
            return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, StudioError = e });
        }
    }
}