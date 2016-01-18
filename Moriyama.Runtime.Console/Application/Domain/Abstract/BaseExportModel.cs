using System;
using System.Collections.Generic;

namespace Moriyama.Content.Export.Application.Domain.Abstract
{
    public abstract class BaseExportModel
    {
        public string Name { get; set; }
        public string Type { get; set; }

        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

        public string CreatorName { get; set; }
        public string WriterName { get; set; }

        public string Path { get; set; }

        public IDictionary<string, object> Content { get; set; }
        public IDictionary<string, string> Meta { get; set; }


        public int SortOrder { get; set; }
        public int Level { get; set; }
    }
}
