using Umbraco.Core.Models;

namespace Moriyama.Content.Export.Application.Domain
{
    public enum ContentCreateStatus
    {
        Success,
        Exists,
        Failed
    }

    public class ContentCreateResult
    {
        public ContentCreateStatus Status { get; set; }
        public IContent Content { get; set; }
        public string Message { get; set; }
    }
}