using Studio.Api.Client;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace PrimeCollaborationManager.Tests
{
    public class DataGenerator
    {
        public static StudioApplicationConfig GetStudioAppConfig()
        {
            return new StudioApplicationConfig
            {
                AuthorizationEndpoint = "http://localhost/auth",
                CallbackPath = "http://localhost/return",
                ClientId = "1111",
                ClientSecret = "secret",
                StudioApiBaseUrl = "http://localhost/api",
                TokenEndpoint = "http://localhost/auth/token",
            };
        }

        public static StudioApiException GetStudioApiException(string code, string error, HttpResponseMessage response)
        {
            return new StudioApiException
            {
                StudioErrorCode = code,
                StudioErrorMessage = error,
                Response = response
            };
        }
    }
}
