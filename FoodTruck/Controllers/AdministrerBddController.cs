using FoodTruck.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FoodTruck.Controllers
{
    public class AdministrerBddController : ControllerParent
    {
        public ActionResult PurgerBdd()
        {
            if (AdminUtilisateur)
            {
                ViewBag.PanierProspectSupprimes = "Nombre d'enregistrement de PanierProspect supprimés : " + new PanierProspectDAL("").Purger(30);
                ViewBag.OubliMotDePasseSupprimes = "Nombre d'enregistrement de OubliMotDePasse supprimés : " + new OubliMotDePasseDAL().Purger();
                ViewBag.JourExceptionnelSupprimes = "Nombre d'enregistrement de JourExceptionnel supprimés : " + new JourExceptionnelDAL().Purger();
            }
            return View();
        }
    }
}