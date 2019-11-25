using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PrimeCollaborationManager.Services
{
    public interface ICollaborationService
    {
        Task<List<Models.Collaboration>> GetCollabListAsync();
        Task<Models.Collaboration> GetCollabDetailsAsync(string id);
        Task<Models.Collaboration> CreateCollabAsync(string id, string name, bool restrictUsers);
        Task SetCollabPermissions(string id, string permission, bool? allow);
        List<string> GetCollabPermissionTypes();
        Task<string> CreateCollabAsync(IFormCollection form);
    }
}
