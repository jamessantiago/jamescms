using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Timers;
using System.Text.RegularExpressions;
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
        private TriviaQuestion currentQuestion;

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

        #region AddMessage

        private void AddMessage(string message)
        {
            var quizmessage = new QuizMessage()
            {
                Message = message,
                To = "All",
                Type = "Message"
            };
            AddMessage(quizmessage);
        }

        private void AddMessage(string message, string ToUser)
        {
            var quizmessage = new QuizMessage()
            {
                Message = message,
                To = ToUser,
                Type = "Message"
            };
            AddMessage(quizmessage);
        }

        private void AddMessage(QuizMessage message)
        {
            int index = 1;
            if (messages.Any())
                index = messages.Keys.Max() + 1;

            if (messages.Count > MAX_MESSAGES)
                messages.Remove(messages.Keys.Min());

            message.Id = index;
            var jsonMessage = new JavaScriptSerializer().Serialize(message);

            messages.Add(index, jsonMessage);

            if (MessageArrived != null)
                MessageArrived(this, new EventArgs());
        }

        #endregion AddMessage

        private void StartQuestion()
        {
            currentQuestion = uow.qg.TriviaQuestions.Skip(random.Next(quizState.TotalTriviaQuestions)).First();
            AddMessage(currentQuestion.Question);
            var firstChanceTimer = new Timer(30000);
            firstChanceTimer.AutoReset = false;
            firstChanceTimer.Elapsed += firstChanceTimer_Elapsed;
        }

        private void firstChanceTimer_Elapsed(object sender, ElapsedEventArgs args)
        {
            string hint = Regex.Replace(currentQuestion.Answer, @"\S", "_");
            AddMessage("Here's a hint: " + hint);
            if (sender is Timer && sender != null)
                ((Timer)sender).Dispose();
            var secondChanceTimer = new Timer(30000);
            secondChanceTimer.AutoReset = false;
            secondChanceTimer.Elapsed += secondChanceTimer_Elapsed;
        }

        private void secondChanceTimer_Elapsed(object sender, ElapsedEventArgs args)
        {
            string hint = Regex.Replace(currentQuestion.Answer, @"\B\S", "_");
            AddMessage("Here's another hint: " + hint);
            if (sender is Timer && sender != null)
                ((Timer)sender).Dispose();
            var lastChanceTimer = new Timer(30000);
            lastChanceTimer.AutoReset = false;
            lastChanceTimer.Elapsed += lastChanceTimer_Elapsed;
        }

        private void lastChanceTimer_Elapsed(object sender, ElapsedEventArgs args)
        {
            string hint = Regex.Replace(currentQuestion.Answer, @"\B\S", "_");
            AddMessage("Last hint: " + hint);
            if (sender is Timer && sender != null)
                ((Timer)sender).Dispose();
            var sendAnswerTimer = new Timer(30000);
            sendAnswerTimer.AutoReset = false;
            sendAnswerTimer.Elapsed += 



                ///james, just make this a single timer, you'll need to have some class properties for when the question is answered anyway.
        }

        #endregion private methods

    }

    public class QuizMessage
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string To { get; set; }
        public string Type { get; set; }        
    }

}