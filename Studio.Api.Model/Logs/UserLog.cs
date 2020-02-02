using System;
using System.Collections.Generic;
using System.Text;

namespace Studio.Api.Model.Logs
{
    public class UserLog
    {
        public int UserID { get; set; }
        public string UserEmail { get; set; }
        public string RequestID { get; set; }
    }
}
