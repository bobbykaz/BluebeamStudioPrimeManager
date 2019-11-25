using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using PrimeCollaborationManager.Models;
using Studio.Api.Client;
using Studio.Api.Model;

namespace PrimeCollaborationManager.Services
{
    public class ProjectsCollabService : ICollaborationService
    {
        static readonly string[] PermissionTypes = new string[] { "Invite", "ManageParticipants", "ManagePermissions", "UndoCheckouts", "CreateSessions", "ShareItems", "FullControl" };
        protected StudioClient _Client { get; set; }
        public ProjectsCollabService(StudioClient client)
        {
            _Client = client;
        }

        public Task<Collaboration> CreateCollabAsync(string id, string name, bool restrictUsers)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Collaboration>> GetCollabListAsync()
        {
            var projects = await _Client.GetProjectsList();
            return projects.Projects.Select(p => ConvertToCollab(p)).ToList();
        }

        public async Task<Collaboration> GetCollabDetailsAsync(string id)
        {
            return ConvertToCollab(await _Client.GetProjectDetails(id));
        }

        public List<string> GetCollabPermissionTypes()
        {
            return PermissionTypes.ToList();
        }

        public Task SetCollabPermissions(string id, string permission, bool? allow)
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

        public async Task<string> CreateCollabAsync(IFormCollection form)
        {
            string name = "Default";
            bool restricted = false;
            bool notification = false;
            PermissionsSettingsList pList = new PermissionsSettingsList()
            {
                Permissions = new List<PermissionSetting>()
            };
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
                            pList.Permissions.Add(new PermissionSetting { Type = key, Allow = formVal });
                        break;
                }
            }

            var projectId = await _Client.CreateProject(name, notification, restricted);

            //permissions?

            return projectId;
        }
    }
}
