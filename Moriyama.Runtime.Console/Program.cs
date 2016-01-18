//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Moriyama.Runtime.Console.Application;
//using Moriyama.Runtime.Console.Application.Domain;
//using Moriyama.Runtime.Console.Application.Parser;
//using Moriyama.Runtime.Console.Interfaces;
//using Newtonsoft.Json;
//using umbraco;
//using Umbraco.Core;

//namespace Moriyama.Runtime.Console
//{
//    class Program
//    {
//        enum OpMode {  Read, Write }

//        static void Main(string[] args)
//        {
//            var mode = OpMode.Write;

//            //Initialize the application
//            var application = new ConsoleApplicationBase();
//            application.Start(application, new EventArgs());
         
//            var context = ApplicationContext.Current;  
//            var contentService = context.Services.ContentService;
            
//            var finder = new UmbracoMediaFinder(contentService);

//            var allContent = finder.FindAllContent().ToArray();
//            var contentFactory = new ExportableContentFactory();

//            var fileSystem = new FileSystem(@"c:\temp\starter");       
//            var exportable = contentFactory.GetExportableContent(allContent).ToArray();

//            if (mode == OpMode.Read)
//            {
//                var allExportedContent = new List<ExportContentModel>();
//                var creator = new UmbracoContentCreator(exportable, contentService, contentFactory);

//                foreach (var file in fileSystem.List())
//                {
//                    var item = JsonConvert.DeserializeObject<ExportContentModel>(System.IO.File.ReadAllText(file));
//                    allExportedContent.Add(item);
//                }

//                foreach (var contentItem in allExportedContent.OrderBy(x => x.Level).ThenBy(x => x.SortOrder))
//                {
//                    var result = creator.Create(contentItem);

//                    System.Console.WriteLine(result.Status + " -> " + contentItem.Name);

//                }

//                return;
//            }

//            var parsers = new List<IExportContentParser>
//            {
//                new NullValueExportContentParser(),
//                new CommaDelimitedIntExportContentParser(exportable),
//                new IntExportContentParser(exportable)
//            };

//            var serialiser = new UmbracoMediaExportSerialiser(parsers);

//            foreach (var export in exportable)
//            {
//                var json = JsonConvert.SerializeObject(serialiser.Serialise(export), Formatting.Indented);
//                fileSystem.Write(export.Path + ".json", json);
//            }
//        }
//    }
//}
