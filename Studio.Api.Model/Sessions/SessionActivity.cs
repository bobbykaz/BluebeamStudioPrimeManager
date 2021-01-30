using System;
using System.Collections.Generic;
using System.Text;

namespace Studio.Api.Model.Sessions
{
    public class SessionActivityList
    { 
        public List<SessionActivity> SessionActivities { get; set; }
        public long TotalCount { get; set; }
    }

    public class SessionActivity
    {
        public long Id { get; set; }
        public long DocumentId { get; set; }
        public long UserId { get; set; }
        public string Message { get; set; }
        public DateTime Created { get; set; }
    }
}
