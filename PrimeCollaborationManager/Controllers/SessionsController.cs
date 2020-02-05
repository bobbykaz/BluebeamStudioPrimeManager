using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PrimeCollaborationManager.Helpers;
using PrimeCollaborationManager.Models;
using PrimeCollaborationManager.Services;
using Serilog;
using Studio.Api.Client;

namespace PrimeCollaborationManager.Controllers
{
    public class SessionsController : ProjectsController
    {
        public SessionsController(StudioApplicationConfig config)
            :base(config)
        {
        }

        protected override async Task InitClient()
        {
            UserLog = UserHelper.GetCurrentUser(HttpContext);
            Client = new StudioClient(Config, UserLog, Log.Logger);
            var token = await HttpContext.GetTokenAsync("access_token");
            Client.SetAuthHeader(token);
            CollaborationService = new SessionsCollabService(Client);
        }
    }
}