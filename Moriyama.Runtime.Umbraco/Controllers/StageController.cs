using System.Configuration;
using System.IO;
using System.Web.Mvc;
using Umbraco.Web.Mvc;

namespace Moriyama.Runtime.Umbraco.Controllers
{
    public class StageController : SurfaceController
    {
        public ActionResult Index()
        {
            var stage = ConfigurationManager.AppSettings["Moriyama.Runtime.StagePath"];

            var views = Path.Combine(stage, "Views");

            DeleteInFolder(views, "*.cshtml");
            var rootPath = Server.MapPath("/");

            Copy(Path.Combine(rootPath, "Views"), views);

            
            var paths = ConfigurationManager.AppSettings["Moriyama.Runtime.Folders"];


            foreach (var path in paths.Split(','))
            {
                var fullPath = Path.Combine(stage, path);
                DeleteInFolder(fullPath, "*.*");

                Copy(Path.Combine(rootPath, path), fullPath);
            }

            return Content("ok");
        }

        private void DeleteInFolder(string folder, string pattern)
        {
            if (!Directory.Exists(folder))
                return;

            var filePaths = Directory.GetFiles(folder, pattern, SearchOption.AllDirectories);

            foreach (var filePath in filePaths)
                System.IO.File.Delete(filePath);
        }

        private void Copy(string from, string to)
        {

            if (!Directory.Exists(from))
                return;

            if (!Directory.Exists(to))
                Directory.CreateDirectory(to);

            foreach (string dirPath in Directory.GetDirectories(from, "*", SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(from, to));

            foreach (string newPath in Directory.GetFiles(from, "*.*", SearchOption.AllDirectories))
            {
                var fi = new FileInfo(newPath);
                if(fi.Name.ToLower() != "web.config")
                    System.IO.File.Copy(newPath, newPath.Replace(from, to), true);
            }
        }
    }
}
