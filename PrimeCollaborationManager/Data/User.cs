using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrimeCollaborationManager.Data
{
    public class User
    {
        public long UserId { get; set; }
        public long BBUserId { get; set; }
        public string Email { get; set; }

        //paid stuff?
        public long TeamMemberLimit { get; set; }
        public long TeamLimit { get; set; }
    }
}
