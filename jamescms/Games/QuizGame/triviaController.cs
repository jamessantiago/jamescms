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
            webSocketQuizGame = new WebSocketQuizGame();
            webSocketQuizGame.Start();
        }

        public ActionResult Index()
        {
            return View();
        }

    }
}
