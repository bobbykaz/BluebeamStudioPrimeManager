using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Studio.Api.Client
{
    public class StudioApiException : Exception
    {
        public HttpResponseMessage Response { get; set; }
        public string StudioErrorCode { get; set; }
        public string StudioErrorMessage { get; set; }
    }
}
