using FoodTruck.DAL;
using FoodTruck.Outils;
using FoodTruck.ViewModels;
using System;
using System.Configuration;
using System.Globalization;
using System.Web.Mvc;

namespace FoodTruck.Controllers
{
    public class CommandeController : ControllerParent
    {
        [HttpPost]
        public ActionResult Index(string codePromo, DateTime dateRetrait, int? remiseFidelite)
        {
            int montantRemiseFidelite = remiseFidelite ?? 0;
            new CodePromoDAL().Validite(codePromo, PanierViewModel.PrixTotal, out double montantRemiseCommerciale);

            if (PanierViewModel.ArticlesDetailsViewModel.Count == 0)
            {
                return View(new Commande());
            }
            else
            {
                if (montantRemiseFidelite != 0 && Utilisateur.Id != 0)
                {
                    int soldeCagnotte = new UtilisateurDAL().RetirerCagnotte(Utilisateur.Id, montantRemiseFidelite);
                    if (soldeCagnotte == -1)
                        montantRemiseFidelite = 0;
                }
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
                string stringDateRetrait = commande.DateRetrait.ToString("dddd dd MMMM yyyy pour HH:mm");

                string message = $"Commande numéro {commande.Id} confirmée\nVeuillez venir la chercher le {stringDateRetrait}.";
                TypeMessage typeMessage;
                if (Utilisateur.Id == 0)
                {
                    message += $"\nAttention : Vous n'avez pas utilisé de compte client.\nMerci de bien noter votre numéro de commande.";
                    typeMessage = TypeMessage.Avertissement;
                }
                else
                {
                    message += "\nMerci";
                    typeMessage = TypeMessage.Ok;
                }
                TempData["message"] = new Message(message, typeMessage);
                return RedirectToAction("Index", "Home");
            }
        }

        private void Mail(Utilisateur utilisateur, Commande commande, PanierViewModel panier)
        {
            string mailFoodTruck = ConfigurationManager.AppSettings["MailFoodTruck"];
            string lesArticlesDansLeMail = "";
            foreach (ArticleViewModel article in panier.ArticlesDetailsViewModel)
                lesArticlesDansLeMail += "\n" + article.Quantite + " x " + article.Article.Nom + " = " + (article.Quantite * article.Article.Prix).ToString("C2", new CultureInfo("fr-FR"));

            CultureInfo cultureinfoFr = new CultureInfo("fr-FR");
            string nomClient = utilisateur.Nom ?? "non renseigné";
            string prenomClient = utilisateur.Prenom ?? "non renseigné";
            string emailClient = utilisateur.Email ?? "non@renseigne";
            string corpsDuMailEnCommunClientFoodtruck =
                $"Nom : {nomClient}\n" +
                $"Prénom : {prenomClient}\n" +
                $"Email : {emailClient}\n\n" +
                $"Articles :{lesArticlesDansLeMail}\n" +
                $"Total de la commande : {commande.PrixTotal.ToString("C2", cultureinfoFr)}\n";
            if (commande.RemiseFidelite > 0)
                corpsDuMailEnCommunClientFoodtruck += $"\nRemise fidélité : {commande.RemiseFidelite.ToString("C2", cultureinfoFr)}";
            if (commande.RemiseCommerciale > 0)
                corpsDuMailEnCommunClientFoodtruck += $"\nRemise commerciale : {commande.RemiseCommerciale.ToString("C2", cultureinfoFr)}";

            string sujet = $"Nouvelle commande numéro {commande.Id}";
            string corpsMail = $"Nouvelle commande {commande.Id}. Merci de la préparer pour le {commande.DateRetrait.ToString("dddd dd MMMM HH:mm")}\n" + corpsDuMailEnCommunClientFoodtruck;

            Utilitaire.EnvoieMail(mailFoodTruck, sujet, corpsMail);
            if (utilisateur.Id != 0)
            {
                string sujetMail2 = $"Nouvelle commande numéro {commande.Id} prise en compte";
                string corpsMail2 = $"Bonjour {utilisateur.Prenom}\n" +
                                    $"Votre dernière commande a bien été prise en compte." +
                                    $"\nVous pourrez venir la chercher le {commande.DateRetrait.ToString("dddd dd MMMM")}" +
                                    $" à partir de {commande.DateRetrait.ToString("HH:mm").Replace(":", "h")}" +
                                    $"\nMerci de votre confiance\n\n" +
                                    "voici le récapitulatif : \n" + corpsDuMailEnCommunClientFoodtruck;
                Utilitaire.EnvoieMail(emailClient, sujetMail2, corpsMail2);
            }
        }
    }
}
