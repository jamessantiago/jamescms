using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using jamescms.Models;

namespace jamescms.Controllers.Portal
{
    public class photoController : Controller
    {
        #region Constructor

        private UnitOfWork uow;

        public photoController()
        {
            uow = new UnitOfWork();
        }

        public photoController(UnitOfWork UnitOfWork)
        {
            uow = UnitOfWork;
        }

        #endregion Constructor


    }
}
