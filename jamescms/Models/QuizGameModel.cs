using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;

namespace jamescms.Models
{
    public class QuizGameContext : DbContext
    {
        public QuizGameContext()
            : base("ApplicationConnection")
        {
        }


    }

    public class QuizState : Entity
    {
        public int LastQuestion { get; set; }
        public DateTime LastTimeStarted { get; set; }        
    }

    public class TriviaQuestion : Entity
    {
        public string Question { get; set; }
        public string Answer { get; set; }
    }

    public class UserGameProfile : Entity
    {
        public int AccountModelUserId { get; set; }
        public DateTime GameAccountCreated { get; set; }
        public DateTime LastTimeSeen { get; set; }
        public string Nickname { get; set; }
        public long Points { get; set; }
    }
}