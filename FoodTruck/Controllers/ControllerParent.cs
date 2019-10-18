using FoodTruck.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FoodTruck.Controllers
{
    public class ControllerParent : Controller
    {
        protected SessionVariables session;
        public ControllerParent()
        {
            session = new SessionVariables();
            VisiteDAL.Enregistrer(session.Utilisateur.Id);
        }
    }
}