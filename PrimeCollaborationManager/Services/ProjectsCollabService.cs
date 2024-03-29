﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using PrimeCollaborationManager.Models;
using PrimeCollaborationManager.Models.Requests;
using Studio.Api.Client;
using Studio.Api.Model;
using Studio.Api.Model.Permissions;
using Studio.Api.Model.Users;

namespace PrimeCollaborationManager.Services
{
    public class ProjectsCollabService : ICollaborationService
    {
        static readonly string[] PermissionTypes = new string[] { "UndoCheckouts", "CreateSessions", "ShareItems", "Invite", "ManageParticipants", "ManagePermissions", "FullControl" };
        private int _PageSize;

        protected IStudioClient Client { get; set; }
        public ProjectsCollabService(IStudioClient client, int pageSize)
        {
            Client = client;
            _PageSize = pageSize;
        }

        public async Task<CollaborationList> GetListAsync(int page = 1)
        {
            var projects = await Client.GetProjectsList(_PageSize, _PageSize * (page - 1));
            var result = new CollaborationList()
            {
                Collaborations = new PagedResult<Collaboration>()
                {
                    Items = projects.Projects.Select(p => ConvertToCollab(p)).ToList(),
                    TotalItems = projects.TotalCount,
                    CurrentPage = page,
                    ItemsPerPage = _PageSize,
                },
                ShowStatus = false,
                ShowTimes = false,
                ShowCreate = false
            };

            return result;
        }

        public async Task<Collaboration> GetDetailsAsync(string id)
        {
            return ConvertToCollab(await Client.GetProjectDetails(id));
        }

        public List<string> GetPermissionTypes()
        {
            return PermissionTypes.ToList();
        }

        public async Task SetPermissionsAsync(UpdateCollabPermissionsRequest request)
        {
            if (request == null)
                return;

            var id = request.CollabId;

            var tasks = new List<Task>();

            if (Permission.AllowIsValid(request.UndoCheckouts))
                tasks.Add(Client.UpdateProjectPermissions(id, new Permission("UndoCheckouts", request.UndoCheckouts)));
            if (Permission.AllowIsValid(request.CreateSessions))
                tasks.Add(Client.UpdateProjectPermissions(id, new Permission("CreateSessions", request.CreateSessions)));
            if (Permission.AllowIsValid(request.ShareItems))
                tasks.Add(Client.UpdateProjectPermissions(id, new Permission("ShareItems", request.ShareItems)));

            if (Permission.AllowIsValid(request.Invite))
                tasks.Add(Client.UpdateProjectPermissions(id, new Permission("Invite", request.Invite)));
            if (Permission.AllowIsValid(request.ManageParticipants))
                tasks.Add(Client.UpdateProjectPermissions(id, new Permission("ManageParticipants", request.ManageParticipants)));
            if (Permission.AllowIsValid(request.ManagePermissions))
                tasks.Add(Client.UpdateProjectPermissions(id, new Permission("ManagePermissions", request.ManagePermissions)));

            if (Permission.AllowIsValid(request.FullControl))
                tasks.Add(Client.UpdateProjectPermissions(id, new Permission("FullControl", request.FullControl)));

            await Task.WhenAll(tasks);
        }

        public async Task UpdateCollaborationAccessAsync(string id, bool restrictAccess)
        {
            await Client.UpdateProjectAsync(id, null, restrictAccess, null, null);
        }

        public static Collaboration ConvertToCollab(Project project)
        {
            if(project == null)
                throw new ArgumentNullException(nameof(project));

            return new Collaboration()
            {
                Type = "Project",
                Id = project.Id,
                Name = project.Name,
                Created = project.Created,
                InviteUrl = project.InviteUrl,
                Owner = project.OwnerEmail,
                Restricted = project.Restricted,
                Status = "Active"
            };
        }

        public async Task<string> CreateAsync(IFormCollection form)
        {
            if (form == null)
                throw new ArgumentNullException(nameof(form));

            string name = "Default";
            bool restricted = false;
            bool notification = false;
            var pList = new List<Permission>();
            foreach (var key in form.Keys)
            {
                var formValIsBool = bool.TryParse(form[key], out bool formVal);
                switch (key)
                {
                    case "Name":
                        name = form[key];
                        break;
                    case "Restricted":
                        if (formValIsBool)
                            restricted = formVal;
                        else
                            throw new ArgumentException($"form input for key {key} is not a bool!:Val:{form[key]}");
                        break;
                    case "Notification":
                        if (formValIsBool)
                            notification = formVal;
                        else
                            throw new ArgumentException($"form input for key {key} is not a bool!:Val:{form[key]}");
                        break;
                    default:
                        if(formValIsBool)
                            pList.Add(new Permission(key, true));
                        break;
                }
            }

            var projectId = await Client.CreateProject(name, notification, restricted);

            var chosenPermTypes = pList.Select(s => s.Type).ToList();
            var missingTypes = GetPermissionTypes();
            missingTypes.RemoveAll(s => chosenPermTypes.Contains(s));
            foreach (var missingPerm in missingTypes)
            { 
                pList.Add(new Permission(missingPerm, false));
            }
            
            foreach (var perm in pList)
            {
                await Client.UpdateProjectPermissions(projectId, perm);
            }

            return projectId;
        }

        public async Task<List<Permission>> GetPermissionsAsync(string id)
        {
            var perms = await Client.GetProjectPermissions(id);
            var displayedPermsList = PermissionTypes.ToList();
            perms.ProjectPermissions = perms.ProjectPermissions.Where(p => displayedPermsList.Contains(p.Type)).ToList();
            return perms.ProjectPermissions;
        }

        public async Task<PagedResult<User>> GetUsersAsync(string id, int page = 1)
        {
            var response = await Client.GetProjectUsers(id, _PageSize, _PageSize * (page - 1));
            return new PagedResult<User>()
            {
                Items = response.ProjectUsers,
                TotalItems = response.TotalCount,
                CurrentPage = page,
                ItemsPerPage = _PageSize
            };
        }

        public async Task UpdateUserRestrictedStatusAsync(string id, int userId, string restrictedStatus)
        {
            await Client.UpdateProjectUserRestrictedStatus(id, userId, restrictedStatus);
        }

        public Task SetUserPermissionsAsync(string id, int userId, string permission, bool? allow)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<SessionActivityRecord>> GetActivity(string id, int page = 1)
        {
            throw new NotImplementedException();
        }
    }
}
