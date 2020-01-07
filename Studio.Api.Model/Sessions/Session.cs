using System;
using System.Collections.Generic;
using System.Text;

namespace Studio.Api.Model.Sessions
{
    public class Session
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool Restricted { get; set; }
        public string OwnerEmail { get; set; }
        public DateTime Created { get; set; }
        public string InviteUrl { get; set; }
        public DateTime? SessionEndDate { get; set; }
    }

    public class SessionsList
    { 
        public List<Session> Sessions { get; set; }
        public int TotalCount { get; set; }
    }

    public class CreateSessionRequest
    {
        public string Name { get; set; }
        public bool Restricted { get; set; }
        public bool Notification { get; set; }
    }

    public class CreateSessionResponse
    {
        public string Id { get; set; }
    }
}
