using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PrimeCollaborationManager.Models.Requests
{
    public class UpdateCollabPermissionsRequest
    {
        [Required]
        public string CollabId { get; set; }
        //Sessions
        public string SaveCopy { get; set; }
        public string PrintCopy { get; set; }
        public string Markup { get; set; }
        public string AddDocuments { get; set; }
        public string MarkupAlert { get; set; }
        //Projects
        public string UndoCheckouts { get; set; }
        public string CreateSessions { get; set; }
        public string ShareItems { get; set; }
        public string Invite { get; set; }
        public string ManageParticipants { get; set; }
        public string ManagePermissions { get; set; }
        public string FullControl { get; set; }
    }
}
