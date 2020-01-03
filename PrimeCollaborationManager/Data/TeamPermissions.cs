using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrimeCollaborationManager.Data
{
    public class TeamPermission
    {
        public long PermissionType { get; set; }
        public PermissionValueEnum PermissionValue { get; set; }

        public long TeamId { get; set; }
        public Team Team { get; set; }
    }

    public enum PermissionTypeEnum
    { 
        //Session
        //Project
        Invite = 201,
        ManageParticipants = 202,
        ManagePermissions = 203,
        UndoCheckouts = 204,
        CreateSessions = 205,
        ShareItems = 206,
        FullControl = 207
    }

    public enum PermissionValueEnum 
    {
        Default = 0,
        Allow = 1,
        Deny = 2,
    }
}
