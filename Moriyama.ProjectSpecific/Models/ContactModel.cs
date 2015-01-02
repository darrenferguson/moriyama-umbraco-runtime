using System.ComponentModel.DataAnnotations;
using Moriyama.Runtime.Models;

namespace Moriyama.Blog.Project.Models
{
    public class ContactModel : RuntimeContentModel
    {
        [Required]
        public string ContactName { get; set; }

        [Required]
        [EmailAddress]
        public string ContactEmail { get; set; }
    }
}