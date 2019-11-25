using Studio.Api.Model.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrimeCollaborationManager.Models
{
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
