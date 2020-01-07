using Studio.Api.Model;
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
    public interface IStudioClient 
    { 
    }

    public class StudioClient
    {
        protected HttpClient _Client { get; set; }

        public string _ClientId { get; protected set; }
        public string _ClientSecret { get; protected set; }

        public string _TokenEndpoint { get; protected set; }
        public string _StudioApiBase { get; protected set; }
        public string _UserInformationEndpoint { get{ return $"{_StudioApiBase}/users/me"; } }

        public StudioClient(StudioApplicationConfig config)
        {
            _ClientId = config.ClientId;
            _ClientSecret = config.ClientSecret;
            _TokenEndpoint = config.TokenEndpoint;
            _StudioApiBase = config.StudioApiBaseUrl;

            if (_StudioApiBase[_StudioApiBase.Length - 1] != '/')
                _StudioApiBase = _StudioApiBase + '/';

            _Client = new HttpClient() { BaseAddress = new Uri(_StudioApiBase), Timeout = TimeSpan.FromSeconds(30) };
            _Client.DefaultRequestHeaders.Add("Accept","application/json");
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

            var response = await _Client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var token = JsonSerializer.Deserialize<StudioOAuthToken>(await response.Content.ReadAsStringAsync());
            return token;
        }
        #endregion

        #region Helpers
        protected async Task<T> Get<T>(string route)
        {
            var response = await _Client.GetAsync(route);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            var responseObj = JsonSerializer.Deserialize<T>(content);

            return responseObj;
        }

        protected async Task Put<TRequest>(string route, TRequest request)
        {
            var content = new StringContent(JsonSerializer.Serialize(request), System.Text.Encoding.UTF8, "application/json");
            var response = await _Client.PutAsync(route, content);
            response.EnsureSuccessStatusCode();
        }

        protected async Task<TResponse> Put<TRequest, TResponse>(string route, TRequest request)
        {
            var content = new StringContent(JsonSerializer.Serialize(request), System.Text.Encoding.UTF8, "application/json");
            var response = await _Client.PutAsync(route, content);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            var responseObj = JsonSerializer.Deserialize<TResponse>(responseContent);

            return responseObj;
        }

        protected async Task Post(string route)
        {
            var response = await _Client.PostAsync(route, null);
            response.EnsureSuccessStatusCode();
        }

        protected async Task Post<TRequest>(string route, TRequest request)
        {
            var content = new StringContent(JsonSerializer.Serialize(request), System.Text.Encoding.UTF8, "application/json");
            var response = await _Client.PostAsync(route, content);
            response.EnsureSuccessStatusCode();
        }

        protected async Task<TResponse> Post<TRequest,TResponse>(string route, TRequest request)
        {
            var content = new StringContent(JsonSerializer.Serialize(request), System.Text.Encoding.UTF8, "application/json");
            var response = await _Client.PostAsync(route, content);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            var responseObj = JsonSerializer.Deserialize<TResponse>(responseContent);

            return responseObj;
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
            using(var client = new HttpClient())
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

        public async Task<ProjectFolderContents> GetProjectFolderContents( string projectId, int folderId)
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

        public async Task<SessionUsersList> GetSessionUsers(string sessionId, int limit = 100, int offset = 0)
        {
            return await Get<SessionUsersList>($"sessions/{sessionId}/users?limit={limit}&offset={offset}");
        }
        #endregion
    }
}
