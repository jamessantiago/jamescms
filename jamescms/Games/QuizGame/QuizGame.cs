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
        private static object syncMessage = new object();

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
        private List<QuizProfile> users = new List<QuizProfile>();
        private string CurrentLeader;
        private int TopPoints;
        private object syncAnswer = new object();
        private const int MAX_MESSAGES = 100;

        #endregion private properties

        #region public properties

        public Dictionary<int, string> Messages
        {
            get { if (messages == null) { messages = new Dictionary<int, string>(); } return messages; }
        }
        public List<QuizProfile> Users
        {
            get { if (users == null) { users = new List<QuizProfile>(); } return users; }
        }
        public event EventHandler MessageArrived;

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
                From = "Judge",
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
            lock (syncMessage)
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
        }

        #endregion AddMessage

        #region Start Question and hints

        private void StartQuestion()
        {
            uow.qg.SaveChanges();
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
                string hint = Regex.Replace(currentQuestion.Answer, @"\S", "-");
                AddMessage("<span style='color:#2e4272'>Here's a hint:</span> " + hint);
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
                string hint = Regex.Replace(currentQuestion.Answer, @"\B\S", "-");
                AddMessage("<span style='color:#2e4272'>Here's another hint: </span>" + hint);
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
                string hint = Regex.Replace(currentQuestion.Answer, @"\B\S\B", "-");
                AddMessage("<span style='color:#2e4272'>Last hint: </span>" + hint);
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
                lock (syncAnswer)
                {
                    string answer = currentQuestion.Answer;
                    AddMessage("<span style='color:#c5649b'>Times up!  The answer was </span>" + answer);
                    AddMessage("<span style='color:#d6b26c'>Get ready for the next question.</span>");
                    if (sender is Timer && sender != null)
                        ((Timer)sender).Dispose();
                }
                StartQuestion();                
            }
        }

        #endregion Start Question and hints

        #endregion private methods

        #region public methods

        public bool AttemptAnswer(string answer, string user)
        {
            lock (syncAnswer)
            {
                AddMessage(answer, user);
                if (answer.Equals(currentQuestion.Answer, StringComparison.InvariantCultureIgnoreCase))
                {
                    int points = 5 - hintsGiven;
                    AddMessage("<span style='color:#7b9f35'>Congratulations to </span>" + user + " <span style='color:#7b9f35'>for providing the correct answer of </span>" + currentQuestion.Answer);
                    AwardPoints(user, points);
                    AddMessage("<span style='color:#aa8439'>Get ready for the next question.</span>");
                    StartQuestion();
                    return true;
                }
                else
                {
                    IncrementAttempts(user);
                    return false;
                }
            }
        }

        public void UserJoin(Guid id, string username)
        {
            if (!users.Any(d => d.SocketId == id))
            {
                if (!users.Any(d => d.UserName == username))
                    AddMessage("<span style='color:#a64b3d'>A new player has joined.  Welcome </span>" + username);
                else 
                    users.Remove(users.First(d => d.UserName == username));

                QuizProfile newProfile = new QuizProfile()
                {
                    SocketId = id,
                    UserName = username,
                    Answered = 0,
                    Attempts = 0,
                    ThisGamePoints = 0
                };
                var userProfile = uow.uc.UserProfiles.FirstOrDefault(d => d.UserName == username);
                if (userProfile != null)
                {
                    var gameProfile = uow.qg.UserGameProfiles.FirstOrDefault(d => d.AccountModelUserId == userProfile.UserId);
                    if (gameProfile != null)
                        newProfile.GameProfileId = gameProfile.Id;
                    else
                        newProfile.GameProfileId = 0;
                }
                else
                    newProfile.GameProfileId = 0;
                users.Add(newProfile);

                QuizMessage userList = new QuizMessage()
                {
                    Type = "SetUsers",
                    Users = users.Select(d => d.UserName).OrderBy(d => d).ToArray()
                };
                AddMessage(userList);
            }    
        }

        public void UserQuit(Guid id)
        {
            if (users.Any(d => d.SocketId == id))
            {
                users.Remove(users.First(d => d.SocketId == id));
                QuizMessage userList = new QuizMessage()
                {
                    Type = "SetUsers",
                    Users = users.Select(d => d.UserName).OrderBy(d => d).ToArray()
                };
                AddMessage(userList);
            }
        }

        public void AwardPoints(string user, int points)
        {
            var userProfile = users.FirstOrDefault(d => d.UserName == user);
            if (userProfile != null)
            {
                userProfile.Attempts++;
                userProfile.Answered++;
                userProfile.ThisGamePoints += points;
                if (userProfile.GameProfileId != 0)
                {
                    var gameProfile = uow.qg.UserGameProfiles.FirstOrDefault(d => d.Id == userProfile.GameProfileId);
                    if (gameProfile != null)
                    {
                        gameProfile.Attempts++;
                        gameProfile.CorrectAnswers++;
                        gameProfile.Points += points;
                        uow.qg.SaveChanges();
                    }
                }

                if (userProfile.ThisGamePoints == users.Select(d => d.ThisGamePoints).Max() && CurrentLeader != user)
                {
                    CurrentLeader = user;
                    TopPoints = userProfile.ThisGamePoints;
                    AddMessage(user + "<span style='color:#a64b3d'> is now the current leader with " + userProfile.ThisGamePoints + " points!</span>");
                }
                else if (CurrentLeader == user)
                {
                    TopPoints = userProfile.ThisGamePoints;
                }
                SendPoints(user);
            }
        }

        public void IncrementAttempts(string user)
        {
            if (users.Any(d => d.UserName == user))
            {
                var userProfile = users.First(d => d.UserName == user);
                userProfile.Attempts++;
                if (userProfile.GameProfileId != 0)
                {
                    var gameProfile = uow.qg.UserGameProfiles.FirstOrDefault(d => d.Id == userProfile.GameProfileId);
                    gameProfile.Attempts++;
                }
                SendPoints(user);
            }
        }

        public void SendPoints(string user)
        {
            if (users.Any(d => d.UserName == user))
            {
                var userProfile = users.First(d => d.UserName == user);
                QuizMessage message = new QuizMessage()
                {
                    SocketId = userProfile.SocketId,
                    Type = "SetStats",
                    Answered = userProfile.Answered,
                    Attempts = userProfile.Attempts,
                    Points = userProfile.ThisGamePoints,
                    Leader = CurrentLeader,
                    TopPoints = TopPoints
                };
                AddMessage(message);
            }
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
        public Guid SocketId { get; set; }
        public string Leader { get; set; }
        public int TopPoints {get; set;}
        public int Attempts { get; set; }
        public int Answered { get; set; }
        public int Points { get; set; }
        public string[] Users { get; set; }
    }

    public class QuizProfile
    {
        public Guid SocketId { get; set; }
        public string UserName { get; set; }
        public int GameProfileId { get; set; }
        public int ThisGamePoints { get; set; }
        public int Attempts { get; set; }
        public int Answered { get; set; }
    }

}