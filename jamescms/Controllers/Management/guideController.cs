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
using jamescms.Helpers;
using jamescms.Services.WebSocketControllers;
using System.IO;

namespace jamescms.Controllers
{
    [Authorize(Roles="Guides")]
    public class guideController : Controller
    {

        private static Dictionary<string, WebSocketFileTail> listeners = new Dictionary<string, WebSocketFileTail>();
        private static Dictionary<string, WebSocketFilePreview> previewListeners = new Dictionary<string, WebSocketFilePreview>();

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

        public ActionResult LogChooser()
        {
            List<string> files = (from file in Directory.EnumerateFiles("D:\\Code\\jamescms\\jamescms\\logs\\")
                                 select Path.GetFileName(file)).ToList();
            
            return PartialView("_LogChooser", files);
        }

        public ActionResult GetLog(string logName)
        {
            var oldListener = listeners.Where(d => d.Key == User.Identity.Name);
            if (oldListener.Count() == 1)
            {
                oldListener.FirstOrDefault().Value.Dispose();
                listeners.Remove(oldListener.FirstOrDefault().Key);
            }
            var listener = new WebSocketFileTail("D:\\Code\\jamescms\\jamescms\\logs\\" + logName, Session.SessionID);
            listener.Start();
            listeners.Add(User.Identity.Name, listener);
            return PartialView("_Log");
        }

        #endregion Error Logs

        #region File Preview

        public ActionResult FilePreviewChooser()
        {
            List<string> files = (from file in Directory.EnumerateFiles("D:\\TextFiles\\")
                                  select Path.GetFileName(file)).ToList();

            return PartialView("_filePreviewChooser", files);
        }

        public ActionResult GetFilePreview(string fileName)
        {
            var oldListener = previewListeners.Where(d => d.Key == User.Identity.Name);
            if (oldListener.Count() == 1)
            {
                oldListener.FirstOrDefault().Value.Dispose();
                previewListeners.Remove(oldListener.FirstOrDefault().Key);
            }
            var previewListener = new WebSocketFilePreview("D:\\TextFiles\\" + fileName, "fp" +  Session.SessionID);
            previewListener.Start();
            previewListeners.Add(User.Identity.Name, previewListener);
            return PartialView("_FilePreview");
        }

        #endregion File Preview
    }
}
