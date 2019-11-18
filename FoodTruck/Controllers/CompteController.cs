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
            if (Utilisateur.Id == 0)
                return RedirectToAction("Connexion", "Compte");
            else
                return RedirectToAction("Profil");
        }

        [HttpGet]
        public ActionResult Profil()
        {
            if (TempData["PanierViewModelSauv"] != null)
            {
                int utilisateurId = (int)Session["UtilisateurId"];
                if (utilisateurId != 0)
                {
                    PanierDAL lePanierDal = new PanierDAL(utilisateurId);
                    foreach (ArticleViewModel article in (TempData["PanierViewModelSauv"] as PanierViewModel).ArticlesDetailsViewModel)
                    {
                        Panier panier = lePanierDal.ListerPanierUtilisateur().Find(pan => pan.ArticleId == article.Article.Id);
                        if (panier == null)
                            lePanierDal.Ajouter(article.Article, article.Quantite);
                        else
                            lePanierDal.ModifierQuantite(article.Article, article.Quantite);
                    }
                }
                TempData["PanierViewModelSauv"] = null;
                return RedirectToAction(ActionNom, ControllerNom);
            }

            CommandeDAL commandeDAL = new CommandeDAL();
            List<Commande> commandes = commandeDAL.CommandesEnCoursUtilisateur(Utilisateur.Id);
            ViewBag.ListeCommandesEnCours = new ListeCommandesViewModel(commandes);
            ViewBag.RemiseTotalUtilisateur = commandeDAL.RemiseTotaleUtilisateur(Utilisateur.Id);
            return View(Utilisateur);
        }

        [HttpPost]
        public ActionResult Profil(string ancienEmail, string email, string ancienMdp, string nom, string prenom, string telephone, string mdp, string mdp2)
        {
            UtilisateurDAL utilisateurDAL = new UtilisateurDAL();
            Utilisateur utilisateur = utilisateurDAL.Connexion(ancienEmail, ancienMdp);
            if (utilisateur == null)
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
                    if (utilisateurDAL.Modification(utilisateur.Id, nouveauMdp, email, nom, prenom, telephone) == 1)
                    {
                        TempData["message"] = new Message("La modification du profil a bien été prise en compte.", TypeMessage.Ok);
                        Utilisateur = utilisateurDAL.Connexion(email, nouveauMdp);
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
            List<Commande> commandes = commandeDAL.CommandesEnCoursUtilisateur(Utilisateur.Id);
            ViewBag.ListeCommandesEnCours = new ListeCommandesViewModel(commandes);
            ViewBag.RemiseTotalUtilisateur = commandeDAL.RemiseTotaleUtilisateur(Utilisateur.Id);
            return View(Utilisateur);
        }

        [HttpGet]
        public ActionResult Commandes()
        {
            if (Utilisateur.Id != 0)
            {
                List<Commande> commandes = new CommandeDAL().CommandesUtilisateur(Utilisateur.Id);
                ListeCommandesViewModel listeCommandesViewModel = new ListeCommandesViewModel(commandes);
                return View(listeCommandesViewModel);
            }
            else
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
        }

        [HttpPost]
        public ActionResult AnnulerCommande(int commandeId)
        {
            CommandeDAL commandeDAL = new CommandeDAL();
            Commande commande = commandeDAL.Detail(commandeId);
            if (commande != null && commande.UtilisateurId == Utilisateur.Id)
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
            if (commande != null && commande.UtilisateurId == Utilisateur.Id)
            {
                List<ArticleViewModel> articles = commandeDAL.Articles(commandeId);
                if (viderPanier)
                {
                    new PanierDAL(Utilisateur.Id).Supprimer();
                    PanierViewModel.Initialiser();
                    ViewBag.Panier = null; //todo
                }
                List<Article> articlesKo = new List<Article>();
                foreach (var a in articles)
                {
                    if (!PanierViewModel.Ajouter(a.Article, a.Quantite, Utilisateur.Id, ProspectGuid))
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
            }
            RecupererPanierEnBase();
            ViewBag.Panier = PanierViewModel;
            return RedirectToAction("Commandes", "Compte");
        }

        [HttpGet]
        public ActionResult Connexion()
        {
            if (Utilisateur.Id == 0)
                return View();
            else
                return RedirectToAction("Profil");
        }

        [HttpPost]
        public ActionResult Connexion(string email, string mdp, bool connexionAuto)
        {
            Utilisateur = new UtilisateurDAL().Connexion(email, mdp);
            if (Utilisateur != null)
            {
                ViewBag.Utilisateur = Utilisateur;
                Session["UtilisateurId"] = Utilisateur.Id;
                if (connexionAuto)
                {
                    HttpCookie cookie = new HttpCookie("GuidClient")
                    {
                        Value = Utilisateur.Guid,
                        Expires = DateTime.Now.AddDays(30)
                    };
                    Response.Cookies.Add(cookie);
                }
                RecupererPanierProspectPuisSupprimer();
                SupprimerCookieProspect();
                string message = $"Bienvenue {Utilisateur.Prenom} {Utilisateur.Nom}\n"+
                                 $"Vous avez {Utilisateur.Cagnotte} € sur votre cagnotte fidélité\n" +
                                 $"Depuis votre inscription, vous avez eu {new CommandeDAL().RemiseTotaleUtilisateur(Utilisateur.Id).ToString("C2", new CultureInfo("fr-FR"))} de remises sur vos commandes";
                TempData["message"] = new Message(message, TypeMessage.Ok);
                return RedirectPermanent(Session["Url"] as string);
            }
            else
            {
                ViewBag.Utilisateur = new Utilisateur();
                TempData["message"] = new Message("Email ou mot de passe incorrect.\nVeuillez réessayer.", TypeMessage.Erreur);
                return View();
            }
        }

        [HttpGet]
        public ActionResult Deconnexion()
        {
            if (Utilisateur.Id != 0)
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
            Utilisateur utilisateur = Utilisateur;
            if (Utilisateur.Id == 0)
            {
                if (VerifMdp(mdp, mdp2))
                {
                    utilisateur = new UtilisateurDAL().Creation(email, mdp, nom, prenom, telephone);
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

            if (utilisateur != null)
            {
                Connexion(email, mdp, connexionAuto);
                return RedirectToAction("Profil");
            }
            else
            {
                ViewBag.Utilisateur = new Utilisateur();
                TempData["message"] = new Message("Compte déjà existant.\nVeuillez saisir une autre adresse mail ou vous <a href=\"/Compte/Connexion\">connecter</a>", TypeMessage.Erreur);
                return View();
            }
        }

        [HttpGet]
        public ActionResult OubliMotDePasse(string codeVerification)
        {
            UtilisateurDAL utilisateurDAL = new UtilisateurDAL();
            int utilisateurId = new OubliMotDePasseDAL().Verifier(codeVerification);
            if (utilisateurId != 0)
            {
                Utilisateur = utilisateurDAL.Details(utilisateurId);
                ViewBag.Utilisateur = Utilisateur;
                Session["UtilisateurId"] = Utilisateur.Id;
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
                Utilisateur utilisateur = new UtilisateurDAL().Details(email);
                if (utilisateur != null)
                {
                    new OubliMotDePasseDAL().Ajouter(utilisateur.Id, codeVerification, DateTime.Now.AddMinutes(dureeValidite));
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
                    UtilisateurDAL utilisateurDAL = new UtilisateurDAL();
                    if (utilisateurDAL.Modification(Utilisateur.Id, mdp) == 1)
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
                List<Commande> commandes = commandeDAL.CommandesEnCoursUtilisateur(Utilisateur.Id);
                ViewBag.ListeCommandesEnCours = new ListeCommandesViewModel(commandes);
                ViewBag.RemiseTotalUtilisateur = commandeDAL.RemiseTotaleUtilisateur(Utilisateur.Id);
                return RedirectToAction("Connexion", "Compte");
            }
            else
                return RedirectToAction("Connexion", "Compte");
        }

        [HttpGet]
        public ActionResult ObtenirDroitsAdmin(string codeVerification)
        {
            UtilisateurDAL utilisateurDAL = new UtilisateurDAL();
            CreerAdmin creerAdmin = new CreerAdminDAL().Verifier(codeVerification);
            if (creerAdmin != null)
            {
                Utilisateur = utilisateurDAL.Details(creerAdmin.Email);
                if (Utilisateur == null)
                {
                    byte[] data = new byte[10];
                    using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
                    {
                        rng.GetBytes(data);
                    }
                    string mdp = (Encoding.UTF8.GetString(data)).GetHash();
                    string telephone = "";
                    Utilisateur = utilisateurDAL.Creation(creerAdmin.Email, mdp, creerAdmin.Nom, creerAdmin.Prenom, telephone);
                }
                utilisateurDAL.DonnerDroitAdmin(Utilisateur.Id);
                {
                    string emailFoodtruck = "info@foodtrucklyon.fr"; // todo passer dans config
                    string objet = $"{Utilisateur.Prenom.Trim()} {Utilisateur.Nom.Trim()} a abtenu les droit admin";
                    string message = $"l'utilisateur {Utilisateur.Prenom.Trim()} {Utilisateur.Nom.Trim()} a obtenu les droits admin";
                    Utilitaire.EnvoieMail(emailFoodtruck, objet, message);
                }
                ViewBag.Utilisateur = Utilisateur;
                Session["UtilisateurId"] = Utilisateur.Id;
                RecupererPanierProspectPuisSupprimer();
                SupprimerCookieProspect();
                TempData["message"] = new Message("Félicitation ! Vous êtes maintenant administrateur du site.\nVous pouvez accéder au menu Administration", TypeMessage.Info);
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
            TempData["PanierViewModelSauv"] = new PanierViewModel(panierProspectDAL.ListerPanierProspect());
            panierProspectDAL.Supprimer();
        }
    }
}
