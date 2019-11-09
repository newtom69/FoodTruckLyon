using FoodTruck.DAL;
using FoodTruck.Models;
using FoodTruck.Outils;
using FoodTruck.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
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
                return View(new ListeCommandesViewModel(new CommandeDAL().CommandesEnCours(fouchetteHeures)));
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
                return View(new ListeCommandesViewModel(new CommandeDAL().CommandesAStatuer()));
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
        public ActionResult Recherche()
        {
            if (AdminCommande)
            {
                ViewBag.DateDebut = DateTime.Today;
                ViewBag.DateFin = DateTime.Today;
                return View(new ListeCommandesViewModel(new CommandeDAL().CommandesToutes("", DateTime.Today, DateTime.Today.AddDays(3))));
            }
            else
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
        }

        [HttpPost]
        public ActionResult Recherche(string recherche, DateTime? dateDebut, DateTime? dateFin)
        {
            if (AdminCommande)
            {
                ViewBag.Recherche = recherche;
                ViewBag.DateDebut = dateDebut;
                ViewBag.DateFin = dateFin;

                string[] tabRecherche = recherche.Split(' ');
                List<Commande>[] tabCommandes = new List<Commande>[tabRecherche.Length];

                for (int i = 0; i < tabRecherche.Length; i++)
                    tabCommandes[i] = new CommandeDAL().CommandesToutes(tabRecherche[i], dateDebut, dateFin);

                List<Commande> commandes = tabCommandes[0];
                for (int i = 1; i < tabCommandes.Length; i++)
                    commandes = commandes.Intersect(tabCommandes[i], new CommandeEqualityComparer()).ToList();

                return View(new ListeCommandesViewModel(commandes));
            }
            else
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
        }

        [HttpGet]
        public ActionResult Futures()
        {
            if (AdminCommande)
                return View(new ListeCommandesViewModel(new CommandeDAL().CommandesFutures()));
            else
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
        }

        [HttpGet]
        public ActionResult PendantFermetures()
        {
            if (AdminCommande)
                return View(new ListeCommandesViewModel(new CommandeDAL().CommandesPendantFermetures()));
            else
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
        }

        [HttpPost]
        public ActionResult PendantFermetures(int id)
        {
            if (AdminCommande && id == 0)
            {
                CommandeDAL commandeDAL = new CommandeDAL();
                List<Commande> commandes = commandeDAL.CommandesPendantFermetures();
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