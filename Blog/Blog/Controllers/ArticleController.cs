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
    public class ArticleController : Controller
    {
        //
        // GET: Article
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        //
        //GET: Article/List
        public ActionResult List()
        {
            using (var database = new BlogDbContext())
            {
                var categories = database
                    .Categories
                    .Include(c => c.Articles)
                    .OrderBy(c => c.Name)
                    .ToList();

                return View(categories);
            }
        }

        //
        //GET: Article/ListByCategory
        public ActionResult ListByCategory(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                using (var database = new BlogDbContext())
                {
                    var model = new CategoryViewModel();

                    var category = database
                                     .Categories
                                     .Where(c => c.Id == id)
                                     .First();

                    model.Name = category.Name;

                    model.Articles = database
                                     .Articles
                                     .Where(a => a.CategoryId == id)
                                     .Include(a => a.Author)
                                     .Include(a => a.Tags)
                                     .ToList();

                    model.Categories = database
                        .Categories
                        .Include(c => c.Articles)
                        .OrderBy(c => c.Name)
                        .ToList();

                    return View(model);

                }
            }


        }

        //
        //GET: Article/Details
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new BlogDbContext())
            {
                var article = database.Articles
                    .Where(a => a.Id == id)
                    .Include(a => a.Author)
                    .Include(a => a.Tags)
                    .Include(a => a.Commentaries)
                    .First();

                if (article == null)
                {
                    return HttpNotFound();
                }

                return View(article);
            }

        }

        //
        //GET: Article/Create
        [Authorize]
        public ActionResult Create()
        {
            using (var database = new BlogDbContext())
            {
                var model = new ArticleViewModel();
                model.Categories = database.Categories
                    .OrderBy(c => c.Name)
                    .ToList();

                return View(model);
            }
        }

        //
        //POST: Article/Create
        [HttpPost]
        [Authorize]
        public ActionResult Create(ArticleViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var database = new BlogDbContext())
                {
                    //Get author id
                    var authorId = database.Users
                        .Where(u => u.UserName == this.User.Identity.Name)
                        .First()
                        .Id;

                    var article = new Article(authorId, model.Title, model.Content, model.CategoryId);

                    this.SetArticleTags(article, model, database);

                    //Set article author
                    article.AuthorId = authorId;

                    //Save articles in DB
                    database.Articles.Add(article);
                    database.SaveChanges();

                    return RedirectToAction("Index");
                }
            }
            return View(model);
        }


        //
        // Get: Article/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new BlogDbContext())
            {
                //Get article from database
                var article = database.Articles
                    .Where(a => a.Id == id)
                    .Include(a => a.Author)
                    .Include(a => a.Category)
                    .First();

                //Validation
                if (!IsUserAuthorizedToEdit(article))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }

                ViewBag.TagsString = string.Join(", ", article.Tags.Select(t => t.Name));

                //Check if article exists
                if (article == null)
                {
                    return HttpNotFound();
                }

                // Pass article to the view
                return View(article);
            }
        }

        //
        //POST: Article/Delete
        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new BlogDbContext())
            {
                //Get the article from the database
                var article = database.Articles
                    .Where(a => a.Id == id)
                    .Include(a => a.Author)
                    .First();

                //Check if the article exists
                if (article == null)
                {
                    return HttpNotFound();
                }

                //Delete the article from the database
                database.Articles.Remove(article);
                database.SaveChanges();

                //Redirect to the index page
                return RedirectToAction("Index");

            }
        }

        //
        //GET: Article/Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new BlogDbContext())
            {
                //Get the article from the database 
                var article = database.Articles
                    .Where(a => a.Id == id)
                    .Include(a => a.Author)
                    .First();

                //Validation
                if (!IsUserAuthorizedToEdit(article))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }

                //Check if the article exists
                if (article == null)
                {
                    return HttpNotFound();
                }
                //Create the view model
                var model = new ArticleViewModel();
                model.Id = article.Id;
                model.Title = article.Title;
                model.Content = article.Content;
                model.CategoryId = article.CategoryId;
                model.Categories = database.Categories
                    .OrderBy(c => c.Name)
                    .ToList();

                model.Tags = string.Join(", ", article.Tags.Select(t => t.Name));

                //Pass the view model to the view
                return View(model);
            }
        }

        //
        //POST: Article/Edit
        [HttpPost]
        public ActionResult Edit(ArticleViewModel model)
        {
            //Check if model state is valid
            if (ModelState.IsValid)
            {
                using (var database = new BlogDbContext())
                {
                    //Get the article from the database
                    var article = database.Articles
                        .FirstOrDefault(a => a.Id == model.Id);
                    //Set the article properties
                    article.Title = model.Title;
                    article.Content = model.Content;
                    article.CategoryId = model.CategoryId;
                    this.SetArticleTags(article, model, database);
                    //Save the article state in the database
                    database.Entry(article).State = EntityState.Modified;
                    database.SaveChanges();
                    //Redirect to the index page
                    return RedirectToAction("Index");
                }
            }

            return View(model);
        }

        private bool IsUserAuthorizedToEdit(Article article)
        {
            bool isAdmin = this.User.IsInRole("Admin");
            bool isAuthor = article.IsAuthor(this.User.Identity.Name);

            return isAdmin || isAuthor;
        }

        private void SetArticleTags(Article article, ArticleViewModel model, BlogDbContext db)
        {
            //Split tags
            var tagsStrings = model.Tags
                .Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.ToLower())
                .Distinct();
            //Clear current article tags
            article.Tags.Clear();

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