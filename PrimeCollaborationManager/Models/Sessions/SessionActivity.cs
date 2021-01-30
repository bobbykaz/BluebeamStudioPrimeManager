using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrimeCollaborationManager.Models
{
    public class SessionActivityRecord
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class SessionActivity
    {
        public CollaborationDetails CollaborationDetails { get; set; }
        public PagedResult<SessionActivityRecord> Activities { get; set; }
    }
}
