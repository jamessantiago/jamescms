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
using System.Threading;

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
            public string[] Roles { get; set; }
        }

        public ActionResult UserControl()
        {
            UserStats stats = new UserStats(){
                TotalUsers = uow.uc.UserProfiles.Count(),
                NewestUser = uow.uc.UserProfiles.OrderByDescending(d => d.UserId).First(),
                Roles = Roles.GetAllRoles()
            };
            return PartialView("_UsersControl", stats);
        }

        public ActionResult AllUsers()
        {
            return PartialView("_Users", uow.uc.UserProfiles.Select(d => d.UserName).ToArray());
        }

        public ActionResult UsersInRole(string role)
        {
            
            return PartialView("_Users", Roles.GetUsersInRole(role));
        }
        
        public ActionResult AddUserToRole(string username, string role, bool BackToRole)
        {
            var user = uow.uc.UserProfiles.Where(d => d.UserName == username).FirstOrDefault();
            user.AddToRole(role);
            if (BackToRole)
                return PartialView("_Users", Roles.GetUsersInRole(role));
            else
                return PartialView("_Users", uow.uc.UserProfiles.Select(d => d.UserName).ToArray());
        }

        public ActionResult RemoveUserFromRole(string username, string role, bool BackToRole)
        {
            var user = uow.uc.UserProfiles.Where(d => d.UserName == username).FirstOrDefault();
            user.RemoveFromRole(role);
            if (BackToRole)
                return PartialView("_Users", Roles.GetUsersInRole(role));
            else
                return PartialView("_Users", uow.uc.UserProfiles.Select(d => d.UserName).ToArray());
        }

        public ActionResult DeleteUser(string username)
        {
            var user = uow.uc.UserProfiles.Where(d => d.UserName == username).FirstOrDefault();
            var userroles = user.GetUserRoles();
            if (userroles.Any())
                Roles.RemoveUserFromRoles(username, userroles);
            uow.uc.UserProfiles.Remove(user);
            uow.uc.SaveChanges();
            return PartialView("_Users", uow.uc.UserProfiles.Select(d => d.UserName).ToArray());
        }

        #endregion Users

        #region Error Logs

        public ActionResult TestWS()
        {
            jamescms.Services.WebSocketListener ws = new Services.WebSocketListener();
            new Thread(new ThreadStart(ws.InitializeListener)) { IsBackground = true }.Start();
            return PartialView("_TestWS");
        }

        #endregion Error Logs
    }
}
