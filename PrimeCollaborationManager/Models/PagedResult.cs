using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrimeCollaborationManager.Models
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; }
        public long TotalItems { get; set; }
        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; }

        public PagedResultDisplay GetModelForDisplay(string controllerAction)
        {
            return new PagedResultDisplay()
            {
                TotalItems = this.TotalItems,
                CurrentPage = this.CurrentPage,
                ItemsPerPage = this.ItemsPerPage,
                ControllerAction = controllerAction,
                ActionParams = new Dictionary<string, string>()
            };
        }
    }

    public class PagedResultDisplay
    {
        public long TotalItems { get; set; }
        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; }
        public string ControllerAction { get; set; }
        public Dictionary<string, string> ActionParams { get; set; }
    }
}
