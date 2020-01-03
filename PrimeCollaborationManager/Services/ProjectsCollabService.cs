using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using PrimeCollaborationManager.Models;
using Studio.Api.Client;
using Studio.Api.Model;
using Studio.Api.Model.Permissions;
using Studio.Api.Model.Users;

namespace PrimeCollaborationManager.Services
{
    public class ProjectsCollabService : ICollaborationService
    {
        static readonly string[] PermissionTypes = new string[] { "Invite", "ManageParticipants", "ManagePermissions", "UndoCheckouts", "CreateSessions", "ShareItems", "FullControl" };
        const int PageSize = 5;

        protected StudioClient _Client { get; set; }
        public ProjectsCollabService(StudioClient client)
        {
            _Client = client;
        }

        public async Task<CollaborationList> GetListAsync(int page = 1)
        {
            var projects = await _Client.GetProjectsList(PageSize, PageSize * (page - 1));
            var result = new CollaborationList()
            {
                TotalCollabs = projects.TotalCount,
                Collaborations = projects.Projects.Select(p => ConvertToCollab(p)).ToList(),
                CurrentPage = page,
                ItemsPerPage = PageSize,
                ShowStatus = false,
                ShowTimes = false
            };

            return result;
        }

        public async Task<Collaboration> GetDetailsAsync(string id)
        {
            return ConvertToCollab(await _Client.GetProjectDetails(id));
        }

        public List<string> GetPermissionTypes()
        {
            return PermissionTypes.ToList();
        }

        public Task SetPermissionsAsync(string id, string permission, bool? allow)
        {
            throw new NotImplementedException();
        }

        public Collaboration ConvertToCollab(Project project)
        {
            return new Collaboration()
            {
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
                            pList.Add(new Permission { Type = key, Allow = PermissionValue.Allow });
                        break;
                }
            }

            var projectId = await _Client.CreateProject(name, notification, restricted);

            var chosenPermTypes = pList.Select(s => s.Type).ToList();
            var missingTypes = GetPermissionTypes();
            missingTypes.RemoveAll(s => chosenPermTypes.Contains(s));
            foreach (var missingPerm in missingTypes)
            { 
                pList.Add(new Permission { Type = missingPerm, Allow = PermissionValue.Deny });
            }
            
            foreach (var perm in pList)
            {
                await _Client.UpdateProjectPermissions(projectId, perm);
            }

            return projectId;
        }

        public async Task<List<Permission>> GetPermissionsAsync(string id)
        {
            var perms = await _Client.GetProjectPermissions(id);
            return perms.ProjectPermissions;
        }

        public async Task<List<User>> GetUsersAsync(string id)
        {
            var response = await _Client.GetProjectUsers(id);
            return response.ProjectUsers;
        }
    }
}
