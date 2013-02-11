using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace jamescms.Controllers
{
    public class errorController : Controller
    {
        //
        // GET: /Error/

        public ActionResult index()
        {
            return View();
        }

        public ActionResult ajax()
        {
            return View();
        }

    }
}
