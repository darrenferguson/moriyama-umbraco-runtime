namespace Moriyama.Blog.Project.Models.Email
{
    public class CommentEmail : Postal.Email
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Comment { get; set; }
    }
}