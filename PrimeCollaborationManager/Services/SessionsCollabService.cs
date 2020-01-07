using Microsoft.AspNetCore.Http;
using PrimeCollaborationManager.Models;
using Studio.Api.Client;
using Studio.Api.Model.Permissions;
using Studio.Api.Model.Sessions;
using Studio.Api.Model.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrimeCollaborationManager.Services
{
    public class SessionsCollabService : ICollaborationService
    {
        static readonly string[] PermissionTypes = new string[] { "SaveCopy", "PrintCopy", "Markup", "AddDocuments", "MarkupAlert", "FullControl" };
        const int PageSize = 5;

        protected StudioClient _Client { get; set; }
        public SessionsCollabService(StudioClient client)
        {
            _Client = client;
        }

        public async Task<CollaborationList> GetListAsync(int page = 1)
        {
            var sessions = await _Client.GetSessionsList(PageSize, PageSize * (page - 1));
            var result = new CollaborationList()
            {
                Collaborations = new PagedResult<Collaboration>()
                {
                    Items = sessions.Sessions.Select(p => ConvertToCollab(p)).ToList(),
                    TotalItems = sessions.TotalCount,
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                },
                ShowStatus = true,
                ShowTimes = false,
                ShowCreate = false
            };

            return result;
        }

        public async Task<Collaboration> GetDetailsAsync(string id)
        {
            return ConvertToCollab(await _Client.GetSessionDetails(id));
        }

        public List<string> GetPermissionTypes()
        {
            return PermissionTypes.ToList();
        }

        public Task SetPermissionsAsync(string id, string permission, bool? allow)
        {
            throw new NotImplementedException();
        }

        public Collaboration ConvertToCollab(Session session)
        {
            return new Collaboration()
            {
                Type = "Session",
                Id = session.Id,
                Name = session.Name,
                Created = session.Created,
                InviteUrl = session.InviteUrl,
                Owner = session.OwnerEmail,
                Restricted = session.Restricted,
                Status = "Active", 
                EndDate = session.SessionEndDate
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
                        if (formValIsBool)
                            pList.Add(new Permission { Type = key, Allow = PermissionValue.Allow });
                        break;
                }
            }

            var id = await _Client.CreateSession(name, notification, restricted);

            var chosenPermTypes = pList.Select(s => s.Type).ToList();
            var missingTypes = GetPermissionTypes();
            missingTypes.RemoveAll(s => chosenPermTypes.Contains(s));
            foreach (var missingPerm in missingTypes)
            {
                pList.Add(new Permission { Type = missingPerm, Allow = PermissionValue.Deny });
            }

            foreach (var perm in pList)
            {
                await _Client.UpdateSessionPermissions(id, perm);
            }

            return id;
        }

        public async Task<List<Permission>> GetPermissionsAsync(string id)
        {
            var perms = await _Client.GetSessionPermissions(id);
            return perms.SessionPermissions;
        }

        public async Task<PagedResult<User>> GetUsersAsync(string id, int page = 1)
        {
            var response = await _Client.GetSessionUsers(id, PageSize, PageSize * (page - 1));
            return new PagedResult<User>()
            {
                Items = response.SessionUsers,
                TotalItems = response.TotalCount,
                CurrentPage = page,
                ItemsPerPage = PageSize
            };
        }

        public Task UpdateCollaborationAccessAsync(string id, bool restrictAccess)
        {
            throw new NotImplementedException();
        }
    }
}
