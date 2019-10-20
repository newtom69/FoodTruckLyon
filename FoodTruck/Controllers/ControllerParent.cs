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
        protected SessionVariables VariablesSession;
        public ControllerParent()
        {
            VariablesSession = new SessionVariables();
            VisiteDAL.Enregistrer(VariablesSession.Utilisateur.Id);
        }
    }
}