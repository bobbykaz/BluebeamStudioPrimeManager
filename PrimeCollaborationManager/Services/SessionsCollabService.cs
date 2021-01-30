using Microsoft.AspNetCore.Http;
using PrimeCollaborationManager.Models;
using PrimeCollaborationManager.Models.Requests;
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
        private int _PageSize;

        protected IStudioClient Client { get; set; }
        public SessionsCollabService(IStudioClient client, int pageSize)
        {
            Client = client;
            _PageSize = pageSize;
        }

        public async Task<CollaborationList> GetListAsync(int page = 1)
        {
            var sessions = await Client.GetSessionsList(_PageSize, _PageSize * (page - 1));
            var result = new CollaborationList()
            {
                Collaborations = new PagedResult<Collaboration>()
                {
                    Items = sessions.Sessions.Select(p => ConvertToCollab(p)).ToList(),
                    TotalItems = sessions.TotalCount,
                    CurrentPage = page,
                    ItemsPerPage = _PageSize,
                },
                ShowStatus = true,
                ShowTimes = false,
                ShowCreate = false
            };

            return result;
        }

        public async Task<Collaboration> GetDetailsAsync(string id)
        {
            return ConvertToCollab(await Client.GetSessionDetails(id));
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

            if (Permission.AllowIsValid(request.SaveCopy))
                tasks.Add(Client.UpdateSessionPermissions(id, new Permission("SaveCopy", request.SaveCopy)));
            if (Permission.AllowIsValid(request.PrintCopy))
                tasks.Add(Client.UpdateSessionPermissions(id, new Permission("PrintCopy", request.PrintCopy)));
            
            if (Permission.AllowIsValid(request.Markup))
                tasks.Add(Client.UpdateSessionPermissions(id, new Permission("Markup", request.Markup)));
            if (Permission.AllowIsValid(request.AddDocuments))
                tasks.Add(Client.UpdateSessionPermissions(id, new Permission("AddDocuments", request.AddDocuments)));
            if (Permission.AllowIsValid(request.MarkupAlert))
                tasks.Add(Client.UpdateSessionPermissions(id, new Permission("MarkupAlert", request.MarkupAlert)));

            if (Permission.AllowIsValid(request.FullControl))
                tasks.Add(Client.UpdateSessionPermissions(id, new Permission("FullControl", request.FullControl)));

            await Task.WhenAll(tasks);
        }

        public static Collaboration ConvertToCollab(Session session)
        {
            if (session == null)
                throw new ArgumentNullException(nameof(session));

            return new Collaboration()
            {
                Type = "Session",
                Id = session.Id,
                Name = session.Name,
                Created = session.Created,
                InviteUrl = session.InviteUrl,
                Owner = session.OwnerEmail,
                Restricted = session.Restricted,
                Status = session.Status, 
                EndDate = session.SessionEndDate
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
                        if (formValIsBool)
                            pList.Add(new Permission(key, true));
                        break;
                }
            }

            var id = await Client.CreateSession(name, notification, restricted);

            var chosenPermTypes = pList.Select(s => s.Type).ToList();
            var missingTypes = GetPermissionTypes();
            missingTypes.RemoveAll(s => chosenPermTypes.Contains(s));
            foreach (var missingPerm in missingTypes)
            {
                pList.Add(new Permission(missingPerm, false));
            }

            foreach (var perm in pList)
            {
                await Client.UpdateSessionPermissions(id, perm);
            }

            return id;
        }

        public async Task<List<Permission>> GetPermissionsAsync(string id)
        {
            var perms = await Client.GetSessionPermissions(id);
            return perms.SessionPermissions;
        }

        public async Task<PagedResult<User>> GetUsersAsync(string id, int page = 1)
        {
            var response = await Client.GetSessionUsers(id, _PageSize, _PageSize * (page - 1));
            return new PagedResult<User>()
            {
                Items = response.SessionUsers,
                TotalItems = response.TotalCount,
                CurrentPage = page,
                ItemsPerPage = _PageSize
            };
        }

        public async Task UpdateCollaborationAccessAsync(string id, bool restrictAccess)
        {
            await Client.UpdateSessionAsync(id, null, restrictAccess, null, null, null, null);
        }

        public async Task UpdateUserRestrictedStatusAsync(string id, int userId, string restrictedStatus)
        {
            await Client.UpdateSessionUserRestrictedStatus(id, userId, restrictedStatus);
        }

        public Task SetUserPermissionsAsync(string id, int userId, string permission, bool? allow)
        {
            throw new NotImplementedException();
        }

        public async Task<PagedResult<SessionActivityRecord>> GetActivity(string id, int page = 1)
        {
            var usersTask = Client.GetSessionUsers(id, 500, 0);
            var actTask = Client.GetSessionActivity(id, _PageSize, _PageSize * (page - 1));

            await Task.WhenAll(usersTask, actTask);

            var users = usersTask.Result;
            var act = actTask.Result;

            var usersDict = users.SessionUsers.ToDictionary(u => u.Id);
            var items = new List<SessionActivityRecord>();
            foreach (var a in act.SessionActivities)
            {
                User u = null;
                if (usersDict.ContainsKey(a.UserId))
                    u = usersDict[a.UserId];
                var ar = new SessionActivityRecord
                {
                    Email = u?.Email ?? $"<{a.UserId}>",
                    Name = u?.Name ?? $"<{a.UserId}>",
                    Message = a.Message,
                    Timestamp = a.Created
                };
                items.Add(ar);
            }

            return new PagedResult<SessionActivityRecord>()
            {
                Items = items,
                TotalItems = act.TotalCount,
                CurrentPage = page,
                ItemsPerPage = _PageSize
            };
        }
    }
}
