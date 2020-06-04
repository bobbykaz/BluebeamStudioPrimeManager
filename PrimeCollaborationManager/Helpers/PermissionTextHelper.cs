using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrimeCollaborationManager.Helpers
{
    public static class PermissionTextHelper
    {
        public static string GetPermissionDisplayName(string permType)
        {
            if (permType == null)
                throw new ArgumentNullException(nameof(permType));

            switch (permType)
            {
                case "SaveCopy":
                    return "Save As";
                case "PrintCopy":
                    return "Print";
                case "Markup":
                    return "Markup";
                case "AddDocuments":
                    return "Add Documents";
                case "MarkupAlert":
                    return "Markup Alert";
                case "UndoCheckouts":
                    return "Revoke Check Out";
                case "CreateSessions":
                    return "Send PDFs to Studio Sessions";
                case "ShareItems":
                    return "Share File Links";
                case "Invite":
                    return "Send Invitations";
                case "ManageParticipants":
                    return "Manage User Access";
                case "ManagePermissions":
                    return "Manage Permissions";
                case "FullControl":
                    return "Full Control";
                default:
                    return permType;
            }
        }
    }
}
