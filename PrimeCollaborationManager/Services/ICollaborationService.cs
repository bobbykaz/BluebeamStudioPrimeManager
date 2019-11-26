using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PrimeCollaborationManager.Services
{
    public interface ICollaborationService
    {
        Task<List<Models.Collaboration>> GetListAsync();
        Task<Models.Collaboration> GetDetailsAsync(string id);
        Task<List<Studio.Api.Model.Permissions.Permission>> GetPermissionsAsync(string id);
        Task<List<Studio.Api.Model.Users.User>> GetUsersAsync(string id);
        Task SetPermissionsAsync(string id, string permission, bool? allow);
        List<string> GetPermissionTypes();
        Task<string> CreateAsync(IFormCollection form);
    }
}
