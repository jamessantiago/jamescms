using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using jamescms.Models;

namespace jamescms.Controllers.Portal
{
    public class photosController : Controller
    {
        #region Constructor

        private UnitOfWork uow;

        public photosController()
        {
            uow = new UnitOfWork();
        }

        public photosController(UnitOfWork UnitOfWork)
        {
            uow = UnitOfWork;
        }

        #endregion Constructor

        public ActionResult Index()
        {

            ViewData["Albums"] = uow.pr.GetAllAlbums();
            return View();
        }

        //[OutputCache(Duration = 604800, VaryByParam = "id", VaryByCustom = "userName", Location = OutputCacheLocation.Server)]
        public ActionResult Album(String id)
        {
            
            ViewData["Photos"] = uow.pr.GetAllPhotos(id);
            
            return View();
        }

        public String GetComments(String albumid, String photoid)
        {
            String result = uow.pr.GetAllComments(albumid, photoid);
            return result;
        }
        
        public ActionResult AddComment(FormCollection collection)
        {
            String albumid = collection["albumid"];
            String photoid = collection["photoid"];
            String comment = collection["comment"] + " - <strong>" + User.Identity.Name + "</strong>";
            uow.pr.AddComment(albumid, photoid, comment);
            return RedirectToAction("Album", new { id = albumid });
        }

        [Authorize(Roles="Administrators")]
        public ActionResult MakePrivate(string id)
        {
            uow.pr.ModifyAlbumSummary(id, "Friends Only");
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Administrators")]
        public ActionResult MakeSemiPrivate(string id)
        {
            uow.pr.ModifyAlbumSummary(id, "coworkers");
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Administrators")]
        public ActionResult MakePublic(string id)
        {
            uow.pr.ModifyAlbumSummary(id, "all");
            return RedirectToAction("Index");
        }
    }
}
