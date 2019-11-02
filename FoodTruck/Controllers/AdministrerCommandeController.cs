using FoodTruck.DAL;
using FoodTruck.ViewModels;
using System.Net;
using System.Web.Mvc;

namespace FoodTruck.Controllers
{
    public class AdministrerCommandeController : ControllerParent
    {
        [HttpGet]
        public ActionResult EnCours()
        {
            if (AdminCommande)
                return View(new ListeCommandesViewModel(new CommandeDAL().ListerCommandesEnCours()));
            else
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
        }

        [HttpPost]
        public ActionResult EnCours(int id, string statut)
        {
            if (AdminCommande)
            {
                bool retire = false;
                bool annule = false;
                if (statut == "retire")
                    retire = true;
                else if (statut == "annule")
                    annule = true;
                new CommandeDAL().MettreAJourStatut(id, retire, annule);
                return RedirectToAction(ActionNom);
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
        }

        [HttpGet]
        public ActionResult AStatuer()
        {
            if (AdminCommande)
                return View(new ListeCommandesViewModel(new CommandeDAL().ListerCommandesAStatuer()));
            else
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
        }

        [HttpPost]
        public ActionResult AStatuer(int id, string statut)
        {
            if (AdminCommande)
            {
                bool retire = false;
                bool annule = false;
                if (statut == "retire")
                    retire = true;
                else if (statut == "annule")
                    annule = true;
                new CommandeDAL().MettreAJourStatut(id, retire, annule);
                return RedirectToAction(ActionNom);
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
        }

        [HttpGet]
        public ActionResult Toutes()
        {
            if (AdminCommande)
                return View(new ListeCommandesViewModel(new CommandeDAL().ListerCommandesToutes()));
            else
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
        }

        [HttpGet]
        public ActionResult AVenir()
        {
            if (AdminCommande)
                return View(new ListeCommandesViewModel(new CommandeDAL().ListerCommandesAVenir()));
            else
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
        }
    }
}