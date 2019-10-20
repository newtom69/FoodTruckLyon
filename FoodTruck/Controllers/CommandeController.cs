using FoodTruck.Models;
using System;
using System.Web.Mvc;
using FoodTruck.DAL;
using System.Net.Mail;
using System.Globalization;
using FoodTruck.ViewModels;

namespace FoodTruck.Controllers
{
    public class CommandeController : ControllerParent
    {
        [HttpPost]
        public ActionResult Index(DateTime dateRetrait)
        {
            if (VariablesSession.PanierViewModel.ArticlesDetailsViewModel.Count == 0)
            {
                return View(new Commande());
            }
            else
            {
                Commande commande = new Commande
                {
                    UtilisateurId = VariablesSession.Utilisateur.Id,
                    DateCommande = DateTime.Now,
                    DateRetrait = dateRetrait,
                    PrixTotal = 0
                };
                foreach (ArticleViewModel article in VariablesSession.PanierViewModel.ArticlesDetailsViewModel)
                {
                    commande.PrixTotal += article.Article.Prix * article.Quantite;
                    ArticleDAL larticleDAL = new ArticleDAL();
                    larticleDAL.AugmenterQuantiteVendue(article.Article.Id, 1);
                }
                CommandeDAL commandeDal = new CommandeDAL();
                commandeDal.Ajouter(commande, VariablesSession.PanierViewModel.ArticlesDetailsViewModel);
                Mail(VariablesSession.Utilisateur, commande, VariablesSession.PanierViewModel);
                PanierDAL panierDAL = new PanierDAL(VariablesSession.Utilisateur.Id);
                panierDAL.Supprimer();
                Session["Panier"] = null;
                return View(commande);
            }
        }

        private void Mail(Utilisateur lUtilisateur, Commande laCommande, PanierViewModel panier)
        {
            string lesArticlesDansLeMail = "";
            foreach (ArticleViewModel article in panier.ArticlesDetailsViewModel)
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
                using (MailMessage message = new MailMessage())
                {
                    message.From = new MailAddress("info@foodtrucklyon.fr");
                    message.To.Add("info@foodtrucklyon.fr");
                    message.ReplyToList.Add(emailClient);
                    message.Subject = "Nouvelle commande numéro " + numeroCommande;
                    message.Body = $"Nouvelle commande {numeroCommande}. Merci de la préparer pour le {laCommande.DateRetrait.ToString("dddd dd MMMM HH:mm")}\n" + corpsDuMailEnCommunClientFoodtruck;
                    using (SmtpClient client = new SmtpClient())
                    {
                        client.EnableSsl = false;
                        client.Send(message);
                    }
                }
                if (lUtilisateur.Id != 0)
                {
                    using (MailMessage message = new MailMessage())
                    {
                        message.From = new MailAddress("info@foodtrucklyon.fr");
                        message.To.Add(emailClient);
                        message.Subject = " Nouvelle commande FoodTruckLyon prise en compte";
                        message.Body = $"Bonjour {lUtilisateur.Prenom}\nVotre dernière commande a bien été prise en compte." +
                            $"\nVous pourrez venir la chercher le {laCommande.DateRetrait.ToString("dddd dd MMMM")}" +
                            $" à partir de {laCommande.DateRetrait.ToString("HH:mm").Replace(":", "h")}"+
                                       $"\nMerci de votre confiance\n\n" +
                                       "voici le récapitulatif : \n" + corpsDuMailEnCommunClientFoodtruck;
                        using (SmtpClient client = new SmtpClient())
                        {
                            client.EnableSsl = false;
                            client.Send(message);
                        }
                    }
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
