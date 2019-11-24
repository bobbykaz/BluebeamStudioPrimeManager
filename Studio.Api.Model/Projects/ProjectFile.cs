using System;
using System.Collections.Generic;
using System.Text;

namespace Studio.Api.Model
{
    public class ProjectFile
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public int ParentFolderId { get; set; }
        public string IdPath { get; set; }
        public DateTime Created { get; set; }
        public int Id { get; set; }
        public string Permission { get; set; }
        public bool IsLocked { get; set; }
        public bool InSession { get; set; }
        public string SessionName { get; set; }
        public string SessionId { get; set; }
        public string LockOwnerEmail { get; set; }
        public string LockOwnerName { get; set; }
        public int RevisionID { get; set; }
    }

    public class ProjectFileDetails : ProjectFile
    {
        public long Size { get; set; }
        public string CRC { get; set; }
        public string DownloadURL { get; set; }
    }

    public class ProjectFileUploadRequest
    {
        public string Name { get; set; }
        public int ParentFolderId { get; set; }
    }

    public class ProjectFileUploadResponse
    {
        public int Id { get; set; }
        public string UploadUrl { get; set; }
        public string UploadContentType { get; set; }
    }
}
