using FoodTruck.DAL;
using FoodTruck.Models;
using FoodTruck.Outils;
using FoodTruck.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;

namespace FoodTruck.Controllers
{
    public class CompteController : ControllerParent
    {
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
                ViewBag.MauvaisEmailMdp = true;
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
                        ViewBag.Modification = true;
                        Utilisateur = utilisateurDAL.Connexion(email, nouveauMdp);
                    }
                }
                else
                {
                    ViewBag.MdpIncorrect = true;
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
            CommandeDAL commandeDAL = new CommandeDAL();
            List<Commande> commandes = commandeDAL.CommandesUtilisateur(Utilisateur.Id);
            ListeCommandesViewModel listeCommandesViewModel = new ListeCommandesViewModel(commandes);
            return View(listeCommandesViewModel);
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
                using (PanierController panierController = new PanierController()) // TODO plus propre
                {
                    panierController.Utilisateur = Utilisateur;
                    panierController.PanierViewModel = PanierViewModel;

                    List<Article> articlesKo = new List<Article>();
                    foreach (var a in articles)
                    {
                        if (panierController.Ajouter(a.Article, a.Quantite))
                        {
                            PanierViewModel.PrixTotal += a.Quantite * a.Article.Prix;
                            ViewBag.Panier = PanierViewModel;
                        }
                        else
                        {
                            articlesKo.Add(a.Article);
                        }
                    }

                    TempData["ArticlesNonAjoutes"] = articlesKo;
                }
                RecupererPanierEnBase();
                ViewBag.Panier = PanierViewModel;
            }
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
                return RedirectToAction("Profil", "Compte");
            }
            else
            {
                ViewBag.Utilisateur = new Utilisateur();
                ViewBag.MauvaisEmailMdp = true;
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
            return Redirect(Session["Url"] as string);

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
                    ViewBag.MdpIncorrect = true;
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
                ViewBag.MauvaisEmailMdp = true;
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
                TempData["mailEnvoyeNouveauMdpKo"] = "Le lien de redéfinition du mot de passe n'est plus valide. Refaite une demande";
                return RedirectToAction("Connexion", "Compte");
            }
        }

        [HttpPost]
        public ActionResult OubliMotDePasse(string action, string email, string mdp, string mdp2)
        {
            if (action == "generationMail")
            {
                string codeVerification = Guid.NewGuid().ToString("n") + email.GetHash();
                string url = HttpContext.Request.Url.ToString() + '/' + codeVerification;
                Utilisateur utilisateur = new UtilisateurDAL().Details(email);
                if (utilisateur != null)
                {
                    new OubliMotDePasseDAL().Ajouter(utilisateur.Id, codeVerification, DateTime.Now.AddMinutes(10));

                    string sujetMail = "Procédure de génération d'un nouveau mot de passe";
                    string message = "Bonjour\n" +
                        "Vous avez oublié votre mot de passe et avez demandé à en redéfinir un nouveau.\n" +
                        "Si vous êtes bien à l'origine de cette demande, veuillez cliquer sur le lien suivant ou recopier l'adresse dans votre navigateur :\n" +
                        "\n" +
                        url +
                        "\n\nVous serez redirigé vers une page de rédéfinition de votre mot de passe";

                    if (Utilitaire.EnvoieMail(email, sujetMail, message))
                        TempData["mailEnvoyeLienGenerationMdpOk"] = "Un email avec un lien de redéfinition de votre mot de passe vient de vous être envoyé";
                    else
                        TempData["mailEnvoyeLienGenerationMdpKo"] = "Erreur dans l'envoi du mail, veuillez rééssayer dans quelques instants";
                }
                else
                {
                    TempData["mailEnvoyeLienGenerationMdpKo"] = "Nous n'avons pas de compte client avec cette adresse email. Merci de vérifier votre saisie";
                }
                return RedirectToAction("Connexion", "Compte");
            }
            else if (action == "changementMotDePasse")
            {
                if (VerifMdp(mdp, mdp2))
                {
                    UtilisateurDAL utilisateurDAL = new UtilisateurDAL();
                    if (utilisateurDAL.Modification(Utilisateur.Id, mdp) == 1)
                    {
                        ViewBag.Modification = true;
                    }
                }
                else
                {
                    ViewBag.MdpIncorrect = true;
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
