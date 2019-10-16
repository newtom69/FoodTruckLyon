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
        [HttpPost]
        public ActionResult Index()
        {
            SessionVariables session = new SessionVariables();
            if (session.PanierViewModel.ArticlesDetailsViewModel.Count == 0)
            {
                return View(new Commande());
            }
            else
            {
                Commande commande = new Commande
                {
                    UtilisateurId = session.Utilisateur.Id,
                    DateCommande = DateTime.Now,
                    ModeLivraison = "à notre Foodtruck",
                    DateLivraison = DateTime.Now.AddMinutes(45), //TODO : commande avant 13h : livré le midi + "ecart / 13h" ; commande après 13h livré le soir
                    PrixTotal = 0
                };
                foreach (ArticleDetailsViewModel article in session.PanierViewModel.ArticlesDetailsViewModel)
                {
                    commande.PrixTotal += article.Article.Prix * article.Quantite;
                    ArticleDAL larticleDAL = new ArticleDAL();
                    larticleDAL.AugmenterQuantiteVendue(article.Article.Id, 1);
                }
                CommandeDAL commandeDal = new CommandeDAL();
                commandeDal.Ajouter(commande, session.PanierViewModel.ArticlesDetailsViewModel);
                Mail(session.Utilisateur, commande, session.PanierViewModel);
                PanierDAL panierDAL = new PanierDAL(session.Utilisateur.Id);
                panierDAL.Supprimer();
                Session["Panier"] = null;
                VisiteDAL.Enregistrer(session.Utilisateur.Id);
                return View(commande);
            }
        }

        private void Mail(Utilisateur lUtilisateur, Commande laCommande, PanierViewModel panier)
        {
            string lesArticlesDansLeMail = "";
            foreach (ArticleDetailsViewModel article in panier.ArticlesDetailsViewModel)
                lesArticlesDansLeMail += "\n" + article.Quantite + " x " + article.Article.Nom + " = " +
                                         (article.Quantite * article.Article.Prix).ToString("C2", new CultureInfo("fr-FR"));

            string nomClient = lUtilisateur.Nom;
            string prenomClient = lUtilisateur.Prenom;
            string emailClient = lUtilisateur.Email;
            string numeroCommande = laCommande.Id.ToString();
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
                message.Subject = "Nouvelle commande numéro " + numeroCommande;
                message.Body = $"Nouvelle commande {numeroCommande}. Merci de la préparer pour le {laCommande.DateLivraison}\n" + corpsDuMailEnCommunClientFoodtruck;

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

            if (lUtilisateur.Id != 0)
            {
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
}
