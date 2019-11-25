using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrimeCollaborationManager.Models
{
    public class User
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string RestrictedStatus { get; set; }
        public int Id { get; set; }
    }
}
