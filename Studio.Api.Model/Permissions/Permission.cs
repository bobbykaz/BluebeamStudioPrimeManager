using System;
using System.Collections.Generic;
using System.Text;

namespace Studio.Api.Model.Permissions
{
    public class ProjectPermissionsList
    {
        public List<Permission> ProjectPermissions { get; set; }
    }

    public class SessionPermissionsList
    {
        public List<Permission> SessionPermissions { get; set; }
    }

    public class Permission
    {
        public string Type { get; set; }
        public string Allow { get; set; }
    }

    public class PermissionValue
    {
        public const string Allow = "Allow";
        public const string Deny = "Deny";
        public const string Default = "Default";
    }

    public class FolderPermissionVale
    {
        public const string Read = "Read";
        public const string ReadWrite = "ReadWrite";
        public const string ReadWriteDelete = "ReadWriteDelete";
        public const string Hidden = "Hidden";
    }

    public class ProjectPermissionType
    {
        public const string CreateSessions = "";
        public const string UndoCheckouts = "";
        public const string Invite = "";
        public const string ManageParticipants = "";
        public const string ManagePermissions = "";
        public const string ShareItems = "";
        public const string FullControl = "";
    }

    public class SessionPermissionType
    {
        public const string SaveCopy = "SaveCopy";
        public const string PrintCopy = "PrintCopy";
        public const string Markup = "Markup";
        public const string AddDocuments = "AddDocuments";
        public const string MarkupAlert = "MarkupAlert";
        public const string FullControl = "FullControl";
    }
}
