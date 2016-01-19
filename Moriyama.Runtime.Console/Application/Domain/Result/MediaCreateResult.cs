using Umbraco.Core.Models;

namespace Moriyama.Content.Export.Application.Domain.Result
{
    public enum MediaCreateStatus
    {
        Success,
        Exists,
        Failed
    }

    public class MediaCreateResult
    {
        public MediaCreateStatus Status { get; set; }
        public IMedia Media { get; set; }
        public string Message { get; set; }
    }
}