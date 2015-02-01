using System;
using System.IO;
using System.Net;
using System.Web;

namespace Moriyama.Blog.Project.Application
{

    public class AkismetComment
    {
        public string Blog = null;
        public string UserIp = null;
        public string UserAgent = null;
        public string Referrer = null;
        public string Permalink = null;
        public string CommentType = null;
        public string CommentAuthor = null;
        public string CommentAuthorEmail = null;
        public string CommentAuthorUrl = null;
        public string CommentContent = null;
    }

    public class Akismet
    {
        const string VerifyUrl = "http://rest.akismet.com/1.1/verify-key";
        const string CommentCheckUrl = "http://{0}.rest.akismet.com/1.1/comment-check";
        const string SubmitSpamUrl = "http://{0}.rest.akismet.com/1.1/submit-spam";
        const string SubmitHamUrl = "http://{0}.rest.akismet.com/1.1/submit-ham";

        readonly string _apiKey;
        readonly string _userAgent = "Joel.Net's Akismet API/1.0";
        readonly string _blog;

        public string CharSet = "UTF-8";

        /// <summary>Creates an Akismet API object.</summary>
        /// <param name="apiKey">Your wordpress.com API key.</param>
        /// <param name="blog">URL to your blog</param>
        /// <param name="userAgent">Name of application using API.  example: "Joel.Net's Akismet API/1.0"</param>
        /// <remarks>Accepts required fields 'apiKey', 'blog', 'userAgent'.</remarks>
        public Akismet(string apiKey, string blog, string userAgent)
        {
            _apiKey = apiKey;
            if (userAgent != null) _userAgent = userAgent + " | Akismet/1.11";
            _blog = blog;
        }

        /// <summary>Verifies your wordpress.com key.</summary>
        /// <returns>'True' is key is valid.</returns>
        public bool VerifyKey()
        {
            var response = HttpPost(VerifyUrl, String.Format("key={0}&blog={1}", _apiKey, HttpUtility.UrlEncode(_blog)), CharSet);
            return (response == "valid");
        }

        /// <summary>Checks AkismetComment object against Akismet database.</summary>
        /// <param name="comment">AkismetComment object to check.</param>
        /// <returns>'True' if spam, 'False' if not spam.</returns>
        public bool CommentCheck(AkismetComment comment)
        {
            return Convert.ToBoolean(HttpPost(String.Format(CommentCheckUrl, _apiKey), CreateData(comment), CharSet));
        }

        /// <summary>Submits AkismetComment object into Akismet database.</summary>
        /// <param name="comment">AkismetComment object to submit.</param>
        public void SubmitSpam(AkismetComment comment)
        {
            HttpPost(String.Format(SubmitSpamUrl, _apiKey), CreateData(comment), CharSet);
        }

        /// <summary>Retracts false positive from Akismet database.</summary>
        /// <param name="comment">AkismetComment object to retract.</param>
        public void SubmitHam(AkismetComment comment)
        {
            HttpPost(String.Format(SubmitHamUrl, _apiKey), CreateData(comment), CharSet);
        }




        /// <summary>Takes an AkismetComment object and returns an (escaped) string of data to POST.</summary>
        /// <param name="comment">AkismetComment object to translate.</param>
        /// <returns>A System.String containing the data to POST to Akismet API.</returns>
        private string CreateData(AkismetComment comment)
        {
            string value = String.Format("blog={0}&user_ip={1}&user_agent={2}&referrer={3}&permalink={4}&comment_type={5}" +
                "&comment_author={6}&comment_author_email={7}&comment_author_url={8}&comment_content={9}",
                HttpUtility.UrlEncode(comment.Blog),
                HttpUtility.UrlEncode(comment.UserIp),
                HttpUtility.UrlEncode(comment.UserAgent),
                HttpUtility.UrlEncode(comment.Referrer),
                HttpUtility.UrlEncode(comment.Permalink),
                HttpUtility.UrlEncode(comment.CommentType),
                HttpUtility.UrlEncode(comment.CommentAuthor),
                HttpUtility.UrlEncode(comment.CommentAuthorEmail),
                HttpUtility.UrlEncode(comment.CommentAuthorUrl),
                HttpUtility.UrlEncode(comment.CommentContent)
            );

            return value;
        }

        /// <summary>Sends HttpPost</summary>
        /// <param name="url">URL to Post data to.</param>
        /// <param name="data">Data to post. example: key=&ltwordpress-key&gt;&blog=http://joel.net</param>
        /// <param name="charSet">Character set of your blog. example: UTF-8</param>
        /// <returns>A System.String containing the Http Response.</returns>
        private string HttpPost(string url, string data, string charSet)
        {
            // Initialize Connection
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded; charset=" + charSet;
            request.UserAgent = _userAgent;
            request.ContentLength = data.Length;


            // Write Data
            var writer = new StreamWriter(request.GetRequestStream());
            writer.Write(data);
            writer.Close();

            // Read Response
            var response = (HttpWebResponse)request.GetResponse();
            var reader = new StreamReader(response.GetResponseStream());

            var value = reader.ReadToEnd();
            reader.Close();

            return value;
        }

    }
}