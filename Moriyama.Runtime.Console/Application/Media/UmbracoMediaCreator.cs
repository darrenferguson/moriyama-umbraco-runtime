using System;
using System.Collections.Generic;
using System.Linq;
using Moriyama.Content.Export.Application.Domain;
using Moriyama.Content.Export.Application.Domain.Result;
using Moriyama.Content.Export.Interfaces;
using Moriyama.Content.Export.Interfaces.Media;
using Umbraco.Core.Models;
using Umbraco.Core.Services;

namespace Moriyama.Content.Export.Application.Media
{
    class UmbracoMediaCreator : IUmbracoMediaCreator
    {
        private readonly IList<ExportableMedia> _allContent;


        private readonly IMediaService _contentService;
        private readonly IExportableContentFactory<ExportableMedia, IMedia> _contentFactory;

        public UmbracoMediaCreator(IEnumerable<ExportableMedia> allContent, IMediaService contentService, IExportableContentFactory<ExportableMedia, IMedia> contentFactory)
        {
            _allContent = allContent.ToList();
            _contentService = contentService;
            _contentFactory = contentFactory;
        }

        public IEnumerable<ExportableMedia> GetAllContent()
        {
            return _allContent;
        }

        public MediaCreateResult Create(ExportMediaModel content)
        {
            var existingContent = _allContent.FirstOrDefault(x => x.Path == content.Path);

            if (existingContent != null)
            {
                return new MediaCreateResult
                {
                    Status = MediaCreateStatus.Exists,
                    Media = existingContent.Content
                };
            }

            var parentId = -1;
            
            if (content.Level > 1)
            {

                var parentPath = content.Path.Substring(0, content.Path.LastIndexOf("/"));

                var parent = _allContent.FirstOrDefault(x => x.Content.Level == content.Level - 1 && x.Path == parentPath);

                if (parent == null)
                {
                    return new MediaCreateResult
                    {
                        Status = MediaCreateStatus.Failed,
                        Message = content.Name + " -> " + "No parent"
                    };
                }

                parentId = parent.Content.Id;
            }

            IMedia createdContent = null;

            try
            {
                createdContent = _contentService.CreateMediaWithIdentity(content.Name, parentId, content.Type);
            }
            catch (Exception ex)
            {
                return new MediaCreateResult
                {
                    Status = MediaCreateStatus.Failed,
                    Message = content.Name + " -> " + ex.Message
                };
            }
           
            var exportable = new ExportableMedia()
            {
                 
                Path = _contentFactory.GetPath(createdContent, _allContent.Select(x => x.Content)),
                Content =  createdContent
            };

            _allContent.Add(exportable);

            return new MediaCreateResult
            {
                Status = MediaCreateStatus.Success,
                Media = createdContent
            };
        }
    }
}
