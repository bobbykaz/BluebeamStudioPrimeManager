using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrimeCollaborationManager.Data
{
    public class TeamMember
    {
        public long TeamMemberId { get; set; }
        public string Email { get; set; }
        public long BBUserId { get; set; }

        public long TeamId { get; set; }
        public Team Team { get; set; }
    }
}
