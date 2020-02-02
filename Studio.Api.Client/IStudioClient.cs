using Studio.Api.Model;
using Studio.Api.Model.Permissions;
using Studio.Api.Model.Sessions;
using Studio.Api.Model.Users;
using System;
using System.Threading.Tasks;

namespace Studio.Api.Client
{
    public interface IStudioClient
    {
        Task<ProjectFileUploadResponse> BeginUploadProjectFile(string projectId, ProjectFileUploadRequest request);
        Task ConfirmUploadProjectFile(string projectId, int projectFileId);
        Task<string> CreateProject(string name, bool subToNotifications, bool restrictAccess);
        Task<string> CreateSession(string name, bool subToNotifications, bool restrictAccess);
        Task<Project> GetProjectDetails(string projectId);
        Task<ProjectFileDetails> GetProjectFileDetails(string projectId, int fileId);
        Task<ProjectFileRevisionDetails> GetProjectFileRevisionDetails(string projectId, int fileId, int revisionId);
        Task<ProjectFileRevisionsList> GetProjectFileRevisions(string projectId, int fileId);
        Task<ProjectFolderContents> GetProjectFolderContents(string projectId, int folderId);
        Task<ProjectFolder> GetProjectFolderDetails(string projectId, int folderId);
        Task<ProjectPermissionsList> GetProjectPermissions(string projectId);
        Task<ProjectsList> GetProjectsList(int limit = 100, int offset = 0);
        Task<ProjectPermissionsList> GetProjectUserPermissions(string projectId, int userId);
        Task<ProjectUsersList> GetProjectUsers(string projectId, int limit = 100, int offset = 0);
        Task<Session> GetSessionDetails(string id);
        Task<SessionPermissionsList> GetSessionPermissions(string sessionId);
        Task<SessionsList> GetSessionsList(int limit = 100, int offset = 0);
        Task<SessionPermissionsList> GetSessionUserPermissions(string sessionId, int userId);
        Task<SessionUsersList> GetSessionUsers(string sessionId, int limit = 100, int offset = 0);
        Task<StudioOAuthToken> RefreshToken(string refreshToken);
        Task RestoreProjectFileRevision(string projectId, int fileId, int revisionId);
        void SetAuthHeader(string accessToken);
        Task<Project> UpdateProjectAsync(string id, string newName, bool? access, bool? notifications, string newOwnerEmail);
        Task UpdateProjectPermissions(string projectId, Permission perm);
        Task UpdateProjectUserPermissions(string projectId, int userId, Permission perm);
        Task UpdateProjectUserRestrictedStatus(string projectId, int userId, string restrictedStatus);
        Task<Session> UpdateSessionAsync(string id, string newName, bool? access, bool? notifications, string newOwnerEmail, string newStatus, DateTime? newEndDate);
        Task UpdateSessionPermissions(string sessionId, Permission perm);
        Task UpdateSessionUserPermissions(string sessionId, int userId, Permission perm);
        Task UpdateSessionUserRestrictedStatus(string sessionId, int userId, string restrictedStatus);
    }
}