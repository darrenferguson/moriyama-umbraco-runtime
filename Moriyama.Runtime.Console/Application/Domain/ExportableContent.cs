﻿using Moriyama.Content.Export.Interfaces.Domain;
using Umbraco.Core.Models;

namespace Moriyama.Content.Export.Application.Domain
{
    public class ExportableContent : IExportableItem<IContent>
    {
        public string Path { get; set; }
        public IContent Content { get; set; }
    }
}
