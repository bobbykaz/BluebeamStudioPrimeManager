using System;
using System.Collections.Generic;
using System.Text;

namespace Studio.Api.Model.Users
{
    public class User
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string RestrictedStatus { get; set; }
        public int Id { get; set; }
    }

    public class ProjectUsersList
    { 
        public List<User> ProjectUsers { get; set; }
        public int TotalCount { get; set; }
    }

    public class SessionUsersList
    {
        public List<User> SessionUsers { get; set; }
        public int TotalCount { get; set; }
    }

    public class UpdateUserRestrictedStatusRequest
    { 
        public string RestrictedStatus { get; set; }
    }
}
