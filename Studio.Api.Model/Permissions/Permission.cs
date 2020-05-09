using System;
using System.Collections.Generic;

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

        public Permission()
        { }

        public Permission(string type, bool? allow)
        {
            Type = type;
            Allow = allow.HasValue ?
                                    (allow.Value ? PermissionValue.Allow : PermissionValue.Deny)
                                    : PermissionValue.Default;
        }

        public Permission(string type, string allow)
        {
            Type = type;
            switch (allow)
            {
                case PermissionValue.Allow:
                    Allow = PermissionValue.Allow;
                    break;
                case PermissionValue.Deny:
                    Allow = PermissionValue.Deny;
                    break;
                case PermissionValue.Default:
                    Allow = PermissionValue.Default;
                    break;
                default:
                    throw new ArgumentException($"Permission 'Allow' not allowed to have value: {allow}");
            }
        }

        public static bool AllowIsValid(string allow)
        {
            if (string.IsNullOrWhiteSpace(allow) || allow == "-")
                return false;

            if (allow == "Allow" || allow == "Deny" || allow == "Default")
                return true;

            return false;
        }
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
        public const string CreateSessions = "CreateSessions";
        public const string UndoCheckouts = "UndoCheckouts";
        public const string Invite = "Invite";
        public const string ManageParticipants = "ManageParticipants";
        public const string ManagePermissions = "ManagePermissions";
        public const string ShareItems = "ShareItems";
        public const string FullControl = "FullControl";
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
