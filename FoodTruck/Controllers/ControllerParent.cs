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
            if (VariablesSession.Utilisateur.AdminSuper)
            {
                ViewBag.AdminArticle = true;
                ViewBag.AdminCommande = true;
                ViewBag.AdminUtilisateur = true;
            }
            else
            {
                if (VariablesSession.Utilisateur.AdminArticle) ViewBag.AdminArticle = true;
                else ViewBag.AdminArticle = false;
                if (VariablesSession.Utilisateur.AdminCommande) ViewBag.AdminCommande = true;
                else ViewBag.AdminCommande = false;
                if (VariablesSession.Utilisateur.AdminUtilisateur) ViewBag.AdminUtilisateur = true;
                else ViewBag.AdminUtilisateur = false;
            }
        }
    }
}