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
using Newtonsoft.Json;

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
                        logger.Debug("Initializing new quiz game instance");
                        if (instance == null)
                        {
                            instance = new QuizGame();
                            instance.SetupQuiz();
                        }
                    }
                }
                return instance;
            }
        }

        #endregion singleton constructor

        #region private properties

        private static Logger logger = LogManager.GetLogger("QuizGame");
        private UnitOfWork uow;
        private QuizState quizState;
        private Dictionary<int, string> messages = new Dictionary<int,string>();
        private Random random = new Random();
        private TriviaQuestion currentQuestion;
        private int hintsGiven = 0;

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
                From = "QuizMaster",
                Type = "Message"
            };
            AddMessage(quizmessage);
        }

        private void AddMessage(string message, string FromUser)
        {
            var quizmessage = new QuizMessage()
            {
                Message = message,
                From = FromUser,
                To = "All",
                Type = "Message"
            };
            AddMessage(quizmessage);
        }

        private void AddMessage(string message, string FromUser, string ToUser)
        {
            var quizmessage = new QuizMessage()
            {
                Message = message,
                From = FromUser,
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

        #region Start Question and hints

        private void StartQuestion()
        {
            hintsGiven = 0;
            currentQuestion = uow.qg.TriviaQuestions.OrderBy(d => d.Id).Skip(random.Next(quizState.TotalTriviaQuestions)).First();
            AddMessage(currentQuestion.Question);
            var firstChanceTimer = new Timer(30000);
            firstChanceTimer.AutoReset = false;
            firstChanceTimer.Elapsed += firstChanceTimer_Elapsed;            
            firstChanceTimer.Start();
        }

        private void firstChanceTimer_Elapsed(object sender, ElapsedEventArgs args)
        {
            if (hintsGiven == 0)
            {
                hintsGiven = 1;
                string hint = Regex.Replace(currentQuestion.Answer, @"\S", "_");
                AddMessage("Here's a hint: " + hint);
                if (sender is Timer && sender != null)
                    ((Timer)sender).Dispose();
                var secondChanceTimer = new Timer(30000);
                secondChanceTimer.AutoReset = false;
                secondChanceTimer.Elapsed += secondChanceTimer_Elapsed;
                secondChanceTimer.Start();
            }
        }

        private void secondChanceTimer_Elapsed(object sender, ElapsedEventArgs args)
        {
            if (hintsGiven == 1)
            {
                hintsGiven = 2;
                string hint = Regex.Replace(currentQuestion.Answer, @"\B\S", "_");
                AddMessage("Here's another hint: " + hint);
                if (sender is Timer && sender != null)
                    ((Timer)sender).Dispose();
                var lastChanceTimer = new Timer(30000);
                lastChanceTimer.AutoReset = false;
                lastChanceTimer.Elapsed += lastChanceTimer_Elapsed;
                lastChanceTimer.Start();
            }
        }

        private void lastChanceTimer_Elapsed(object sender, ElapsedEventArgs args)
        {
            if (hintsGiven == 2)
            {
                hintsGiven = 3;
                string hint = Regex.Replace(currentQuestion.Answer, @"\B\S\B", "_");
                AddMessage("Last hint: " + hint);
                if (sender is Timer && sender != null)
                    ((Timer)sender).Dispose();
                var sendAnswerTimer = new Timer(30000);
                sendAnswerTimer.AutoReset = false;
                sendAnswerTimer.Elapsed += sendAnswerTimer_Elapsed;
                sendAnswerTimer.Start();
            }
        }

        private void sendAnswerTimer_Elapsed(object sender, ElapsedEventArgs args)
        {
            if (hintsGiven == 3)
            {
                string answer = currentQuestion.Answer;
                AddMessage("Times up!  The answer was " + answer);
                AddMessage("Get ready for the next question.");
                if (sender is Timer && sender != null)
                    ((Timer)sender).Dispose();
                StartQuestion();                
            }
        }

        #endregion Start Question and hints

        #endregion private methods

        #region public methods

        public bool AttemptAnswer(string answer, string user)
        {
            AddMessage(answer, user);
            if (answer.Equals(currentQuestion.Answer, StringComparison.InvariantCultureIgnoreCase))
            {
                AddMessage("Congratulations to " + user + " for providing the correct answer of " + currentQuestion.Answer);
                AddMessage("Get ready for the next question.");
                StartQuestion();
                return true;
            }
            else
                return false;
        }

        #endregion public methods

    }

    public class QuizMessage
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string To { get; set; }
        public string From { get; set; }
        public string Type { get; set; }        
    }

}