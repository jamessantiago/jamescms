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
        
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult p(int id = 0)
        {
            ViewData["page"] = id + 1;

            var total = uow.tc.Texts.Count();
            var take = (id * 10 + 10) > total ? total - (id * 10) : 10;
            if (take > 0)
                return PartialView("_TextPage", uow.tc.Texts.OrderByDescending(d => d.Id).Skip(id * 10).Take(take));
            else
                return PartialView("_EndofPage");
        }

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

        public ActionResult loadOne(int id)
        {
            ViewData["readyControl"] = true;
            var text = uow.tc.Texts.Where(d => d.Id == id).FirstOrDefault();
            return PartialView("_SingleText", text);
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

        public ActionResult Create()
        {
            uow.tc.Texts.Add(
                new Text
                {
                    Title = "My first post",
                    UrlTitle = "Mafdy_firsdft_post",
                    Article =
@"
###This my very first post

this is a test

    {{C#}}
    public void GetCats() {
        return new Meow() { Name = ""Whiskers"" };
    }
",
                    Posted = new DateTime(1970, 1, 1),
                    Updated = new DateTime(1970, 1, 1)
                });

            uow.tc.Texts.Add(
                new Text
                {
                    Title = "My second post",
                    UrlTitle = "Mayf_Secdfond_postf",
                    Article = "This my second post",
                    Posted = new DateTime(1970, 1, 1),
                    Updated = new DateTime(1970, 1, 1),
                    Tags = new List<Tag>() { new Tag() { Name = "Fister" } }
                });

            uow.tc.Texts.Add(
                new Text
                {
                    Title = "My thifrd post",
                    Article = "This fmy third post",
                    UrlTitle = "thirfadd_post",
                    Posted = new DateTime(1970, 1, 1),
                    Updated = new DateTime(1970, 1, 1),
                    Tags = new List<Tag>() { new Tag() { Name = "thirds" } },
                });


            uow.tc.Texts.Add(
                new Text
                {
                    Title = "My powershell post",
                    UrlTitle = "poaawefddrsh_post",
                    Article =
@"
    {{Powershell}}
    function GetNextString ($current) { 
      $x = ConvertStringToInt $current
      $x ++
      ConvertIntToString $x
    } 
",
                    Posted = new DateTime(1970, 1, 1),
                    Updated = new DateTime(1970, 1, 1)
                });
            try
            {
                uow.tc.SaveChanges();
            }
            catch { }
            
            return RedirectToAction("Index", "sd");
        }

    }
}
