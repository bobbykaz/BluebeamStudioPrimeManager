using Microsoft.AspNetCore.Http;
using Studio.Api.Model.Logs;
using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;

namespace PrimeCollaborationManager.Helpers
{
    public static class UserHelper
    {
        public static UserLog GetCurrentUser(HttpContext ctx)
        {
            if (ctx == null)
                throw new ArgumentException("HttpContext is null");

            var result = new UserLog
            {
                UserID = GetBBUserId(ctx.User),
                UserEmail = GetEmail(ctx.User),
                RequestID = Activity.Current?.Id ?? Guid.NewGuid().ToString(),
                TraceIdentifier = ctx.TraceIdentifier
            };

            return result;
        }

        public static int GetBBUserId(ClaimsPrincipal userIdent)
        {
            if (userIdent == null || userIdent.Claims == null)
                return -1;

            var idClaim = userIdent.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            var id = -1;

            if (idClaim != null && int.TryParse(idClaim.Value, out id))
                return id;

            return -1;
        }

        public static string GetEmail(ClaimsPrincipal userIdent)
        {
            if (userIdent == null || userIdent.Claims == null)
                return EmailNotFound;

            var emailClaim = userIdent.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            if (emailClaim == null || string.IsNullOrWhiteSpace(emailClaim.Value))
                return EmailNotFound;

            return emailClaim.Value;
        }

        public const string EmailNotFound = "[email claim not found]";
    }
}
