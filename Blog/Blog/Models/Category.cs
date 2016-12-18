using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Blog.Models
{
    public class Category
    {
        private ICollection<Article> articles;

        public Category()
        {
            this.articles = new HashSet<Article>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        [Index(IsUnique =true)]
        public string Name { get; set; }

        public virtual ICollection<Article> Articles { get; set; }
    }
}