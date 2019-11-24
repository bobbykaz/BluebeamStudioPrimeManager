using System;
using System.Collections.Generic;

namespace Studio.Api.Model
{
    public class Project
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool Restricted { get; set; }
        public string OwnerEmail { get; set; }
        public DateTime Created { get; set; }
        public string InviteUrl { get; set; }
    }

    public class ProjectsList
    {
        public List<Project> Projects { get; set; }
        public int TotalCount { get; set; }
    }
}
