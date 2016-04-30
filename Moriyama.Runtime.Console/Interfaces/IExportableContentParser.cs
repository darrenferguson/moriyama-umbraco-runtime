﻿using Moriyama.Content.Export.Application.Domain.Abstract;
using Moriyama.Content.Export.Application.Domain.Result;

namespace Moriyama.Content.Export.Interfaces
{
    public interface IExportableContentParser
    {
        string Name { get; }

        ParseResult ParseContent(BaseExportModel model);
        ParseResult ParseForImport(BaseExportModel model);
    }
}
