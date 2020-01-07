using Studio.Api.Model.Permissions;
using Studio.Api.Model.Users;
using System;
using System.Collections.Generic;

namespace PrimeCollaborationManager.Models
{
    public class CollaborationList 
    { 
        public PagedResult<Collaboration> Collaborations { get; set; }
        public bool ShowCreate { get; set; }
        public bool ShowTimes { get; set; }
        public bool ShowStatus { get; set; }
    }

    public class Collaboration
    {
        public string Type { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public bool Restricted { get; set; }
        public string Owner { get; set; }
        public DateTime Created { get; set; }
        public string InviteUrl { get; set; }
        public string Status { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class CollaborationDetails
    { 
        public Collaboration Collab { get; set; }
        public List<Permission> Permissions { get; set; }
        public PagedResult<User> Users { get; set; }
        public bool ShowStatus { get; set; }
    }
}
