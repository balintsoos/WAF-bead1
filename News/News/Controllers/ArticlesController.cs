using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using News.Models;
using System.Net;
using System.Data.Entity;

namespace News.Controllers
{
    public class ArticlesController : Controller
    {
        private NewsModel db = new NewsModel();

        // GET: Articles
        public ActionResult Index()
        {
            return View(db.Articles.OrderBy(a => a.PublishedAt).ToList());
        }

        // GET: Articles/Archive
        public ActionResult Archive()
        {
            return View(db.Articles.OrderBy(a => a.PublishedAt).ToList());
        }

        public ActionResult MyArticles()
        {
            var username = System.Web.HttpContext.Current.User.Identity.Name;

            if (username == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return View(db.Articles.Where(a => a.Author == username).OrderBy(a => a.PublishedAt).ToList());
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

            return View(item);
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
    }
}