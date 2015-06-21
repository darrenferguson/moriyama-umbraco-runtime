using System;
using System.Configuration;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using log4net;
using Newtonsoft.Json;

namespace Moriyama.Blog.Project.Controllers
{
    public class SocialController : Controller
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        [HttpGet]
        public ActionResult Share(string url, string name, string network)
        {

            url = HttpUtility.UrlEncode(Shorten(url));
            name = HttpUtility.UrlEncode(name);

            network = network.ToLower();
            
            string targetUrl;

            switch (network)
            {
                case "googleplus":
                    targetUrl = "https://plus.google.com/share?url=" + url;
                    break;
                case "facebook":
                    targetUrl = "http://www.facebook.com/sharer/sharer.php?u=" + url;
                    break;
                case "linkedin":
                    targetUrl = "http://www.linkedin.com/shareArticle?mini=true&url=" + url + "&title=" + name;
                    break;
                default:
                    targetUrl = "https://twitter.com/intent/tweet?url=" + url + "&text=" + name;
                    break;
            }
            
            return Redirect(targetUrl);
        }

        private string Shorten(string url)
        {
            var token = ConfigurationManager.AppSettings["BitlyKey"];

            var shortenUrl = "https://api-ssl.bitly.com/v3/shorten?access_token="+token+"&longUrl=" +
                    HttpUtility.UrlEncode(url);

            try
            {
                var client = new WebClient();
                var result = client.DownloadString(shortenUrl);
                var ob = JsonConvert.DeserializeObject<ShortUrl>(result);
                url = ob.data.url;
            }
            catch (Exception ex)
            {
                Logger.Warn(ex);
            }

            return url;
        }

        private class Data
        {
            public string long_url { get; set; }
            public string url { get; set; }
            public string hash { get; set; }
            public string global_hash { get; set; }
            public int new_hash { get; set; }
        }

        private class ShortUrl
        {
            public int status_code { get; set; }
            public string status_txt { get; set; }
            public Data data { get; set; }
        }
    }
}