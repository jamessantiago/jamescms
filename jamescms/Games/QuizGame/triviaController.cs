using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using jamescms.Games;
using jamescms.Services.WebSocketControllers;
using System.IO;
using jamescms.Models;

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
            if (!webSocketQuizGame.QuizGameStarted)
                webSocketQuizGame.Start();
   
            return View();
        }

    }
}
