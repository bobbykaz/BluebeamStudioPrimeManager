﻿namespace Studio.Api.Client
{
    public class StudioApplicationConfig
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string StudioApiBaseUrl { get; set; }
        public string TokenEndpoint { get; set; }
        public string AuthorizationEndpoint { get; set; }
        public string UserInformationEndpoint { get { return $"{StudioApiBaseUrl}/users/me"; } }
        public string CallbackPath { get; set; }
        public int ApiResultPageSize { get; set; }
        public int TokenRefreshEarlyMinutes { get; set; }
    }
}
