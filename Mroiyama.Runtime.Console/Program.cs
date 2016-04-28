using Mroiyama.Runtime.Console.Classes;
using System;
using Moriyama.Runtime.Umbraco;
using UmbConsole;
using Umbraco.Core;
using Umbraco.Web;

namespace Moriyama.Runtime.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine("You should've seen by the look in my eyes, baby");

            var application = new ConsoleApplicationBase();
            application.Start(application, new EventArgs());

            //Write status for ApplicationContext
            var context = ApplicationContext.Current;


            System.Console.WriteLine("There was somethin missin");

            var contentFinder = new UmbracoContentFinder(context);
            RuntimeUmbracoContext.Instance.Init(@"c:\temp", null);
            var ids = contentFinder.GetAllUmbracoContentIds();
            foreach (var id in ids)
            {
                var content = context.Services.ContentService.GetById(id);
                System.Console.WriteLine(content.Name);
                var runtimeContentModel = RuntimeUmbracoContext.Instance.UmbracoContentSerialiser.Serialise(content);
                System.Console.WriteLine(runtimeContentModel.Name);
            }

        }
    }
}
