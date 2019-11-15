using System.Web.Mvc;
using System.Net;
using System.Configuration;
using FoodTruck.Outils;
using System;
using FoodTruck.DAL;

namespace FoodTruck.Controllers
{
    public class AdministrerClientController : ControllerParentAdministrer
    {
        public ActionResult Client()
        {
            return View();
        }

        [HttpGet]
        public ActionResult DonnerDroitsAdmin()
        {
            if (AdminUtilisateur)
                return View();
            else
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
        }

        [HttpPost]
        public ActionResult DonnerDroitsAdmin(string email, string nom, string prenom, string telephone)
        {
            int dureeValidite = int.Parse(ConfigurationManager.AppSettings["DureeValiditeLienDroitsAdmin"]);
            string codeVerification = Guid.NewGuid().ToString("n");
            string url = $"www.foodtrucklyon.fr/Compte/ObtenirDroitsAdmin/{codeVerification}"; //TODO plus propre
            Utilisateur utilisateur = new Utilisateur
            {
                Email = email,
                Nom = nom,
                Prenom = prenom,
                Telephone = telephone,
            };
            new AdminTemporaireDAL().Ajouter(utilisateur, codeVerification, DateTime.Now.AddMinutes(dureeValidite));

            string sujetMail = "Vous avez les droits d'administration";
            string message = $"Bonjour {prenom} {nom}\n\n" +
                "Un administrateur de foodtrucklyon vous a donné les droits d'accès administrateur au site.\n" +
                "Veuillez cliquer sur le lien suivant ou recopier l'adresse dans votre navigateur afin de valider l'action:\n" +
                "\n" +
                url +
                "\n\n" +
                $"Attention, ce lien expirera dans {dureeValidite / 60} heures et n'est valable que pour l'adresse {email}";

            if (Utilitaire.EnvoieMail(email, sujetMail, message))
                TempData["mailEnvoyeOk"] = $"Un email de confirmation vient d'être envoyé au destinataire. Il expirera dans {dureeValidite / 60} heures";
            else
                TempData["mailEnvoyeKo"] = "Erreur dans l'envoi du mail, veuillez rééssayer dans quelques instants";

            return View();
        }
    }
}