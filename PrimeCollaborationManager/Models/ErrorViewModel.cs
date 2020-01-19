using System;

namespace PrimeCollaborationManager.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }
        public Studio.Api.Client.StudioApiException StudioError { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
