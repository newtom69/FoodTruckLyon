using FoodTruck.Models;
using System;
using System.Web.Mvc;
using FoodTruck.DAL;
using System.Net.Mail;
using System.Globalization;
using FoodTruck.ViewModels;

namespace FoodTruck.Controllers
{
    public class CommandeController : Controller
    {
        // GET: Commande
        [HttpPost]
        public ActionResult Index()
        {
            SessionVariables session = new SessionVariables();
            ViewBag.Panier = session.PanierViewModel;
            ViewBag.Utilisateur = session.Utilisateur;

            ViewBag.pasDePanier = false;
            ViewBag.pasDeClient = false;

            if (session.PanierViewModel.ArticlesDetailsViewModel.Count==0)
            {
                ViewBag.pasDePanier = true;
                return View();
            }
            if (session.Utilisateur.Id==0)
            {
                ViewBag.pasDeClient = true;
                return View();
            }


            Commande laCommande = new Commande
            {
                UtilisateurId = session.Utilisateur.Id,
                DateCommande = DateTime.Now,
                ModeLivraison = "à notre Foodtruck",
                DateLivraison = DateTime.Now.AddMinutes(45), //TODO
                PrixTotal = 0
            };

            foreach (ArticleDetailsViewModel article in session.PanierViewModel.ArticlesDetailsViewModel)
            {
                laCommande.PrixTotal += article.Article.Prix * article.Quantite;
                ArticleDAL larticleDAL = new ArticleDAL();
                larticleDAL.AugmenterQuantiteVendue(article.Article.Id, 1);
            }

            CommandeDAL laCommandeDal = new CommandeDAL();
            laCommandeDal.Ajouter(laCommande, session.PanierViewModel.ArticlesDetailsViewModel);
            
            Mail(session.Utilisateur, laCommande, session.PanierViewModel);

            ViewBag.laCommande = laCommande;
            Session["Panier"] = null;
            PanierDAL lePanierDAL = new PanierDAL(session.Utilisateur.Id);
            lePanierDAL.Supprimer();
            
            VisiteDAL.Enregistrer(session.Utilisateur.Id);
            return View();
        }


        public void Mail(Utilisateur lUtilisateur, Commande laCommande, PanierViewModel panier)
        {

            string lesArticlesDansLeMail = "";
            foreach (ArticleDetailsViewModel article in panier.ArticlesDetailsViewModel)
                lesArticlesDansLeMail += "\n" + article.Quantite + " x " + article.Article.Nom + " = " +
                                         (article.Quantite * article.Article.Prix).ToString("C2", new CultureInfo("fr-FR"));

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
