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
        private Random random = new Random();
        private const int MAX_MESSAGES = 100;

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
                    LastTimeStarted = DateTime.Now,
                    TotalTriviaQuestions = uow.qg.TriviaQuestions.Count()
                });
                uow.qg.SaveChanges();
            }
            else
            {
                quizState = uow.qg.QuizStates.First();
                quizState.LastTimeStarted = DateTime.Now;
                quizState.TotalTriviaQuestions = uow.qg.TriviaQuestions.Count();
                uow.qg.SaveChanges();
            }

            if (!uow.qg.TriviaQuestions.Any())
            {
                AddMessage("There don't seem to be any questions in the database");
            }
            else
            {
                AddMessage("Quiz game is starting");
                StartQuestion();
            }
        }

        private void StartQuestion()
        {
            var question = uow.qg.TriviaQuestions.Skip(random.Next(quizState.TotalTriviaQuestions)).Take(1);

        }

        private void AddMessage(string message)
        {
            int index = 1;
            if (messages.Any())
                index = messages.Keys.Max() + 1;

            if (messages.Count > MAX_MESSAGES)
                messages.Remove(messages.Keys.Min());

            messages.Add(index, message);

            if (MessageArrived != null)
                MessageArrived(this, new EventArgs());
        }

        #endregion private methods

    }

}