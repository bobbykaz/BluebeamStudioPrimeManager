using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PrimeCollaborationManager.Models;
using Studio.Api.Client;
using Studio.Api.Model;

namespace PrimeCollaborationManager.Services
{
    public class ProjectsCollabService : ICollaborationService
    {
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
            throw new NotImplementedException();
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
    }
}
