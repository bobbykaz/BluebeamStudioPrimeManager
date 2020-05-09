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
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

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
                CallbackPath = Configuration["StudioApi:ClientRedirectPath"]
            };

            services.AddSingleton(apiConfig);

            Log.Logger.Information($"App Startup - Using Client {apiConfig.ClientId} - Api {apiConfig.StudioApiBaseUrl} - Auth {apiConfig.AuthorizationEndpoint} - Redirect {apiConfig.CallbackPath}");

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
                    }
                };
            })
            .AddCookie(options =>
            {
                options.ExpireTimeSpan = new TimeSpan(0, 5, 0);
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
        }
    }
}
