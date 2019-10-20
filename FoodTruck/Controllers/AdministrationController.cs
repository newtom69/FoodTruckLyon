using FoodTruck.DAL;
using FoodTruck.Models;
using FoodTruck.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FoodTruck.Controllers
{
    public class AdministrationController : ControllerParent
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult CommandesEnCours()
        {
            AdministrationViewModel administrationViewModel = null;
            if (session.AdminSuper || session.AdminCommande)
            {
                //lister les commandes en cours
                CommandeDAL commandeDAL = new CommandeDAL();
                List<Commande> commandesEnCours = commandeDAL.ListerCommandesEnCours();
                administrationViewModel = new AdministrationViewModel(commandesEnCours);
            }
            return View(administrationViewModel);
        }

        [HttpPost]
        public ActionResult CommandesEnCours(int id, string statut)
        {
            bool retire = false;
            bool annule = false;
            if (statut == "retire")
                retire = true;
            else if (statut == "annule")
                annule = true;
            new CommandeDAL().MettreAJourStatut(id, retire, annule);
            return RedirectToAction(RouteData.Values["action"].ToString());
        }

        [HttpGet]
        public ActionResult Commandes()
        {
            AdministrationViewModel administrationViewModel = null;
            if (session.AdminSuper || session.AdminCommande)
            {
                CommandeDAL commandeDAL = new CommandeDAL();
                var commandes = commandeDAL.ListerCommandesToutes();
                administrationViewModel = new AdministrationViewModel(commandes);
            }
            return View(administrationViewModel);
        }

        [HttpPost]
        public ActionResult Commandes(int id, string statut)
        {
            bool retire = false;
            bool annule = false;
            if (statut == "retire")
                retire = true;
            else if (statut == "annule")
                annule = true;
            new CommandeDAL().MettreAJourStatut(id, retire, annule);
            return RedirectToAction(RouteData.Values["action"].ToString());
        }
    }
}