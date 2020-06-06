using Certes;
using FluffySpoon.AspNet.LetsEncrypt;
using FluffySpoon.AspNet.LetsEncrypt.Certes;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace PrimeCollaborationManager
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //var connection = Configuration["DB:ConnectionString"];

            //services.AddDbContext<AppDbContext>(options =>
            //    options.UseSqlite(connection)
            //);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .CreateLogger();

            services.AddMvc();

            var apiConfig = new Studio.Api.Client.StudioApplicationConfig()
            {
                ClientId = Configuration["StudioApi:ClientId"],
                ClientSecret = Configuration["StudioApi:ClientSecret"],
                TokenEndpoint = Configuration["StudioApi:AuthServerTokenEndpoint"],
                StudioApiBaseUrl = Configuration["StudioApi:StudioApiBaseUrl"],
                AuthorizationEndpoint = Configuration["StudioApi:AuthServerAuthorizeEndpoint"],
                CallbackPath = Configuration["StudioApi:ClientRedirectPath"],
                ApiResultPageSize = int.Parse(Configuration["StudioApi:ApiResultPageSize"]),
                TokenRefreshEarlyMinutes = int.Parse(Configuration["StudioApi:MinutesToRefreshBeforeExpiration"]),
            };

            services.AddSingleton(apiConfig);
            int cookieHourTime = int.Parse(Configuration["Settings:CookieExpirationHours"]);
            int cookieMinTime = int.Parse(Configuration["Settings:CookieExpirationMinutes"]);
            Log.Logger.Information($"App Startup - Using Client {apiConfig.ClientId} - Api {apiConfig.StudioApiBaseUrl} - Auth {apiConfig.AuthorizationEndpoint} - Redirect {apiConfig.CallbackPath} - PageSize {apiConfig.ApiResultPageSize} - TokenRefreshEarlyMinutes {apiConfig.TokenRefreshEarlyMinutes} - Cookie Expiration {cookieHourTime} hrs {cookieMinTime} minutes");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = "Bluebeam";
            })
            .AddOAuth("Bluebeam", options =>
            {
                options.ClientId = apiConfig.ClientId;
                options.ClientSecret = apiConfig.ClientSecret;
                options.CallbackPath = new PathString(apiConfig.CallbackPath);
                //options.Scope.Add("full_user");
                options.Scope.Add("full_prime");
                options.AuthorizationEndpoint = apiConfig.AuthorizationEndpoint;
                options.TokenEndpoint = apiConfig.TokenEndpoint;
                options.UserInformationEndpoint = apiConfig.UserInformationEndpoint;
                options.AccessDeniedPath = new PathString("/Home/Denied");
                options.SaveTokens = true;

                options.ClaimActions.MapJsonKey(ClaimTypes.Email, "Email");
                options.ClaimActions.MapJsonKey(ClaimTypes.Name, "Email");
                options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "UserId");
                options.ClaimActions.MapJsonKey("urn:bluebeam:displayname", "DisplayName");
                options.ClaimActions.MapJsonKey("urn:bluebeam:primerole", "PrimeMemberRole");

                options.Events = new OAuthEvents
                {
                    OnCreatingTicket = async context =>
                    {
                        var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);
                        var response = await context.Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted);
                        response.EnsureSuccessStatusCode();

                        var user = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

                        context.RunClaimActions(user.RootElement);
                    },
                    OnRemoteFailure = context =>
                    {
                        Log.Logger.Warning($"Remote Failure: {context.Failure}");
                        if (context.Failure.Message == "user_denied_authorization")
                        {
                            context.Response.Redirect("/Home/Denied");
                            context.HandleResponse();
                        }
                        return Task.CompletedTask;
                    }
                };
            })
            .AddCookie(options =>
            {
                options.ExpireTimeSpan = new TimeSpan(cookieHourTime, cookieMinTime, 0);
                options.SlidingExpiration = false;
                options.Events = new CookieAuthenticationEvents()
                {
                    OnValidatePrincipal = async context =>
                    {
                        await RefreshTokenHelper.Refresh(context, apiConfig);
                    }
                };
            });
            services.AddControllersWithViews();

            //LetsEncrypt
            if (bool.Parse(Configuration["LetsEncrypt:Enabled"]))
            {
                var certEmail = Configuration["LetsEncrypt:Email"];
                var certCountry = Configuration["LetsEncrypt:CountryName"];
                var certLocality = Configuration["LetsEncrypt:Locality"];
                var certState = Configuration["LetsEncrypt:State"];
                var certOrg = Configuration["LetsEncrypt:Organization"];
                var certOrgUnit = Configuration["LetsEncrypt:OrganizationUnit"];
                var certUseStaging = bool.Parse(Configuration["LetsEncrypt:UseStaging"]);
                var certDomains = Configuration.GetSection("LetsEncrypt:Domains").GetChildren().Select(s => s.Value).ToArray();
                Log.Logger.Information($"App Startup - Cert info: {certEmail} {certCountry} / {certLocality} / {certState} [{certOrg} {certOrgUnit}] UseStage: {certUseStaging} - Domains: {certDomains}");

                services.AddFluffySpoonLetsEncrypt(new LetsEncryptOptions()
                {
                    Email = certEmail, //LetsEncrypt will send you an e-mail here when the certificate is about to expire
                    UseStaging = certUseStaging, //switch to true for testing
                    Domains = certDomains,
                    TimeUntilExpiryBeforeRenewal = TimeSpan.FromDays(30), //renew automatically 30 days before expiry
                    TimeAfterIssueDateBeforeRenewal = TimeSpan.FromDays(7), //renew automatically 7 days after the last certificate was issued
                    CertificateSigningRequest = new CsrInfo() //these are your certificate details
                    {
                        CountryName = certCountry,
                        Locality = certLocality,
                        Organization = certOrg,
                        OrganizationUnit = certOrgUnit,
                        State = certState
                    },
                    RenewalFailMode = RenewalFailMode.LogAndRetry
                });

                //the following line tells the library to persist challenges in-memory. challenges are the "/.well-known" URL codes that LetsEncrypt will call.
                services.AddFluffySpoonLetsEncryptMemoryChallengePersistence();
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            if (bool.Parse(Configuration["LetsEncrypt:Enabled"]))
            {
                app.UseFluffySpoonLetsEncrypt();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            Log.Logger.Information($"App Configure Complete");
        }
    }
}
