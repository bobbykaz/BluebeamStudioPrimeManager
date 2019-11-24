using System;
using System.Collections.Generic;
using System.Text;

namespace Studio.Api.Model
{
    public class ProjectFolder
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public int ParentFolderId { get; set; }
        public string IdPath { get; set; }
        public DateTime Created { get; set; }
        public int Id { get; set; }
        public string Permission { get; set; }
    }

    public class ProjectFolderContents
    {
        public List<ProjectFile> ProjectFiles { get; set; }
        public List<ProjectFolder> ProjectFolders { get; set; }
        public int TotalCount { get; set; }
    }
}
