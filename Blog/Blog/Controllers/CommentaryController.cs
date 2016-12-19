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
    public class CommentaryController : Controller
    {
        // GET: Commentary
        public ActionResult Index()
        {
            return Index();
        }

        //
        // GET: Commentary/Create
        [Authorize]
        public ActionResult Create(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new BlogDbContext())
            {
                var model = new CommentaryViewModel();

                var article = database.Articles
                    .Where(a => a.Id == id)
                    .First();

                model.ArticleId = (int)id;
                model.ArticleTitle = article.Title;


                return View(model);
            }

        }

        //
        // POST: Commentary/Create
        [HttpPost]
        [Authorize]
        public ActionResult Create(CommentaryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var database = new BlogDbContext())
                {
                    var authorId = database.Users
                        .Where(u => u.UserName == this.User.Identity.Name)
                        .First()
                        .Id;

                    int points = 0;

                    model.AuthorName = database.Users
                        .Where(u => u.UserName == this.User.Identity.Name)
                        .First()
                        .FullName;

                    var comment = new Commentary(authorId, model.Content, points, model.ArticleId, model.AuthorName);

                    comment.AuhtorId = authorId;

                    database.Commentary.Add(comment);
                    database.SaveChanges();

                    return RedirectToAction("Details\\" + model.ArticleId, "Article");
                }
            }

            return View(model);
        }

    }
}