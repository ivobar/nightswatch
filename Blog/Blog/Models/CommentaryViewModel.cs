using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Blog.Models
{
    public class CommentaryViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Content { get; set; }

        public string AuthorId { get; set; }

        public string AuthorName { get; set; }

        public int ArticleId { get; set; }

        public string ArticleTitle { get; set; }
    }
}