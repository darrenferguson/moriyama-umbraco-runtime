using System.Collections.Generic;

namespace Moriyama.Content.Export.Application.Domain.Result
{
    public class ParseResult
    {
        public IDictionary<string, object> Content { get; set; }
        public IDictionary<string, string> Meta { get; set; }
    }
}
