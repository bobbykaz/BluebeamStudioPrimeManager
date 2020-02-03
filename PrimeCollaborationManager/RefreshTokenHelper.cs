// Copyright (c) Bluebeam Inc. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Authentication.Cookies;
using PrimeCollaborationManager.Helpers;
using Serilog;
using Studio.Api.Client;
using System;
using System.Threading.Tasks;

namespace PrimeCollaborationManager
{
    public static class RefreshTokenHelper
    {
        public static async Task Refresh(CookieValidatePrincipalContext ctx, StudioApplicationConfig cfg)
        {
            if(ctx == null)
                throw new ArgumentException("ctx is missing from Refresh method");

            if (ctx.Properties.ExpiresUtc < DateTime.Now)
            {
                ctx.RejectPrincipal();
                return;
            }

            try
            {
                var expiresAt = ctx.Properties.Items[".Token.expires_at"];
                if (expiresAt != null)
                {
                    DateTime expTime;
                    bool istime = DateTime.TryParse(expiresAt, out expTime);
                    if (istime && expTime.AddMinutes(-30) < DateTime.Now)
                    {
                        var currentUser = UserHelper.GetCurrentUser(ctx.HttpContext);

                        var client = new StudioClient(cfg, currentUser, Log.Logger);
                        var refreshToken = ctx.Properties.Items[".Token.refresh_token"];
                        var token = await client.RefreshToken(refreshToken);
                        ctx.Properties.Items[".Token.expires_at"] = token.ExpiresTime.ToString();
                        ctx.Properties.Items[".Token.access_token"] = token.AccessToken;
                        ctx.Properties.Items[".Token.refresh_token"] = token.RefreshToken;
                        ctx.ShouldRenew = true;
                    }

                }
            }
            catch (Exception e)
            {
                Log.Logger.Error("Error Refreshing token {Exception}",e);
                ctx.RejectPrincipal();
            }
        }
    }
}