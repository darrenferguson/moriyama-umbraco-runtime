using Moriyama.Runtime.Models;

namespace Moriyama.Blog.Project.Models
{
    public class CommentModel : RuntimeContentModel
    {
        public string CommentName { get; set; }
        public string CommentEmail { get; set; }
        public string CommentMessage { get; set; }

        public string Moriyama { get; set; }
    }
}