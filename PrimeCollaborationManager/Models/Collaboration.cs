using Studio.Api.Model.Permissions;
using Studio.Api.Model.Users;
using System;
using System.Collections.Generic;

namespace PrimeCollaborationManager.Models
{
    public class CollaborationList 
    { 
        public List<Collaboration> Collaborations { get; set; }
        public int TotalCollabs { get; set; }
        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; }
        public bool ShowCreate { get; set; }
        public bool ShowTimes { get; set; }
        public bool ShowStatus { get; set; }
    }

    public class Collaboration
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool Restricted { get; set; }
        public string Owner { get; set; }
        public DateTime Created { get; set; }
        public string InviteUrl { get; set; }
        public string Status { get; set; }
    }

    public class CollaborationDetails
    { 
        public Collaboration Collab { get; set; }
        public List<Permission> Permissions { get; set; }
        public List<User> Users { get; set; }
    }
}
