using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrimeCollaborationManager.Models
{
    public class PermissionsSettingsList
    {
        public List<PermissionSetting> Permissions { get; set; }
    }

    public class PermissionSetting
    {
        public string Type { get; set; }
        public bool? Allow { get; set; }
    }
}
