using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using News.Models;
using System.Net;
using System.Data.Entity;
using PagedList;
using System.Globalization;
using System.IO;
using System.Web.Helpers;

namespace News.Controllers
{
    public class ArticlesController : Controller
    {
        private NewsModel db = new NewsModel();

        // GET: Articles
        public ActionResult Index()
        {
            var articlesInOrder = db.Articles.OrderByDescending(a => a.PublishedAt);
            
            ViewBag.Lead = articlesInOrder.Where(a => a.isLead).FirstOrDefault();

            if (ViewBag.Lead != null)
            {
                ViewBag.LeadImage = GetMainArticleImageId(ViewBag.Lead.Id);
            }

            var articles = articlesInOrder.Where(a => a.isLead == false).Take(10).ToList();

            return View(articles);
        }

        public FileResult Image(Int32? imageId, Boolean isLarge = false)
        {
            Byte[] image = GetArticleImage(imageId, isLarge);

            if (image == null) // nem sikerült betölteni a képet
                return File("~/Content/missing-image.png", "image/png");

            return File(image, "image/png");
        }

        // GET: Articles/Archive
        public ActionResult Archive(string searchString, int? page)
        {
            var articlesInOrder = db.Articles.OrderByDescending(a => a.PublishedAt);
            int pageNumber = (page ?? 1);
            int pageSize = 20;

            if (!String.IsNullOrEmpty(searchString))
            {
                page = 1;
                DateTime searchDate;

                if (DateTime.TryParseExact(searchString, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out searchDate) == true)
                {
                    var articles = articlesInOrder.Where(a => a.Title.Contains(searchString)
                        || a.Summary.Contains(searchString)
                        || a.Content.Contains(searchString)
                        || DbFunctions.TruncateTime(a.PublishedAt) == searchDate.Date);

                    return View(articles.ToPagedList(pageNumber, pageSize));
                }
                else
                {
                    var articles = articlesInOrder.Where(a => a.Title.Contains(searchString)
                        || a.Summary.Contains(searchString)
                        || a.Content.Contains(searchString));

                    return View(articles.ToPagedList(pageNumber, pageSize));
                }
            }

            return View(articlesInOrder.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult MyArticles()
        {
            var username = System.Web.HttpContext.Current.User.Identity.Name;

            if (username == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return View(db.Articles.Where(a => a.Author == username).OrderByDescending(a => a.PublishedAt).ToList());
        }

        // GET: Articles/Create
        public ActionResult Create()
        {

            return View(new Article());
        }

        // POST: Articles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Author,Summary,Content,isLead,PublishedAt")] Article item)
        {
            item.PublishedAt = DateTime.Now;

            if (ModelState.IsValid)
            {
                db.Articles.Add(item);

                WebImage img = WebImage.GetImageFromRequest();

                if (img != null)
                {
                    Image image = new Image();
                    image.Article = item;
                    image.ImageLarge = img.GetBytes();
                    image.ImageSmall = img.Resize(img.Width / 2, img.Height / 2).GetBytes();

                    db.Images.Add(image);
                }

                if (item.isLead)
                {
                    ChangeLeadArticle(item.Id);
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(item);
        }

        // GET Articles/Read/{id}
        public ActionResult Read(Int32? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Article item = db.Articles.Find(id);

            if (item == null)
            {
                return HttpNotFound();
            }

            ViewBag.Image = GetMainArticleImageId(item.Id);

            return View(item);
        }

        // GET: Articles/Images/{articleId}
        public ActionResult Images(Int32? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Article item = db.Articles.Find(id);

            if (item == null)
            {
                return HttpNotFound();
            }

            ViewBag.Images = GetArticleImageIds(item.Id);
            ViewBag.ArticleId = item.Id;

            return View();
        }

        // GET: Articles/Edit/{id}
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Article item = db.Articles.Find(id);

            if (item == null)
            {
                return HttpNotFound();
            }

            return View(item);
        }

        // POST: Articles/Edit/{id}
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Author,Summary,Content,isLead,PublishedAt")] Article item)
        {
            item.PublishedAt = DateTime.Now;

            if (ModelState.IsValid)
            {
                db.Entry(item).State = EntityState.Modified;

                WebImage img = WebImage.GetImageFromRequest();

                if (img != null)
                {
                    Image image = new Image();
                    image.Article = item;
                    image.ImageLarge = img.GetBytes();
                    image.ImageSmall = img.Resize(img.Width / 2, img.Height / 2).GetBytes();

                    db.Images.Add(image);
                }

                if (item.isLead)
                {
                    ChangeLeadArticle(item.Id);
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(item);
        }

        // GET: Articles/Delete/{id}
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Article item = db.Articles.Find(id);

            if (item == null)
            {
                return HttpNotFound();
            }

            return View(item);
        }

        // POST: Articles/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Article item = db.Articles.Find(id);
            db.Articles.Remove(item);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private Int32[] GetArticleImageIds(Int32 articleId)
        {
            return db.Images.Where(image => image.ArticleId == articleId).Select(image => image.Id).ToArray();
        }

        private Int32? GetMainArticleImageId(Int32 articleId)
        {
            Image img = db.Images.Where(image => image.ArticleId == articleId).FirstOrDefault();

            if (img == null)
                return null;

            return img.Id;
        }

        private Byte[] GetArticleImage(Int32? imageId, Boolean large)
        {
            Image image = db.Images.FirstOrDefault(img => img.Id == imageId);

            if (image == null)
                return null;

            if (large)
                return image.ImageLarge;
            else
                return image.ImageSmall;
        }

        private void ChangeLeadArticle(Int32 ArticleId)
        {
            var articles = db.Articles.Where(article => article.isLead && article.Id != ArticleId);

            foreach (var article in articles)
            {
                db.Entry(article).State = EntityState.Modified;

                article.isLead = false;
            }

            db.SaveChanges();
        }
    }
}