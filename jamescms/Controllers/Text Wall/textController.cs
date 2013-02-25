using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using jamescms.Models;

namespace jamescms.Controllers
{
    public class textController : Controller
    {

        #region Constructor

        private UnitOfWork uow;

        public textController()
        {
            uow = new UnitOfWork();
        }

        public textController(UnitOfWork UnitOfWork)
        {
            uow = UnitOfWork;
        }

        #endregion Constructor

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult p(int id = 0)
        {
            return PartialView("_TextPage", uow.tc.Texts.Skip(id * 10).Take(10));
        }

        public ActionResult d(string id)
        {
            var text = uow.tc.Texts.Where(d => d.UrlTitle == id).FirstOrDefault();
            if (text != null)
                return View(text);
            else
                return RedirectToAction("Index");
        }

        public ActionResult Create()
        {
            uow.tc.Texts.Add(new Text()
            {
                Article = "firsto",
                UrlTitle = "firsto",
                Posted = DateTime.Now,
                Title = "Firest",
                Updated = DateTime.Now
            });
            uow.tc.SaveChanges();
            
            return RedirectToAction("Index", "sd");
        }

    }
}
