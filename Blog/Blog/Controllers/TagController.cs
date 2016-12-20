using Blog.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Blog.Controllers
{
    public class TagController : Controller
    {
        //
        //GET: Tag
        public ActionResult List(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new BlogDbContext())
            {
                //Get articles from database 
                var articles = database.Tags
                    .Include(t => t.Articles.Select(a => a.Tags))
                    .Include(t => t.Articles.Select(a => a.Author))
                    .FirstOrDefault(t => t.Id == id)
                    .Articles
                    .ToList();
                //Return view
                return View(articles);

            }
        }

        private void SetArticleTags(Article article, ArticleViewModel model, BlogDbContext db)
        {
            var tagsStrings = model.Tags
                                .Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                .Select(t => t.ToLower())
                                .Distinct();

            //Set new article tags
            foreach (var tagString in tagsStrings)
            {
                //Get tag from db by its name
                Tag tag = db.Tags.FirstOrDefault(t => t.Name.Equals(tagString));
                //If the thag is null, create new tag
                if (tag == null)
                {
                    tag = new Tag() { Name = tagString };
                    db.Tags.Add(tag);
                }
                //Add tag to article tags
                article.Tags.Add(tag);
            }

        }
    }
}