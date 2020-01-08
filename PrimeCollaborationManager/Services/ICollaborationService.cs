using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PrimeCollaborationManager.Services
{
    public interface ICollaborationService
    {
        Task<Models.CollaborationList> GetListAsync(int page = 1);
        Task<Models.Collaboration> GetDetailsAsync(string id);
        Task<List<Studio.Api.Model.Permissions.Permission>> GetPermissionsAsync(string id);
        Task<Models.PagedResult<Studio.Api.Model.Users.User>> GetUsersAsync(string id, int page = 1);
        Task UpdateCollaborationAccessAsync(string id, bool restrictAccess);
        Task UpdateUserRestrictedStatusAsync(string id, int userId, string restrictedStatus);
        Task SetPermissionsAsync(string id, string permission, bool? allow);
        List<string> GetPermissionTypes();
        Task<string> CreateAsync(IFormCollection form);
    }
}
