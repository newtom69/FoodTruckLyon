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
            AdministrationViewModel administrationViewModel = null;
            if (session.AdminSuper || session.AdminCommande)
            {
                //lister les commandes en cours
                CommandeDAL commandeDAL = new CommandeDAL();
                List<Commande> commandes = commandeDAL.ListerEnCours();
                administrationViewModel = new AdministrationViewModel(commandes);

            }
            return View(administrationViewModel);
        }
    }
}