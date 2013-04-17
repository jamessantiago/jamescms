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

        public ActionResult Load()
        {
            QuizGameContext qg = new QuizGameContext();
            qg.Configuration.AutoDetectChangesEnabled = false;
            string filepath = "D:\\Downloads\\trivia";
            foreach (var file in Directory.GetFiles(filepath))
            {
                using (StreamReader fr = new StreamReader(file))
                {
                    string line = "";
                    int lineCount = 0;
                    while ((line = fr.ReadLine()) != null)
                    {
                        lineCount++;
                        var parts = line.Split('*');
                        if (parts.Length == 2 && !string.IsNullOrEmpty(parts[0]) && !string.IsNullOrEmpty(parts[1]))
                        {
                            TriviaQuestion temp = new TriviaQuestion()
                            {
                                Question = parts[0],
                                Answer = parts[1]
                            };
                            qg.TriviaQuestions.Add(temp);
                            try
                            {
                                if (lineCount % 100 == 0)
                                {
                                    qg.SaveChanges();
                                    qg.Dispose();
                                    qg = new QuizGameContext();
                                    qg.Configuration.AutoDetectChangesEnabled = false;
                                }
                            }
                            catch { }
                        }
                    }
                }
            }
            return RedirectToAction("Index");
        }

    }
}
