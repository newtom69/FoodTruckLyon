using FoodTruck.Models;
using System;
using System.Web.Mvc;
using FoodTruck.DAL;
using System.Net.Mail;
using System.Globalization;

namespace FoodTruck.Controllers
{
    public class CommandeController : Controller
    {
        // GET: Commande
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.PanierAbsent = false;
            ViewBag.pasDePanier = false;
            ViewBag.pasDeClient = false;

            if (Session["Panier"] == null)
            {
                ViewBag.pasDePanier = true;
                return View();
            }

            PanierUI lePanier = (PanierUI)Session["Panier"];

            if (Session["Utilisateur"] == null)
            {
                ViewBag.pasDeClient = true;
                return View();
            }

            Utilisateur lUtilisateur = (Utilisateur)Session["Utilisateur"];
            ViewBag.lUtilisateur = lUtilisateur;

            Commande laCommande = new Commande
            {
                UtilisateurId = lUtilisateur.Id,
                DateCommande = DateTime.Now,
                ModeLivraison = "à notre Foodtruck",
                DateLivraison = DateTime.Now.AddMinutes(45), //TODO
                PrixTotal = 0
            };

            foreach (ArticleUI article in lePanier.ListeArticlesUI)
            {
                laCommande.PrixTotal += article.Prix * article.Quantite;
                ArticleDAL larticleDAL = new ArticleDAL();
                larticleDAL.AugmenterQuantiteVendue(article.Id, 1);
            }

            CommandeDAL laCommandeDal = new CommandeDAL();
            laCommandeDal.Ajouter(laCommande, lePanier.ListeArticlesUI);
            
            Mail(lUtilisateur, laCommande, lePanier);

            ViewBag.laCommande = laCommande;
            Session["Panier"] = null;
            PanierDAL lePanierDAL = new PanierDAL(lUtilisateur.Id);
            lePanierDAL.Supprimer();

            return View();
        }


        public void Mail(Utilisateur lUtilisateur, Commande laCommande, PanierUI lePanier)
        {

            string lesArticlesDansLeMail = "";
            foreach (ArticleUI article in lePanier.ListeArticlesUI)
                lesArticlesDansLeMail += "\n" + article.Quantite + " x " + article.Nom + " = " +
                                         (article.Quantite * article.Prix).ToString("C2", new CultureInfo("fr-FR"));

            string nomClient = lUtilisateur.Nom;
            string prenomClient = lUtilisateur.Prenom;
            string emailClient = lUtilisateur.Email;

            string corpsDuMailEnCommunClientFoodtruck =
                $"Nom : {nomClient}\nPrénom : {prenomClient}\nEmail : {emailClient}\n\nArticles :{lesArticlesDansLeMail}" +
                $"\nTotal de la commande : {laCommande.PrixTotal.ToString("C2", new CultureInfo("fr-FR"))}";

            try
            {
                MailMessage message = new MailMessage
                {
                    From = new MailAddress("info@foodtruck-lyon.com")
                };
                message.To.Add("info@foodtruck-lyon.com");
                message.Subject = "Nouvelle commande";
                message.Body = $"Nouvelle commande d'un client. Merci de lui préparer pour le {laCommande.DateLivraison}\n" + corpsDuMailEnCommunClientFoodtruck;

                using (SmtpClient client = new SmtpClient
                {
                    EnableSsl = true
                })
                {
                    client.Send(message);
                }
            }
            catch (Exception)
            {
                Response.StatusCode = 400;
                ViewBag.Mail = "Erreur dans l'envoi de la commande veuillez rééssayer s'il vous plait";
            }

            try
            {
                MailMessage message = new MailMessage
                {
                    From = new MailAddress("info@foodtruck-lyon.com")
                };
                message.To.Add(emailClient);
                message.Subject = " Nouvelle commande FoodTruck-Lyon prise en compte";
                message.Body = $"Bonjour {lUtilisateur.Prenom}\nVotre dernière commande a bien été prise en compte." +
                               $"\nMerci de votre confiance\n\n" +
                               "voici le récapitulatif : \n" + corpsDuMailEnCommunClientFoodtruck;

                using (SmtpClient client = new SmtpClient { EnableSsl = true })
                {
                    client.Send(message);
                }
            }
            catch (Exception)
            {
                Response.StatusCode = 400;
                ViewBag.Mail = "Erreur dans l'envoi de la commande veuillez rééssayer s'il vous plait";
            }
        }
    }
}
