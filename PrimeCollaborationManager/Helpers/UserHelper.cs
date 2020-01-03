using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PrimeCollaborationManager.Helpers
{
    public class UserHelper
    {
        public static int GetBBUserId(HttpContext ctx)
        {
            if (ctx.User.Identity != null)
            {
                var idClaim = ctx.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                return int.Parse(idClaim.Value);
            }

            throw new Exception("No user in context!");
        }

        public static string GetEmail(HttpContext ctx)
        {
            if (ctx.User.Identity != null)
            {
                var emailClaim = ctx.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
                return emailClaim.Value;
            }

            throw new Exception("No user in context!");
        }
    }
}
