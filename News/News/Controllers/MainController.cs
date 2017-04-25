using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace News.Controllers
{
    public class MainController : Controller
    {
        public ActionResult Contact()
        {
            ViewBag.Message = "Creator of this application:";

            return View();
        }
    }
}