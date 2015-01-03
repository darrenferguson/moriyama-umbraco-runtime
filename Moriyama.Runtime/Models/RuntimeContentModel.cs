using System;
using System.Collections.Generic;

namespace Moriyama.Runtime.Models
{
    public class RuntimeContentModel
    {
        public string Name { get; set; }
        public string Type { get; set; }

        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

        public string CreatorName { get; set; }
        public string WriterName { get; set; }

        public string Url { get; set; }
        public string RelativeUrl { get; set; }
        
        public IDictionary<string, object> Content { get; set; }

        public string Template { get; set; }
        
        public DateTime? CacheTime { get; set; }


        public int SortOrder { get; set; }
        public int Level { get; set; }
    }
}
