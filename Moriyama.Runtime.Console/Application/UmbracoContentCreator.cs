using System;
using System.Collections.Generic;
using System.Linq;
using Moriyama.Content.Export.Application.Domain;
using Moriyama.Content.Export.Interfaces;
using Umbraco.Core.Models;
using Umbraco.Core.Services;

namespace Moriyama.Content.Export.Application
{
    class UmbracoContentCreator : IUmbracoContentCreator
    {
        private IList<ExportableContent> _allContent;


        private readonly IContentService _contentService;
        private readonly IExportableContentFactory _contentFactory;

        public UmbracoContentCreator(IEnumerable<ExportableContent> allContent, IContentService contentService, IExportableContentFactory contentFactory)
        {
            _allContent = allContent.ToList();
            _contentService = contentService;
            _contentFactory = contentFactory;
        }


        public IEnumerable<ExportableContent> GetAllContent()
        {
            return _allContent;
        }

        public ContentCreateResult Create(ExportContentModel content)
        {
            var existingContent = _allContent.FirstOrDefault(x => x.Path == content.Path);

            if (existingContent != null)
            {
                return new ContentCreateResult
                {
                    Status = ContentCreateStatus.Exists,
                    Content = existingContent.Content
                };
            }

            var parentId = -1;
            
            if (content.Level > 1)
            {

                var parentPath = content.Path.Substring(0, content.Path.LastIndexOf("/"));

                var parent = _allContent.FirstOrDefault(x => x.Content.Level == content.Level - 1 && x.Path == parentPath);

                if (parent == null)
                {
                    return new ContentCreateResult
                    {
                        Status = ContentCreateStatus.Failed,
                        Message = content.Name + " -> " + "No parent"
                    };
                }

                parentId = parent.Content.Id;
            }

            IContent createdContent = null;

            try
            {
                createdContent = _contentService.CreateContentWithIdentity(content.Name, parentId, content.Type);
            }
            catch (Exception ex)
            {
                return new ContentCreateResult
                {
                    Status = ContentCreateStatus.Failed,
                    Message = content.Name + " -> " + ex.Message
                };
            }
           
            var exportable = new ExportableContent
            {
                Content = createdContent,
                Path = _contentFactory.GetPath(createdContent, _allContent.Select(x => x.Content))
            };

            _allContent.Add(exportable);

            return new ContentCreateResult
            {
                Status = ContentCreateStatus.Success,
                Content = createdContent
            };
            
        }
    }
}
