﻿using FoodTruck.DAL;
using FoodTruck.Models;
using FoodTruck.ViewModels;
using System;
using System.Globalization;
using System.Net.Mail;
using System.Web.Mvc;

namespace FoodTruck.Controllers
{
    public class CommandeController : ControllerParent
    {
        [HttpPost]
        public ActionResult Index(string codePromo, DateTime dateRetrait, int? remiseFidelite)
        {
            int montantRemiseFidelite = remiseFidelite ?? 0;
            double montantRemiseCommerciale = 0;
            CodePromoDAL codePromoDAL = new CodePromoDAL();
            codePromoDAL.Validite(codePromo, PanierViewModel.PrixTotal, ref montantRemiseCommerciale);

            if (PanierViewModel.ArticlesDetailsViewModel.Count == 0)
            {
                return View(new Commande());
            }
            else
            {
                int soldeCagnotte = new UtilisateurDAL().RetirerCagnotte(Utilisateur.Id, montantRemiseFidelite);
                if (soldeCagnotte == -1)
                    montantRemiseFidelite = 0;

                Commande commande = new Commande
                {
                    UtilisateurId = Utilisateur.Id,
                    DateCommande = DateTime.Now,
                    DateRetrait = dateRetrait,
                    PrixTotal = 0,
                    RemiseFidelite = montantRemiseFidelite,
                    RemiseCommerciale = montantRemiseCommerciale
                };
                foreach (ArticleViewModel article in PanierViewModel.ArticlesDetailsViewModel)
                {
                    commande.PrixTotal = Math.Round(commande.PrixTotal + article.Article.Prix * article.Quantite, 2);
                    new ArticleDAL().AugmenterQuantiteVendue(article.Article.Id, 1);
                }
                if (commande.PrixTotal > montantRemiseFidelite + montantRemiseCommerciale)
                {
                    commande.PrixTotal = Math.Round(commande.PrixTotal - montantRemiseFidelite - montantRemiseCommerciale, 2);
                }
                else
                {
                    commande.RemiseCommerciale = Math.Round(commande.PrixTotal - montantRemiseFidelite, 2);
                    commande.PrixTotal = 0;
                }

                new CommandeDAL().Ajouter(commande, PanierViewModel.ArticlesDetailsViewModel);
                Mail(Utilisateur, commande, PanierViewModel);
                new PanierDAL(Utilisateur.Id).Supprimer();
                ViewBag.Panier = null; //todo
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
                $"\nTotal de la commande : {laCommande.PrixTotal.ToString("C2", new CultureInfo("fr-FR"))}\n";
            
            if (laCommande.RemiseFidelite > 0)
                corpsDuMailEnCommunClientFoodtruck += $"\nRemise fidélité : {laCommande.RemiseFidelite.ToString("C2", new CultureInfo("fr-FR"))}";
            if (laCommande.RemiseCommerciale > 0)
                corpsDuMailEnCommunClientFoodtruck += $"\nRemise commerciale : {laCommande.RemiseCommerciale.ToString("C2", new CultureInfo("fr-FR"))}";
            
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
                            $" à partir de {laCommande.DateRetrait.ToString("HH:mm").Replace(":", "h")}" +
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
