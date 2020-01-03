using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrimeCollaborationManager.Data
{
    public class Team
    {
        public long TeamId { get; set; }
        public long UserId { get; set; }
        public string Name { get; set; }

        public List<TeamMember> TeamMembers { get; set; }
        public List<TeamPermission> TeamPermissions { get; set; }
    }
}
