using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using jamescms.Games;
using jamescms.Services.WebSocketControllers;

namespace jamescms.Controllers
{
    public class triviaController : Controller
    {
        private WebSocketQuizGame webSocketQuizGame;

        public triviaController()
        {
            if (webSocketQuizGame == null)
                webSocketQuizGame = new WebSocketQuizGame();
        }

        public ActionResult Index()
        {
            //if (!webSocketQuizGame.Users.Contains(Session.SessionID))
                webSocketQuizGame.Join(Session.SessionID);
            return View();
        }

    }
}
