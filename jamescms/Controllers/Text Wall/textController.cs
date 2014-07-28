using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using jamescms.Models;
using jamescms.Helpers;

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

        [OutputCache(Duration = 3600)]
        public ActionResult Index()
        {
            ViewData["page"] = 1;

            var total = uow.tc.Texts.Count();
            var take = (0 * 10 + 10) > total ? total - (0 * 10) : 10;
            return View(uow.tc.Texts.OrderByDescending(d => d.Posted).Skip(0).Take(take));
        }

        [OutputCache(Duration = 3600)]
        public ActionResult p(int id = 0)
        {
            ViewData["page"] = id + 1;

            var total = uow.tc.Texts.Count();
            var take = (id * 10 + 10) > total ? total - (id * 10) : 10;
            if (take > 0)
                return PartialView("_TextPage", uow.tc.Texts.OrderByDescending(d => d.Posted).Skip(id * 10).Take(take));
            else
                return PartialView("_EndofPage");
        }

        [OutputCache(Duration = 3600)]
        public ActionResult d(string id)
        {
            var text = uow.tc.Texts.Where(d => d.UrlTitle == id).FirstOrDefault();
            if (text != null)
                return View(text);
            else
                return RedirectToAction("Index");
        }

        [Authorize(Roles="Guides")]
        public ActionResult edit(int id)
        {
            var item = uow.tc.Texts.Where(d => d.Id == id).FirstOrDefault();
            return PartialView("_TextEdit", item);
        }

        [Authorize(Roles = "Guides")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult edit(Text model)
        {
            var text = uow.tc.Texts.Where(d => d.Id == model.Id).FirstOrDefault();
            UpdateModel(text);
            if (ModelState.IsValid)
            {
                uow.tc.SaveChanges();
                ViewData["readyControl"] = true;
                return PartialView("_SingleText", text);
            }
            else
                return PartialView("_TextEdit", text);
        }

        [Authorize(Roles = "Guides")]
        [HttpPost]
        public ActionResult preview(FormCollection collection)
        {
            string data = collection[0];
            return PartialView("_preview", data);
        }

        [OutputCache(Duration = 3600)]
        public ActionResult loadOne(int id)
        {
            ViewData["readyControl"] = true;
            var text = uow.tc.Texts.Where(d => d.Id == id).FirstOrDefault();
            return PartialView("_SingleText", text);
        }

        [OutputCache(Duration = 3600)]
        public ActionResult latest()
        {
            var text = uow.tc.Texts.OrderByDescending(d => d.Posted).FirstOrDefault();
            return PartialView("_LatestText", text);
        }

        [OutputCache(Duration = 3600)]
        public ActionResult recentList()
        {
            var total = uow.tc.Texts.Count();
            var take = 5 > total ? total - 5 : 5;
            var texts = uow.tc.Texts.OrderByDescending(d => d.Posted).Take(5);
            return PartialView("_RecentTexts", texts);
        }

        [Authorize(Roles = "Guides")]
        public ActionResult PullTextFiles()
        {
            new Thread(new ThreadStart(TextFileHelper.PullTextFiles)).Start();
            return RedirectToAction("Index");            
        }

        [Authorize(Roles="Guides")]
        public ActionResult PushTextFiles()
        {
            new Thread(new ThreadStart(TextFileHelper.PushTextFiles)).Start();
            return RedirectToAction("Index");
        }        

    }
}
