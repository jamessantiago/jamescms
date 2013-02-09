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
            uow.TextContext.Texts.Take(10);
            return View();
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

        public ActionResult Create()
        {
            uow.TextContext.Texts.Add(new Text()
            {
                Article = "firsto",
                Posted = DateTime.Now,
                Title = "Firest",
                Updated = DateTime.Now
            });
            uow.TextContext.SaveChanges();
            return RedirectToAction("Index", "sd");
        }

    }
}
