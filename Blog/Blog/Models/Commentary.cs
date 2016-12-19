using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Blog.Models
{
    public class Commentary
    {
        public Commentary()
        {

        }

        public Commentary(string authorId, string content, int points, int articleId, string authorName)
        {
            this.AuhtorId = authorId;
            this.Content = content;
            this.Points = points;
            this.ArticleId = articleId;
            this.AuthorName = authorName;
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Content { get; set; }

        [ForeignKey("Author")]
        public string AuhtorId { get; set; }

        public string AuthorName { get; set; }

        public virtual ApplicationUser Author { get; set; }

        public int Points { get; set; }

        [ForeignKey("Article")]
        public int ArticleId { get; set; }

        public virtual Article Article { get; set; }
    }
}