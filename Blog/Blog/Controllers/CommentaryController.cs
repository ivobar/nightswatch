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

        //
        // GET: Commentary/VoteUp
        public ActionResult VoteUp(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new BlogDbContext())
            {
                if (Request.IsAuthenticated)
                {

                }
            }
            return RedirectToAction("Details\\"+1, "Article");
        }

        //
        // GET: Commentary/Edit
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id==null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new BlogDbContext())
            {
                var comment = database.Commentary
                    .Where(c => c.Id == id)
                    .First();

                //Validation
                if (!IsUserAuthorizedToEditComment(comment))
                {
                    return View("Forbidden");
                }

                return View(comment);
            }
        }

        //
        // POST: Commentary/Edit
        [HttpPost]
        [Authorize]
        public ActionResult Edit(CommentaryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var database = new BlogDbContext())
                {
                    var comment = database.Commentary
                        .Where(c => c.Id == model.Id)
                        .FirstOrDefault();

                    comment.Content = model.Content;

                    database.Entry(comment).State = EntityState.Modified;
                    database.SaveChanges();

                    return RedirectToAction("Details\\"+ model.ArticleId, "Article");
                }
            }

            return View(model);
        }

        //
        // GET: Commentary/Delete
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new BlogDbContext())
            {
                var comment = database.Commentary
                    .Where(c => c.Id == id)
                    .First();

                //Validation
                if (!IsUserAuthorizedToEditComment(comment))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }

                return View(comment);
            }
        }

        //
        // POST: Commentary/Delete
        [HttpPost]
        [Authorize]
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed (int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new BlogDbContext())
            {
                var comment = database.Commentary
                    .FirstOrDefault(c => c.Id == id);

                if (comment == null)
                {
                    return HttpNotFound();
                }

                database.Commentary.Remove(comment);
                database.SaveChanges();

                return RedirectToAction("Details\\" + comment.ArticleId, "Article");
            }
        }

        private bool IsUserAuthorizedToEditComment(Commentary comment)
        {
            bool isAdmin = this.User.IsInRole("Admin");
            bool isAuthor = comment.IsAuthorOfComment(this.User.Identity.Name);

            return isAdmin || isAuthor;
        }

    }
}