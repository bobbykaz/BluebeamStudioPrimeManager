using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PrimeCollaborationManager.Helpers;
using PrimeCollaborationManager.Models;
using PrimeCollaborationManager.Models.Requests;
using PrimeCollaborationManager.Services;
using Serilog;
using Serilog.Core;
using Studio.Api.Client;
using Studio.Api.Model.Logs;

namespace PrimeCollaborationManager.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        protected StudioApplicationConfig Config { get; set; }
        protected StudioClient Client { get; set; }
        protected UserLog UserLog { get; set; }
        protected ICollaborationService CollaborationService { get; set; }
        public ProjectsController(StudioApplicationConfig config)
        {
            Config = config;
        }

        protected virtual async Task InitClient()
        {
            UserLog = UserHelper.GetCurrentUser(HttpContext);
            Client = new StudioClient(Config, UserLog, Log.Logger);
            var token = await HttpContext.GetTokenAsync("access_token");
            Client.SetAuthHeader(token);
            CollaborationService = new ProjectsCollabService(Client, Config.ApiResultPageSize);
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            try
            {
                await InitClient();
                var projects = await CollaborationService.GetListAsync(page);
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
                var project = await CollaborationService.GetDetailsAsync(collabId);
                var model = new CollaborationDetails { Collab = project };
                return View(model);
            }
            catch (StudioApiException e)
            {
                return HandleError(e);
            }
        }

        #region permissions
        [HttpPost]
        public async Task<IActionResult> UpdateAccess(string collabId, bool newRestrictedStatus)
        {
            try
            {
                await InitClient();
                await CollaborationService.UpdateCollaborationAccessAsync(collabId, newRestrictedStatus);

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
                var detail =  CollaborationService.GetDetailsAsync(collabId);
                var perms =  CollaborationService.GetPermissionsAsync(collabId);
                await Task.WhenAll(detail, perms);
                var allPerms = CollaborationService.GetPermissionTypes();
                var foundTypes = perms.Result.Select(p => p.Type).ToList();
                foreach (var perm in allPerms)
                {
                    if (!foundTypes.Contains(perm))
                        perms.Result.Add(new Studio.Api.Model.Permissions.Permission(perm, (bool?)null));
                }
                var model = new CollaborationDetails { Collab = detail.Result, Permissions = perms.Result };
                return View(model);
            }
            catch (StudioApiException e)
            {
                return HandleError(e);
            }
        }

        public async Task<IActionResult> UpdatePermissions([Bind] UpdateCollabPermissionsRequest request)
        {
            try
            {
                if (request == null)
                    return BadRequest();

                await InitClient();
                await CollaborationService.SetPermissionsAsync(request);
                return RedirectToAction("PermissionDetails", new Dictionary<string, string> { { "collabId", request.CollabId } });
            }
            catch (StudioApiException e)
            {
                return HandleError(e);
            }
        }
        #endregion

        #region Users
        public async Task<IActionResult> UserList(string collabId, int page = 1)
        {
            try
            {
                await InitClient();
                var detail = CollaborationService.GetDetailsAsync(collabId);
                var users = CollaborationService.GetUsersAsync(collabId, page);
                await Task.WhenAll(detail, users);
                var model = new CollaborationDetails { Collab = detail.Result, Users = users.Result };
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
                await CollaborationService.UpdateUserRestrictedStatusAsync(collabId, user, allow ? "Allow" : "Deny");
                return RedirectToAction("UserList", new Dictionary<string, string> { { "collabId", collabId }, { "page", $"{page}" } });
            }
            catch (StudioApiException e)
            {
                return HandleError(e);
            }
        }
        #endregion

        #region CreateProject
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                await InitClient();
                var model = new CreateCollaboration()
                {
                    Restricted = true,
                    InitialPermissionTypes = CollaborationService.GetPermissionTypes()
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
                var id = await CollaborationService.CreateAsync(form);
                return RedirectToAction("Details", new { collabId = id });
            }
            catch (StudioApiException e)
            {
                return HandleError(e);
            }
        }
        #endregion

        private IActionResult HandleError(StudioApiException e)
        {
            Log.Logger.Error("Error encountered; User: {@User}; Exception: {Exception}", UserLog, e);
            return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, StudioError = e });
        }
    }
}