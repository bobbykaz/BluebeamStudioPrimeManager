using Serilog;
using Studio.Api.Model;
using Studio.Api.Model.Logs;
using Studio.Api.Model.Permissions;
using Studio.Api.Model.Sessions;
using Studio.Api.Model.Users;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace Studio.Api.Client
{
    public class StudioClient : IStudioClient
    {
        protected HttpClient _Client { get; set; }
        protected ILogger _Log { get; set; }
        protected string _ClientId { get; set; }
        protected string _ClientSecret { get; set; }
        protected string _TokenEndpoint { get; set; }

        protected UserLog _UserLog { get; set;}

        public StudioClient(StudioApplicationConfig config, UserLog user, ILogger log)
        {
            _Log = log;
            _UserLog = user;
            _ClientId = config.ClientId;
            _ClientSecret = config.ClientSecret;
            _TokenEndpoint = config.TokenEndpoint;
            var studioApiBase = config.StudioApiBaseUrl;

            if (studioApiBase[studioApiBase.Length - 1] != '/')
                studioApiBase = studioApiBase + '/';

            _Client = new HttpClient() { BaseAddress = new Uri(studioApiBase), Timeout = TimeSpan.FromSeconds(30) };
            _Client.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        #region Auth
        public void SetAuthHeader(string accessToken)
        {
            _Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", accessToken);
        }

        public async Task<StudioOAuthToken> RefreshToken(string refreshToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"{_TokenEndpoint}");
            var data = new List<KeyValuePair<string, string>>();

            data.Add(new KeyValuePair<string, string>("grant_type", "refresh_token"));
            data.Add(new KeyValuePair<string, string>("refresh_token", refreshToken));
            data.Add(new KeyValuePair<string, string>("client_id", _ClientId));
            data.Add(new KeyValuePair<string, string>("client_secret", _ClientSecret));

            request.Content = new FormUrlEncodedContent(data);
            var strContent = await request.Content.ReadAsStringAsync();

            var response = await _Client.SendAsync(request);

            await CheckError(response);

            var token = JsonSerializer.Deserialize<StudioOAuthToken>(await response.Content.ReadAsStringAsync());
            return token;
        }
        #endregion

        #region Helpers
        protected async Task<T> Get<T>(string route)
        {
            var response = await _Client.GetAsync(route);
            await CheckError(response);
            var content = await response.Content.ReadAsStringAsync();

            var responseObj = JsonSerializer.Deserialize<T>(content);

            return responseObj;
        }

        protected async Task Put<TRequest>(string route, TRequest request)
        {
            var strContent = JsonSerializer.Serialize(request);
            var content = new StringContent(strContent, System.Text.Encoding.UTF8, "application/json");
            var response = await _Client.PutAsync(route, content);
            await CheckError(response);
        }

        protected async Task<TResponse> Put<TRequest, TResponse>(string route, TRequest request)
        {
            var strContent = JsonSerializer.Serialize(request);
            var content = new StringContent(strContent, System.Text.Encoding.UTF8, "application/json");
            var response = await _Client.PutAsync(route, content);
            await CheckError(response);
            var responseContent = await response.Content.ReadAsStringAsync();
            var responseObj = JsonSerializer.Deserialize<TResponse>(responseContent);

            return responseObj;
        }

        protected async Task Post(string route)
        {
            var response = await _Client.PostAsync(route, null);
            await CheckError(response);
        }

        protected async Task Post<TRequest>(string route, TRequest request)
        {
            var strContent = JsonSerializer.Serialize(request);
            var content = new StringContent(strContent, System.Text.Encoding.UTF8, "application/json");
            var response = await _Client.PostAsync(route, content);
            await CheckError(response);
        }

        protected async Task<TResponse> Post<TRequest, TResponse>(string route, TRequest request)
        {
            var strContent = JsonSerializer.Serialize(request);
            var content = new StringContent(strContent, System.Text.Encoding.UTF8, "application/json");
            var response = await _Client.PostAsync(route, content);
            await Log(response);
            await CheckError(response);
            var responseContent = await response.Content.ReadAsStringAsync();
            var responseObj = JsonSerializer.Deserialize<TResponse>(responseContent);

            return responseObj;
        }

        public async Task Log(HttpResponseMessage resp)
        {
            var httpLog = await HttpLog.FromHttpResponse(resp);
            _Log.Information("User: {@User}; ApiCall: {@ApiCall}", _UserLog, httpLog);
        }

        public async Task CheckError(HttpResponseMessage response)
        {
            await Log(response);
            if (!response.IsSuccessStatusCode)
            {
                if (!string.IsNullOrWhiteSpace(response.ReasonPhrase) && response.ReasonPhrase.IndexOf("-") > 0)
                {
                    int separator = response.ReasonPhrase.IndexOf("-");
                    var e = new StudioApiException { Response = response };
                    e.StudioErrorCode = response.ReasonPhrase.Substring(0, separator).Trim();
                    e.StudioErrorMessage = response.ReasonPhrase.Substring(separator + 1).Trim();
                    throw e;
                }
                throw new StudioApiException { Response = response, StudioErrorCode = "???", StudioErrorMessage = "A problem occurred when communicating with Bluebeam Studio." };
            }
            response.EnsureSuccessStatusCode();
        }

        public static async Task UploadFileToStudioAWS(string url, Stream contentStream, string contentType)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, $"{url}");
            request.Headers.Add("x-amz-server-side-encryption", "AES256");
            request.Content = new StreamContent(contentStream)
            {
                Headers =
                        {
                            ContentLength = contentStream.Length,
                            ContentType = new MediaTypeHeaderValue(contentType)
                        }
            };
            using (var client = new HttpClient())
            {
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
            }
        }

        public static async Task<Stream> DownloadFile(string url)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                return await response.Content.ReadAsStreamAsync();
            }
        }
        #endregion

        #region Sessions
        public async Task<string> CreateSession(string name, bool subToNotifications, bool restrictAccess)
        {
            CreateSessionRequest req = new CreateSessionRequest
            {
                Name = name,
                Notification = subToNotifications,
                Restricted = restrictAccess
            };

            var response = await Post<CreateSessionRequest, CreateSessionResponse>("sessions", req);
            return response.Id;
        }

        public async Task<Session> UpdateSessionAsync(string id, string newName, bool? access, bool? notifications, string newOwnerEmail, string newStatus, DateTime? newEndDate)
        {
            var req = new UpdateSessionRequest
            {
                Name = newName,
                Restricted = access,
                Notification = notifications,
                OwnerEmailOrId = newOwnerEmail,
                SessionEndDate = newEndDate,
                Status = newStatus
            };

            var response = await Put<UpdateSessionRequest, Session>($"sessions/{id}", req);

            return response;
        }

        public async Task<SessionsList> GetSessionsList(int limit = 100, int offset = 0)
        {
            return await Get<SessionsList>($"sessions?limit={limit}&offset={offset}&?includeDeleted=true");
        }

        public async Task<Session> GetSessionDetails(string id)
        {
            return await Get<Session>($"sessions/{id}");
        }
        #endregion

        #region Projects
        public async Task<string> CreateProject(string name, bool subToNotifications, bool restrictAccess)
        {
            CreateProjectRequest req = new CreateProjectRequest
            {
                Name = name,
                Notification = subToNotifications,
                Restricted = restrictAccess
            };

            var response = await Post<CreateProjectRequest, CreateProjectResponse>("projects", req);
            return response.Id;
        }

        public async Task<Project> UpdateProjectAsync(string id, string newName, bool? access, bool? notifications, string newOwnerEmail)
        {
            var req = new UpdateProjectRequest
            {
                Name = newName,
                Restricted = access,
                Notification = notifications,
                OwnerEmailOrId = newOwnerEmail
            };

            var response = await Put<UpdateProjectRequest, Project>($"projects/{id}", req);

            return response;
        }

        public async Task<ProjectsList> GetProjectsList(int limit = 100, int offset = 0)
        {
            return await Get<ProjectsList>($"projects?limit={limit}&offset={offset}");
        }

        public async Task<Project> GetProjectDetails(string projectId)
        {
            return await Get<Project>($"projects/{projectId}");
        }

        public async Task<ProjectFolder> GetProjectFolderDetails(string projectId, int folderId)
        {
            return await Get<ProjectFolder>($"projects/{projectId}/folders/{folderId}");
        }

        public async Task<ProjectFolderContents> GetProjectFolderContents(string projectId, int folderId)
        {
            return await Get<ProjectFolderContents>($"projects/{projectId}/folders/{folderId}/items");
        }

        public async Task<ProjectFileDetails> GetProjectFileDetails(string projectId, int fileId)
        {
            return await Get<ProjectFileDetails>($"projects/{projectId}/folders/{fileId}");
        }

        public async Task<ProjectFileUploadResponse> BeginUploadProjectFile(string projectId, ProjectFileUploadRequest request)
        {
            return await Post<ProjectFileUploadRequest, ProjectFileUploadResponse>($"projects/{projectId}/files", request);
        }

        public async Task ConfirmUploadProjectFile(string projectId, int projectFileId)
        {
            await Post($"projects/{projectId}/files/{projectFileId}/confirm-upload");
        }

        public async Task<ProjectFileRevisionsList> GetProjectFileRevisions(string projectId, int fileId)
        {
            return await Get<ProjectFileRevisionsList>($"projects/{projectId}/files/{fileId}/revisions");
        }

        public async Task<ProjectFileRevisionDetails> GetProjectFileRevisionDetails(string projectId, int fileId, int revisionId)
        {
            return await Get<ProjectFileRevisionDetails>($"projects/{projectId}/files/{fileId}/revisions/{revisionId}");
        }

        public async Task RestoreProjectFileRevision(string projectId, int fileId, int revisionId)
        {
            await Post($"projects/{projectId}/files/{fileId}/revisions/{revisionId}/restore");
        }
        #endregion

        #region Permissions
        public async Task<ProjectPermissionsList> GetProjectPermissions(string projectId)
        {
            return await Get<ProjectPermissionsList>($"projects/{projectId}/permissions");
        }

        public async Task UpdateProjectPermissions(string projectId, Permission perm)
        {
            await Put($"projects/{projectId}/permissions", perm);
        }

        public async Task<ProjectPermissionsList> GetProjectUserPermissions(string projectId, int userId)
        {
            return await Get<ProjectPermissionsList>($"projects/{projectId}/permissions/{userId}");
        }

        public async Task UpdateProjectUserPermissions(string projectId, int userId, Permission perm)
        {
            await Put($"projects/{projectId}/permissions/{userId}", perm);
        }

        public async Task<SessionPermissionsList> GetSessionPermissions(string sessionId)
        {
            return await Get<SessionPermissionsList>($"sessions/{sessionId}/permissions");
        }

        public async Task UpdateSessionPermissions(string sessionId, Permission perm)
        {
            await Put($"sessions/{sessionId}/permissions", perm);
        }

        public async Task<SessionPermissionsList> GetSessionUserPermissions(string sessionId, int userId)
        {
            return await Get<SessionPermissionsList>($"sessions/{sessionId}/permissions/{userId}");
        }

        public async Task UpdateSessionUserPermissions(string sessionId, int userId, Permission perm)
        {
            await Put($"sessions/{sessionId}/permissions/{userId}", perm);
        }
        #endregion

        #region Users
        public async Task<ProjectUsersList> GetProjectUsers(string projectId, int limit = 100, int offset = 0)
        {
            return await Get<ProjectUsersList>($"projects/{projectId}/users?limit={limit}&offset={offset}");
        }

        public async Task UpdateProjectUserRestrictedStatus(string projectId, int userId, string restrictedStatus)
        {
            var req = new UpdateUserRestrictedStatusRequest() { RestrictedStatus = restrictedStatus };
            await Put($"projects/{projectId}/users/{userId}", req);
        }

        public async Task<SessionUsersList> GetSessionUsers(string sessionId, int limit = 100, int offset = 0)
        {
            return await Get<SessionUsersList>($"sessions/{sessionId}/users?limit={limit}&offset={offset}");
        }

        public async Task UpdateSessionUserRestrictedStatus(string sessionId, int userId, string restrictedStatus)
        {
            var req = new UpdateUserRestrictedStatusRequest() { RestrictedStatus = restrictedStatus };
            await Put($"sessions/{sessionId}/users/{userId}", req);
        }
        #endregion
    }
}
