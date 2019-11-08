using FoodTruck.DAL;
using FoodTruck.Models;
using FoodTruck.ViewModels;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;

namespace FoodTruck.Controllers
{
    public class AdministrerCommandeController : ControllerParent
    {
        [HttpGet]
        public ActionResult EnCours()
        {
            const int fouchetteHeures = 4;
            if (AdminCommande)
                return View(new ListeCommandesViewModel(new CommandeDAL().ListerCommandesEnCours(fouchetteHeures)));
            else
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
        }

        [HttpPost]
        public ActionResult EnCours(int id, string statut)
        {
            if (AdminCommande)
            {
                if (statut == "retire")
                    new CommandeDAL().Retirer(id);
                else if (statut == "annule")
                    new CommandeDAL().Annuler(id);
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
                if (statut == "retire")
                    new CommandeDAL().Retirer(id);
                else if (statut == "annule")
                    new CommandeDAL().Annuler(id);
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

        [HttpGet]
        public ActionResult PendantFermetures()
        {
            if (AdminCommande)
                return View(new ListeCommandesViewModel(new CommandeDAL().ListerCommandesPendantFermetures()));
            else
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
        }

        [HttpPost]
        public ActionResult PendantFermetures(int id)
        {
            if (AdminCommande && id == 0)
            {
                CommandeDAL commandeDAL = new CommandeDAL();
                List<Commande> commandes = commandeDAL.ListerCommandesPendantFermetures();
                foreach (Commande commande in commandes)
                {
                    int commandeId = commande.Id;
                    int clientId = commande.UtilisateurId;
                    if (clientId != 0)
                    {
                        Utilisateur utilisateur = new UtilisateurDAL().Details(clientId);
                        string objetMail = $"Problème commande {commandeId} : Fermeture de votre foodtruck";
                        string corpsMessage = $"Bonjour {utilisateur.Prenom}\n\n" +
                            $"Vous avez passé la commande numéro {commandeId} pour le {commande.DateRetrait.ToString("dddd dd MMMM yyyy à HH:mm").Replace(":", "h")} et nous vous en remercions.\n\n" +
                            $"Malheureusement nous ne sommes plus ouvert pendant votre horaire de retrait et nous avons été contraint de l'annuler.\n\n" +
                            $"Nous vous invitons à choisir un autre créneau de retrait (vous pouvons dupliquer votre commande annulée dans votre espace client).\n\n" +
                            $"Nous vous prions de nous excuser pour la gène occasionnée.\n\n" +
                            $"Bien cordialement\n" +
                            $"Votre équipe Foodtrucklyon";
                        string adresseMailClient = utilisateur.Email;
                        Utilitaire.EnvoieMail(adresseMailClient, objetMail, corpsMessage);
                        commandeDAL.Annuler(commandeId);
                    }
                }
                return RedirectToAction(ActionNom);
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
        }
    }
}