using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using jamescms.Models;
using NLog;

namespace jamescms.Games
{
    public sealed class QuizGame
    {

        #region singleton constructor

        private static volatile QuizGame instance;
        private static object syncRoot = new object();

        private QuizGame()
        {
            uow = new UnitOfWork();
        }

        public static QuizGame Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new QuizGame();
                    }
                }
                return instance;
            }
        }

        #endregion singleton constructor

        #region private properties

        private Logger logger = LogManager.GetLogger("QuizGame");
        private UnitOfWork uow;
        private QuizState quizState;
        private Dictionary<int, string> messages;

        #endregion private properties

        #region public properties

        public Dictionary<int, string> Messages
        {
            get { if (messages == null) { messages = new Dictionary<int, string>(); } return messages; }
        }
        public event EventHandler MessageArrived;
        public event EventHandler QuizStarted;

        #endregion public properties

        #region private methods

        private void SetupQuiz()
        {
            if (!uow.qg.QuizStates.Any())
            {
                uow.qg.QuizStates.Add(new QuizState()
                {
                    LastQuestion = 0,
                    LastTimeStarted = DateTime.Now
                });
            }
            else
            {
                quizState = uow.qg.QuizStates.First();
                quizState.LastTimeStarted = DateTime.Now;
                uow.qg.SaveChanges();
            }

            if (!uow.qg.TriviaQuestions.Any())
            {
                messages.AddMessage("There don't seem to be any questions in the database");
            }
            else
            {
                messages.AddMessage("Quiz game is starting");
                StartQuestion();
            }
        }

        private void StartQuestion()
        {
            var question = uow.qg.TriviaQuestions.OrderBy(d => Guid.NewGuid()).Take(1);

        }

        #endregion private methods

    }

    public static class QuizGameExtensions
    {
        private const int MAX_MESSAGES = 100;

        public static void AddMessage(this Dictionary<int, string> dic, string message)
        {
            int index = 1;
            if (dic.Any())
                index = dic.Keys.Max() + 1;

            if (dic.Count > MAX_MESSAGES)
                dic.Remove(dic.Keys.Min());

            dic.Add(index, message);

        }
    }
}