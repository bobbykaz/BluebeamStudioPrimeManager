using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PrimeCollaborationManager.Models;
using PrimeCollaborationManager.Services;
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
            var token = await HttpContext.GetTokenAsync("access_token");
            Client.SetAuthHeader(token);
            CollaborationService = new SessionsCollabService(Client);
        }
    }
}