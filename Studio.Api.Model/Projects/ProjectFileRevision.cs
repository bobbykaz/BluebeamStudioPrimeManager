using System;
using System.Collections.Generic;
using System.Text;

namespace Studio.Api.Model
{
    public class ProjectFileRevision
    {
        public int Id { get; set; }
        public int RevisionNumber { get; set; }
        public bool RevisionRestorable { get; set; }
        public string OperationType { get; set; }
        public string Comment { get; set; }
        public string UserEmail { get; set; }
        public DateTime TimeStamp { get; set; }
    }

    public class ProjectFileRevisionsList
    {
        public List<ProjectFileRevision> ProjectFileRevisions { get; set; }
        public int TotalCount { get; set; }
    }

    public class ProjectFileRevisionDetails : ProjectFileRevision
    {
        public long Size { get; set; }
        public string CRC { get; set; }
        public string DownloadUrl { get; set; }
    }
}
