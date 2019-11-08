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
                new CommandeDAL().MettreAJourStatut(id, statut == "retire", statut == "annule");
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
                new CommandeDAL().MettreAJourStatut(id, statut == "retire", statut == "annule");
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
        public ActionResult PendantFermetures(int id, string statut)
        {
            if (AdminCommande)
            {
                if (id != 0)
                {
                    new CommandeDAL().MettreAJourStatut(id, statut == "retire", statut == "annule");
                    return RedirectToAction(ActionNom);
                }
                else
                {
                    List<Commande> commandes = new CommandeDAL().ListerCommandesPendantFermetures();
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
                                $"Malheureusement nous ne sommes plus ouvert pendant votre horaire de retrait.\n\n" +
                                $"Nous vous invitons donc à choisir un autre créneau de retrait en dupliquant votre commande que vous aurez préalablement annulée.\n\n" +
                                $"Nous vous prions de nous excuser pour la gène occasionnée.\n\n" +
                                $"Bien cordialement\n" +
                                $"Votre équipe Foodtrucklyon";
                            string adresseMailClient = utilisateur.Email;
                            Utilitaire.EnvoieMail(adresseMailClient, objetMail, corpsMessage);
                        }
                    }
                    return RedirectToAction(ActionNom);
                }
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
        }
    }
}