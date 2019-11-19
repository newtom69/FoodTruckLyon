using System.Web.Mvc;
using System.Net;
using System.Configuration;
using FoodTruck.Outils;
using System;
using FoodTruck.DAL;
using FoodTruck.ViewModels;

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
        public ActionResult DonnerDroitsAdmin(string email, string nom, string prenom)
        {
            int dureeValidite = int.Parse(ConfigurationManager.AppSettings["DureeValiditeLienDroitsAdmin"]);
            string codeVerification = Guid.NewGuid().ToString("n");
            string url = $"{Request.Url.Scheme}://{Request.Url.Authority}/Compte/ObtenirDroitsAdmin/{codeVerification}";

            Utilisateur utilisateur = new Utilisateur
            {
                Email = email,
                Nom = nom,
                Prenom = prenom,
            };
            new CreerAdminDAL().Ajouter(utilisateur, codeVerification, DateTime.Now.AddMinutes(dureeValidite));
            string sujetMail = "Vous avez les droits d'administration";
            string message = $"Bonjour {prenom} {nom}\n\n" +
                "Un administrateur de foodtrucklyon vous a donné les droits d'accès administrateur au site.\n" +
                "Veuillez cliquer sur le lien suivant ou recopier l'adresse dans votre navigateur afin de valider l'action:\n" +
                "\n" +
                url +
                "\n\n" +
                $"Attention, ce lien expirera dans {dureeValidite / 60} heures et n'est valable que pour l'adresse {email}";
            if (Utilitaire.EnvoieMail(email, sujetMail, message))
                TempData["message"] = new Message($"Un email de confirmation vient d'être envoyé à l'adresse {email}.\nIl expirera dans {dureeValidite / 60} heures", TypeMessage.Info);
            else
                TempData["message"] = new Message("Erreur dans l'envoi du mail.\nVeuillez rééssayer plus tard", TypeMessage.Erreur);

            return View();
        }
    }
}