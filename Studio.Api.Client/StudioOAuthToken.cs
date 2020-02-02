using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Studio.Api.Client
{
    public class StudioOAuthToken
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
        [JsonPropertyName("userName")]
        public string UserName { get; set; }
        [JsonPropertyName("client_id")]
        public string ClientID { get; set; }
        [JsonPropertyName("scope")]
        public string Scopes { get; set; }
        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
        [JsonPropertyName(".expires")]
        public string Expires { get; set; }
        public DateTime ExpiresTime { get { return DateTime.Parse(Expires); }  }
        [JsonPropertyName(".issued")]
        public string Issued { get; set; }
        public DateTime IssuedTime { get { return DateTime.Parse(Expires); } }
        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }
        [JsonPropertyName("refresh_token_expires_in")]
        public string RefreshTokenExpiresIn { get; set; }
        public DateTime RefreshTokenExpiresTime { get { return IssuedTime.AddSeconds(int.Parse(RefreshTokenExpiresIn)); } }
    }
}
