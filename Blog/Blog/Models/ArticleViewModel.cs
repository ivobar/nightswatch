using Blog.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Blog.Models
{
    public class ArticleViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        public string AuthorId { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public ICollection<Category> Categories { get; set; }

        public ICollection<Commentary> Commentaries { get; set; }

        public ICollection<Tag> TagsCollection { get; set; }

        [Required]
        public string Tags { get; set; }

        public int SearchType { get; set; }

        public string SearchText { get; set; }


    }
}