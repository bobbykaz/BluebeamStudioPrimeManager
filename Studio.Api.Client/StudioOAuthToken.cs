using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Studio.Api.Client
{
    public class StudioOAuthToken
    {
        public class TokenDto
        {
            [JsonProperty("access_token")]
            public string AccessToken { get; set; }
            [JsonProperty("userName")]
            public string UserName { get; set; }
            [JsonProperty("client_id")]
            public string ClientID { get; set; }
            [JsonProperty("scope")]
            public string Scopes { get; set; }
            [JsonProperty("expires_in")]
            public int ExpiresIn { get; set; }
            [JsonProperty(".expires")]
            public DateTimeOffset ExpiresTime { get; set; }
            [JsonProperty(".issued")]
            public DateTimeOffset IssuedTime { get; set; }
            [JsonProperty("refresh_token")]
            public string RefreshToken { get; set; }
            [JsonProperty("refresh_token_expires_in")]
            public int RefreshTokenExpiresIn { get; set; }
            public DateTimeOffset RefreshTokenExpiresTime { get { return IssuedTime.AddSeconds(RefreshTokenExpiresIn); } }
        }
    }
}
