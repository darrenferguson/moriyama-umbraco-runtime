using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Moriyama.Content.Export.Application.Domain;
using Moriyama.Content.Export.Interfaces;

namespace Moriyama.Content.Export.Application.Parser
{
    public class LocalLinkExportContentParser : IExportContentParser
    {
         private readonly IEnumerable<ExportableContent> _allContent;

         public LocalLinkExportContentParser(IEnumerable<ExportableContent> allContent)
        {
            _allContent = allContent;
        }

        public string Name { get { return "LocalLink"; } }

        public ExportContentModel ParseForImport(ExportContentModel model)
        {
            var newContent = model.Content.ToDictionary(entry => entry.Key, entry => entry.Value);

            foreach (var property in model.Content)
            {
                if (model.Meta.ContainsKey(property.Key) && (model.Meta[property.Key] == Name))
                {
                    var localLinkRegex = new Regex(@"\{localLink\:(.*?)\}",
                        RegexOptions.Compiled | RegexOptions.Multiline);
                    var value = (string) property.Value;

                    foreach (Match match in localLinkRegex.Matches(value))
                    {
                        var replacement = match.Value;
                        var path = match.Groups[1].Value;

                        var id = IdForPathLink(path);
                        value = value.Replace(replacement, "/{localLink:" +id + "}");
                    }
                    newContent[property.Key] = value;
                }
            }
            model.Content = newContent;
            return model;
        }

        public ExportContentModel ParseContent(ExportContentModel model)
        {
            var newContent = model.Content.ToDictionary(entry => entry.Key, entry => entry.Value);

            foreach (var property in model.Content)
            {
                if (property.Value is string && ((string)property.Value).Contains("{localLink:"))
                {
                    var localLinkRegex = new Regex(@"\/\{localLink\:(.*?)\}", RegexOptions.Compiled | RegexOptions.Multiline);
                    var value = (string)property.Value;

                    foreach (Match match in localLinkRegex.Matches(value))
                    {
                        var replacement = match.Value;
                        var docId = match.Groups[1].Value;
                        value = value.Replace(replacement, "{localLink:" + UrlForLocalLink(docId) + "}");
                    }

                    newContent[property.Key] = value;
                    model.Meta.Add(property.Key, "LocalLink");
                }
            }

            model.Content = newContent;
            return model;
        }


        private int IdForPathLink(string path)
        {
            try
            {
                var content = _allContent.FirstOrDefault(x => x.Path == path);
                return content.Content.Id;
            }
            catch (Exception ex)
            {
            }
            return -1;
        }

        private string UrlForLocalLink(string id)
        {
            try
            {
                var intId = Convert.ToInt32(id);
                var content = _allContent.FirstOrDefault(x => x.Content.Id == intId);
                return content.Path;
            }
            catch (Exception ex)
            {
            }
            return "#";
        }
    }
}
