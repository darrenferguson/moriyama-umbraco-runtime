using System;
using System.Net;
using System.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Moriyama.Blog.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var token = "14bb580581001feaad834eec82673cf8369745f0";

            var url = "https://api-ssl.bitly.com/v3/shorten?access_token=14bb580581001feaad834eec82673cf8369745f0&longUrl=" +
                      HttpUtility.UrlEncode("http://blog.darren-ferguson.com/");

            var client = new WebClient();
            
            var result = client.DownloadString(url);
            var ob = JsonConvert.DeserializeObject<ShortUrl>(result);

            Console.WriteLine(JsonConvert.SerializeObject(ob, Formatting.Indented));
        }

        public class Data
        {
            public string long_url { get; set; }
            public string url { get; set; }
            public string hash { get; set; }
            public string global_hash { get; set; }
            public int new_hash { get; set; }
        }

        public class ShortUrl
        {
            public int status_code { get; set; }
            public string status_txt { get; set; }
            public Data data { get; set; }
        }

    }
}
