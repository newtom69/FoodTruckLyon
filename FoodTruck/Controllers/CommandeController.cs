using FoodTruck.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FoodTruck.DAL;
using System.Net.Mail;
using System.Globalization;

namespace FoodTruck.Controllers
{
    public class CommandeController : Controller
    {
        // GET: Commande
        public ActionResult Index()
        {
            ViewBag.PanierAbsent = false;
            ViewBag.pasDePanier = false;
            ViewBag.pasDeClient = false;

            if (this.Session["MonPanier"] == null)
            {
                ViewBag.pasDePanier = true;
                return View();
            }

            Panier lePanier = (Panier) this.Session["MonPanier"];

            if (this.Session["Utilisateur"] == null)
            {
                ViewBag.pasDeClient = true;
                return View();
            }

            Utilisateur lUtilisateur = (Utilisateur) this.Session["Utilisateur"];
            ViewBag.lUtilisateur = lUtilisateur;


            Commande laCommande = new Commande
            {
                UtilisateurId = lUtilisateur.Id,
                DateCommande = DateTime.Now,
                ModeLivraison = "à notre Foodtruck",
                DateLivraison = DateTime.Now.AddMinutes(45), //TODO
                listeArticles = lePanier.listeArticles,
                PrixTotal = 0
            };

            foreach (Article article in lePanier.listeArticles)
            {
                laCommande.PrixTotal += article.Prix * article.Quantite;
                ArticleDAL larticleDAL = new ArticleDAL();
                larticleDAL.AugmenterQuantiteVendue(article.Id, article.Quantite);
            }

            CommandeDAL laCommandeDal = new CommandeDAL();
            laCommandeDal.Ajouter(laCommande);
            
            Mail(lUtilisateur, laCommande, lePanier);

            Session["MaCommande"] = laCommande;
            ViewBag.laCommande = laCommande;
            lePanier = new Panier();
            Session["MonPanier"] = null;
            PanierDAL lePanierDAL = new PanierDAL(lUtilisateur.Id);
            lePanierDAL.Supprimer();

            return View();
        }


        public void Mail(Utilisateur lUtilisateur, Commande laCommande, Panier lePanier)
        {
            string lesArticlesDansLeMail = "";
            foreach (Article article in lePanier.listeArticles)
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
                MailMessage message = new MailMessage();
                message.From = new MailAddress("info@foodtruck-lyon.com");
                message.To.Add("info@foodtruck-lyon.com");
                message.Subject = "Nouvelle commande";
                message.Body = $"Nouvelle commande d'un client. Merci de lui préparer pour le {laCommande.DateLivraison}\n" + corpsDuMailEnCommunClientFoodtruck;

                SmtpClient client = new SmtpClient();
                client.EnableSsl = true;
                client.Send(message);
            }
            catch (Exception)
            {
                Response.StatusCode = 400;
                ViewBag.Mail = "Erreur dans l'envoi de la commande veuillez rééssayer s'il vous plait";
            }

            try
            {
                MailMessage message = new MailMessage();
                message.From = new MailAddress("info@foodtruck-lyon.com");
                message.To.Add(emailClient);
                message.Subject = " Nouvelle commande FoodTruck-Lyon prise en compte";
                message.Body = $"Bonjour {lUtilisateur.Prenom}\nVotre dernière commande a bien été prise en compte." +
                               $"\nMerci de votre confiance\n\n" +
                               "voici le récapitulatif : \n" + corpsDuMailEnCommunClientFoodtruck;

                SmtpClient client = new SmtpClient();
                client.EnableSsl = true;
                client.Send(message);
            }
            catch (Exception)
            {
                Response.StatusCode = 400;
                ViewBag.Mail = "Erreur dans l'envoi de la commande veuillez rééssayer s'il vous plait";
            }
        }
    }
}
