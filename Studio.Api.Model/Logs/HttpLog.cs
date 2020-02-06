using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Studio.Api.Model.Logs
{
    public class HttpLog
    {
        public string Method { get; set; }
        public string Route { get; set; }
        public string RequestBody { get; set; }

        public string ReasonPhrase { get; set; }
        public int StatusCode { get; set; }
        public List<string> Headers { get; set; }
        public string ResponseBody { get; set; }

        public static async Task<HttpLog> FromHttpResponse(HttpResponseMessage response)
        {
            var result = new HttpLog { Headers = new List<string>() };

            result.Method = response.RequestMessage.Method.Method;
            result.Route = response.RequestMessage.RequestUri.ToString();
            if(!response.IsSuccessStatusCode && response.RequestMessage.Content != null)
                result.RequestBody = await response.RequestMessage.Content.ReadAsStringAsync();

            result.ReasonPhrase = response.ReasonPhrase;
            result.StatusCode = (int)response.StatusCode;
            if(!response.IsSuccessStatusCode && response.Content != null )
                result.ResponseBody = await response.Content.ReadAsStringAsync();
            
            foreach (var pair in response.Headers)
            {
                result.Headers.Add(string.Format($"{pair.Key}: {string.Join(";", pair.Value)}"));
            }

            return result;
        }
    }
}
