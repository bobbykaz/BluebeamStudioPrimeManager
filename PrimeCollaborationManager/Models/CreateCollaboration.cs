using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PrimeCollaborationManager.Models
{
    public class CreateCollaboration
    {
        [Required]
        [MinLength(1)]
        public string Name { get; set; }
        public bool Notification { get; set; }
        public bool Restricted { get; set; }
        public List<string> InitialPermissionTypes { get; set; }
    }
}
