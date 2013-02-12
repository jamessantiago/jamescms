using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using jamescms.Filters;
using jamescms.Models;

namespace jamescms.Controllers
{
    [Authorize(Roles="Guides")]
    public class guideController : Controller
    {

        #region Constructor

        private UnitOfWork uow;

        public guideController()
        {
            uow = new UnitOfWork();
        }

        public guideController(UnitOfWork UnitOfWork)
        {
            uow = UnitOfWork;
        }

        #endregion Constructor

        public ActionResult Index()
        {
            return View();
        }

        #region Users

        public class UserStats
        {
            public int TotalUsers { get; set; }
            public UserProfile NewestUser { get; set; }
        }

        public ActionResult UserControl()
        {
            UserStats stats = new UserStats(){
                TotalUsers = uow.uc.UserProfiles.Count(),
                NewestUser = uow.uc.UserProfiles.OrderByDescending(d => d.UserId).First()
            };
            return PartialView("_UserControl", stats);
        }

        public ActionResult AllUsers()
        {
            return PartialView("_AllUsers", uow.uc.UserProfiles);
        }

        public ActionResult EditUser(string username)
        {
            var user = uow.uc.UserProfiles.Where(d => d.UserName == username).FirstOrDefault();
            return PartialView("_EditUser", user);
        }

        [HttpPost]
        public ActionResult AddUserToRole(string username, string role)
        {
            var user = uow.uc.UserProfiles.Where(d => d.UserName == username).FirstOrDefault();
            user.AddToRole(role);
            return PartialView("_EditUser", user);
        }

        #endregion Users
    }
}
