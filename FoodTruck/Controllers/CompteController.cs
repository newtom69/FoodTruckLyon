using FoodTruck.DAL;
using FoodTruck.Outils;
using FoodTruck.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace FoodTruck.Controllers
{
    public class CompteController : ControllerParent
    {
        public CompteController()
        {
            ViewBag.PanierLatteralDesactive = true;
        }

        [HttpGet]
        public ActionResult Index()
        {
            if (Client.Id == 0)
                return RedirectToAction("Connexion", "Compte");
            else
                return RedirectToAction("Profil");
        }

        [HttpGet]
        public ActionResult Profil()
        {
            ViewBag.RemiseTotalUtilisateur = new CommandeDAL().RemiseTotaleUtilisateur(Client.Id);
            return View(Client);
        }

        [HttpPost]
        public ActionResult Profil(string ancienEmail, string email, string ancienMdp, string nom, string prenom, string telephone, string mdp, string mdp2)
        {
            ClientDAL utilisateurDAL = new ClientDAL();
            Client client = utilisateurDAL.Connexion(ancienEmail, ancienMdp);
            if (client == null)
            {
                TempData["message"] = new Message("L'ancien mot de passe n'est pas correct.\nAucune modification n'a été prise en compte.", TypeMessage.Erreur);
            }
            else
            {
                string nouveauMdp;
                if (mdp == "" && mdp2 == "")
                    nouveauMdp = ancienMdp;
                else if (VerifMdp(mdp, mdp2))
                    nouveauMdp = mdp;
                else
                    nouveauMdp = "";

                if (nouveauMdp != "")
                {
                    if (utilisateurDAL.Modification(client.Id, nouveauMdp, email, nom, prenom, telephone) == 1)
                    {
                        TempData["message"] = new Message("La modification du profil a bien été prise en compte.", TypeMessage.Ok);
                        Client = utilisateurDAL.Connexion(email, nouveauMdp);
                    }
                }
                else
                {
                    TempData["message"] = new Message("Mauvais choix de mots de passe.\nVeuillez réessayer s'il vous plait (minimum 8 caractères et identiques)", TypeMessage.Erreur);
                    ViewBag.Nom = nom;
                    ViewBag.Prenom = prenom;
                    ViewBag.Email = email;
                    ViewBag.Telephone = telephone;
                }
            }
            CommandeDAL commandeDAL = new CommandeDAL();
            ViewBag.RemiseTotalUtilisateur = commandeDAL.RemiseTotaleUtilisateur(Client.Id);
            return View(Client);
        }

        [HttpGet]
        public ActionResult Commandes()
        {
            if (Client.Id != 0)
                return View();
            else
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
        }

        [HttpPost]
        public ActionResult AnnulerCommande(int commandeId)
        {
            CommandeDAL commandeDAL = new CommandeDAL();
            Commande commande = commandeDAL.Detail(commandeId);
            if (commande != null && commande.ClientId == Client.Id)
            {
                commandeDAL.Annuler(commandeId);
            }
            return RedirectToAction("Commandes", "Compte");
        }

        [HttpPost]
        public ActionResult ReprendreArticlesCommande(int commandeId, bool viderPanier)
        {
            CommandeDAL commandeDAL = new CommandeDAL();
            Commande commande = commandeDAL.Detail(commandeId);
            if (commande != null && commande.ClientId == Client.Id)
            {
                List<ArticleViewModel> articles = commandeDAL.Articles(commandeId);
                if (viderPanier)
                {
                    new PanierDAL(Client.Id).Supprimer();
                    PanierViewModel.Initialiser();
                    ViewBag.Panier = null; //todo
                }
                List<Article> articlesKo = new List<Article>();
                foreach (var a in articles)
                {
                    if (!PanierViewModel.Ajouter(a.Article, a.Quantite, Client.Id, ProspectGuid))
                        articlesKo.Add(a.Article);
                }
                ViewBag.Panier = PanierViewModel;
                TempData["ArticlesNonAjoutes"] = articlesKo;
                if (articlesKo.Count > 0)
                {
                    string dossierImagesArticles = ConfigurationManager.AppSettings["PathImagesArticles"];
                    string message = "Les articles suivants ne peuvent pas être repris car ils ne sont plus disponibles :" +
                    "<div class=\"gestionCommandeArticle\">" +
                    "<section class=\"imagesGestionCommande\">";
                    foreach (Article article in articlesKo)
                    {
                        message += "<div class=\"indexArticle\">" +
                        $"<img src=\"{dossierImagesArticles}/{article.Image}\" alt=\"{article.Nom}\" /> " +
                        $"<p>{article.Nom}</p>" +
                        $"</div>";
                    }
                    message += "</section>" +
                    "</div>";
                    TempData["message"] = new Message(message, TypeMessage.Info); // TODO faire plus propre et ailleurs (formatage html propre à la vue)
                }
                else
                {
                    TempData["message"] = new Message($"La reprise des {articles.Count} articles de votre commande s'est correctement réalisée", TypeMessage.Ok);
                }
            }
            RecupererPanierEnBase();
            ViewBag.Panier = PanierViewModel;
            return RedirectToAction("Commandes", "Compte");
        }

        [HttpGet]
        public ActionResult Connexion()
        {
            if (Client.Id == 0)
                return View();
            else
                return RedirectToAction("Profil");
        }

        [HttpPost]
        public ActionResult Connexion(string email, string mdp, bool connexionAuto)
        {
            Client = new ClientDAL().Connexion(email, mdp);
            if (Client != null)
            {
                ViewBag.Client = Client;
                Session["ClientId"] = Client.Id;
                if (connexionAuto)
                    ConnexionAutomatique();

                RecupererPanierProspectPuisSupprimer();
                SupprimerCookieProspect();
                string message = $"Bienvenue {Client.Prenom} {Client.Nom}\nVous avez {Client.Cagnotte} € sur votre cagnotte fidélité\nDepuis votre inscription du {Client.Inscription.ToString("dd MMMM yyyy")}, vous avez eu {new CommandeDAL().RemiseTotaleUtilisateur(Client.Id).ToString("C2", new CultureInfo("fr-FR"))} de remises sur vos commandes";
                TempData["message"] = new Message(message, TypeMessage.Ok);
                return Redirect(UrlCourante());
            }
            else
            {
                ViewBag.Client = new Client();
                TempData["message"] = new Message("Email ou mot de passe incorrect.\nVeuillez réessayer.", TypeMessage.Erreur);
                return View();
            }
        }

        [HttpGet]
        public ActionResult Deconnexion()
        {
            if (Client.Id != 0)
            {
                HttpCookie newCookie = new HttpCookie("GuidClient")
                {
                    Expires = DateTime.Now.AddDays(-30)
                };
                Response.Cookies.Add(newCookie);
                InitialiserSession();
                ViewBag.Panier = null; // todo
            }
            TempData["message"] = new Message("Vous êtes maintenant déconnecté.\nMerci de votre visite.\nA bientôt.", TypeMessage.Info);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult Creation()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Creation(string email, string mdp, string mdp2, string nom, string prenom, string telephone, bool connexionAuto)
        {
            Client client = Client;
            if (Client.Id == 0)
            {
                if (VerifMdp(mdp, mdp2))
                {
                    client = new ClientDAL().Creation(email, mdp, nom, prenom, telephone);
                }
                else
                {
                    TempData["message"] = new Message("Mauvais choix de mots de passe.\nVeuillez réessayer (minimum 8 caractères et identiques)", TypeMessage.Erreur);
                    ViewBag.Nom = nom;
                    ViewBag.Prenom = prenom;
                    ViewBag.Email = email;
                    ViewBag.Telephone = telephone;
                    return View();
                }
            }

            if (client != null)
            {
                Connexion(email, mdp, connexionAuto);
                return RedirectToAction("Profil");
            }
            else
            {
                ViewBag.Client = new Client();
                TempData["message"] = new Message("Compte déjà existant.\nVeuillez saisir une autre adresse mail ou vous <a href=\"/Compte/Connexion\">connecter</a>", TypeMessage.Erreur);
                return View();
            }
        }

        [HttpGet]
        public ActionResult OubliMotDePasse(string codeVerification)
        {
            ClientDAL utilisateurDAL = new ClientDAL();
            int utilisateurId = new OubliMotDePasseDAL().Verifier(codeVerification);
            if (utilisateurId != 0)
            {
                Client = utilisateurDAL.Details(utilisateurId);
                ViewBag.Client = Client;
                Session["ClientId"] = Client.Id;
                RecupererPanierProspectPuisSupprimer();
                SupprimerCookieProspect();
                return View();
            }
            else
            {
                TempData["message"] = new Message("Le lien de redéfinition du mot de passe n'est plus valide.\nVeuillez refaire une demande", TypeMessage.Interdit);
                return RedirectToAction("Connexion", "Compte");
            }
        }

        [HttpPost]
        public ActionResult OubliMotDePasse(string action, string email, string mdp, string mdp2)
        {
            if (action == "generationMail")
            {
                int dureeValidite = int.Parse(ConfigurationManager.AppSettings["DureeValiditeLienReinitialisationMotDePasse"]);
                string codeVerification = Guid.NewGuid().ToString("n") + email.GetHash();
                string url = HttpContext.Request.Url.ToString() + '/' + codeVerification;
                Client client = new ClientDAL().Details(email);
                if (client != null)
                {
                    new OubliMotDePasseDAL().Ajouter(client.Id, codeVerification, DateTime.Now.AddMinutes(dureeValidite));
                    string sujetMail = "Procédure de réinitialisation de votre mot de passe";
                    string message = "Bonjour\n" +
                        "Vous avez oublié votre mot de passe et avez demandé à le réinitialiser.\n" +
                        "Si vous êtes bien à l'origine de cette demande, veuillez cliquer sur le lien suivant ou recopier l'adresse dans votre navigateur :\n" +
                        "\n" +
                        url +
                        "\n\nVous serez alors redirigé vers une page de réinitialisation de votre mot de passe.\n" +
                        $"Attention, ce lien expirera dans {dureeValidite} minutes et n'est valable qu'une seule fois";

                    if (Utilitaire.EnvoieMail(email, sujetMail, message))
                        TempData["message"] = new Message($"Un email de réinitialisation de votre mot de passe vient de vous être envoyé.\nIl expirera dans {dureeValidite} minutes.", TypeMessage.Info);
                    else
                        TempData["message"] = new Message("Erreur dans l'envoi du mail.\nVeuillez réessayer dans quelques instants", TypeMessage.Erreur);
                }
                else
                    TempData["message"] = new Message("Nous n'avons pas de compte client avec cette adresse email.\nMerci de vérifier votre saisie", TypeMessage.Erreur);

                return RedirectToAction("Connexion", "Compte");
            }
            else if (action == "changementMotDePasse")
            {
                if (VerifMdp(mdp, mdp2))
                {
                    ClientDAL utilisateurDAL = new ClientDAL();
                    if (utilisateurDAL.Modification(Client.Id, mdp) == 1)
                    {
                        TempData["message"] = new Message("La modification de votre mot de passe a bien été prise en compte", TypeMessage.Ok);
                    }
                }
                else
                {
                    TempData["message"] = new Message("Mauvais choix de mots de passe.\nVeuillez réessayer (minimum 8 caractères et identiques)", TypeMessage.Erreur);
                    return View();
                }

                CommandeDAL commandeDAL = new CommandeDAL();
                ViewBag.RemiseTotalUtilisateur = commandeDAL.RemiseTotaleUtilisateur(Client.Id);
                return RedirectToAction("Connexion", "Compte");
            }
            else
                return RedirectToAction("Connexion", "Compte");
        }

        [HttpGet]
        public ActionResult ObtenirDroitsAdmin(string codeVerification)
        {
            ClientDAL utilisateurDAL = new ClientDAL();
            CreerAdmin creerAdmin = new CreerAdminDAL().Verifier(codeVerification);
            if (creerAdmin != null)
            {
                Client = utilisateurDAL.Details(creerAdmin.Email);
                if (Client == null)
                {
                    byte[] data = new byte[10];
                    using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
                    {
                        rng.GetBytes(data);
                    }
                    string mdp = Encoding.UTF8.GetString(data).GetHash();
                    string telephone = "";
                    Client = utilisateurDAL.Creation(creerAdmin.Email, mdp, creerAdmin.Nom, creerAdmin.Prenom, telephone);
                }
                utilisateurDAL.DonnerDroitAdmin(Client.Id);
                {
                    string mailFoodTruck = ConfigurationManager.AppSettings["MailFoodTruck"];
                    string objet = $"{Client.Prenom.Trim()} {Client.Nom.Trim()} a abtenu les droit admin";
                    string message = $"le client {Client.Prenom.Trim()} {Client.Nom.Trim()} a obtenu les droits admin";
                    Utilitaire.EnvoieMail(mailFoodTruck, objet, message);
                }
                ViewBag.Client = Client;
                Session["ClientId"] = Client.Id;
                RecupererPanierProspectPuisSupprimer();
                SupprimerCookieProspect();
                ConnexionAutomatique();
                TempData["message"] = new Message("Félicitation ! Vous êtes maintenant administrateur du site.\nVous pouvez accéder au menu Administration\nVous serez connecté automatiquement à partir de cet appareil/navigateur\nSi vous changez de poste/navigateur, merci d'utiliser la fonctionnalité \"Oubli de mot de passe\" afin de définir votre propre mot de passe", TypeMessage.Info);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["message"] = new Message("Le lien d'obtention des droits d'administration n'est plus valable.\nMerci de refaire une demande", TypeMessage.Interdit);
                return RedirectToAction("Index", "Home");
            }
        }

        private bool VerifMdp(string mdp1, string mdp2)
        {
            if (mdp1 != mdp2 || mdp1.Length < 8)
                return false;
            else
                return true;
        }

        private void SupprimerCookieProspect()
        {
            HttpCookie cookie = new HttpCookie("Prospect")
            {
                Expires = DateTime.Now.AddDays(-30)
            };
            Response.Cookies.Add(cookie);
        }

        private void RecupererPanierProspectPuisSupprimer()
        {
            PanierProspectDAL panierProspectDAL = new PanierProspectDAL(ProspectGuid);
            PanierViewModel panierViewModelSauv = new PanierViewModel(panierProspectDAL.ListerPanierProspect());
            if (panierViewModelSauv != null && Client.Id != 0)
            {
                PanierDAL panierDal = new PanierDAL(Client.Id);
                foreach (ArticleViewModel article in (panierViewModelSauv).ArticlesDetailsViewModel)
                {
                    Panier panier = panierDal.ListerPanierUtilisateur().Find(pan => pan.ArticleId == article.Article.Id);
                    if (panier == null)
                        panierDal.Ajouter(article.Article, article.Quantite);
                    else
                        panierDal.ModifierQuantite(article.Article, article.Quantite);
                }
            }
            panierProspectDAL.Supprimer();
        }

        private void ConnexionAutomatique()
        {
            HttpCookie cookie = new HttpCookie("GuidClient")
            {
                Value = Client.Guid,
                Expires = DateTime.Now.AddDays(30)
            };
            Response.Cookies.Add(cookie);
        }
    }
}
