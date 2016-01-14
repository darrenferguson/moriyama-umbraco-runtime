using System;
using System.Collections.Generic;

namespace Moriyama.Runtime.Console.Application.Domain
{
    public class ExportContentModel
    {
        public string Name { get; set; }
        public string Type { get; set; }

        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

        public string CreatorName { get; set; }
        public string WriterName { get; set; }

        public string Path { get; set; }

        public IDictionary<string, object> Content { get; set; }

        public string Template { get; set; }
        
        public int SortOrder { get; set; }
        public int Level { get; set; }
    }
}
