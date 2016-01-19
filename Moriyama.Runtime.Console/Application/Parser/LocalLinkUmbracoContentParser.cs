using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Moriyama.Content.Export.Application.Domain;
using Moriyama.Content.Export.Application.Domain.Abstract;
using Moriyama.Content.Export.Application.Domain.Result;
using Moriyama.Content.Export.Interfaces;

namespace Moriyama.Content.Export.Application.Parser
{
    public class LocalLinkExportContentParser : IExportContentParser
    {
         private readonly IEnumerable<ExportableContent> _allContent;
         private readonly IEnumerable<ExportableMedia> _allMedia;

         public LocalLinkExportContentParser(IEnumerable<ExportableContent> allContent, IEnumerable<ExportableMedia> allMedia)
         {
             _allContent = allContent;
             _allMedia = allMedia;
         }

        public string Name { get { return "LocalLink"; } }

        public ParseResult ParseForImport(BaseExportModel model)
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
            
            return new ParseResult
            {
                Content = newContent,
                Meta = model.Meta
            };
        }

        public ParseResult ParseContent(BaseExportModel model)
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


            return new ParseResult
            {
                Content = newContent,
                Meta = model.Meta
            };
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
                
                var media = _allMedia.FirstOrDefault(v => v.Content.Id == intId);
                var content = _allContent.FirstOrDefault(x => x.Content.Id == intId);

                if(content != null) 
                    return content.Path;

                if (media != null)
                    return media.Path;
            }
            catch (Exception ex)
            {
            }

            return "#";
        }
    }
}
