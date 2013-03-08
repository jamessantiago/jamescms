using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;
using System.Transactions;

namespace jamescms.Models
{
    public class UnitOfWork : IDisposable
    {

        #region Private Contexts

        private UsersContext usersContext { get; set; }
        private TextContext textContext { get; set; }
        private PhotoRepository photoRepository { get; set; }

        #endregion Private Contexts

        #region Public Contexts

        public UsersContext uc
        {
            get
            {
                if (usersContext == null)
                {
                    usersContext = new UsersContext();
                }
                return usersContext;
            }
        }
        public TextContext tc
        {
            get
            {
                if (textContext == null)
                {
                    textContext = new TextContext();
                }
                return textContext;
            }
        }
        public PhotoRepository pr
        {
            get
            {
                if (photoRepository == null)
                {
                    photoRepository = new PhotoRepository();
                }
                return photoRepository;
            }
        }

        #endregion Public Contexts

        #region Public Methods

        public virtual void SaveAllContexts()
        {
            using (TransactionScope scope = CreateTransactionScope())
            {
                if (usersContext != null)
                    usersContext.SaveChanges();

                if (textContext != null)
                    textContext.SaveChanges();

                scope.Complete();                    
            }
        }

        #endregion Public Methods

        #region Utils

        //see http://blogs.msdn.com/b/dbrowne/archive/2010/06/03/using-new-transactionscope-considered-harmful.aspx
        private static TransactionScope CreateTransactionScope()
        {
            var transactionOptions = new TransactionOptions();
            transactionOptions.IsolationLevel = IsolationLevel.ReadCommitted;
            transactionOptions.Timeout = TransactionManager.MaximumTimeout;
            return new TransactionScope(TransactionScopeOption.Required, transactionOptions);
        }

        #endregion Utils

        #region Dispose

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~UnitOfWork()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (usersContext != null)
                {
                    usersContext.Dispose();
                    usersContext = null;
                }

                if (textContext != null)
                {
                    textContext.Dispose();
                    textContext = null;
                }
            }
        }

        #endregion Dispose
    }

    
}