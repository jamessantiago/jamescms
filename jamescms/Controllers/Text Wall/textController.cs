using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace jamescms.Controllers
{
    public class textController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("p", new { id = 1 });
        }

        public ActionResult p(int id)
        {
            int page = id;
            return View();
        }

        public ActionResult Details(string id)
        {
            return View();
        }

    }
}
