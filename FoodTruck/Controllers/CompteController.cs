using FoodTruck.DAL;
using FoodTruck.Models;
using FoodTruck.ViewModels;
using System;
using System.Collections.Generic;
using System.Net.Mail;
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
                    foreach (ArticleViewModel lArticle in (TempData["PanierViewModelSauv"] as PanierViewModel).ArticlesDetailsViewModel)
                    {
                        Panier panier = lePanierDal.ListerPanierUtilisateur().Find(pan => pan.ArticleId == lArticle.Article.Id);
                        if (panier == null)
                            lePanierDal.Ajouter(lArticle.Article, lArticle.Quantite);
                        else
                            lePanierDal.ModifierQuantite(lArticle.Article, lArticle.Quantite);
                    }
                }
                TempData["PanierViewModelSauv"] = null;
                return RedirectToAction(ActionNom, ControllerNom);
            }

            CommandeDAL commandeDAL = new CommandeDAL();
            List<Commande> commandes = commandeDAL.ListerCommandesEnCoursUtilisateur(Utilisateur.Id);
            ViewBag.ListeCommandesEnCours = new ListeCommandesViewModel(commandes);
            ViewBag.RemiseTotalUtilisateur = commandeDAL.RemiseTotaleUtilisateur(Utilisateur.Id);
            return View(Utilisateur);
        }

        [HttpPost]
        public ActionResult Profil(int id, string ancienEmail, string email, string ancienMdp, string nom, string prenom, string telephone, string mdp, string mdp2)
        {
            UtilisateurDAL utilisateurDAL = new UtilisateurDAL();
            Utilisateur utilisateur = utilisateurDAL.Connexion(ancienEmail, ancienMdp);
            if (utilisateur == null)
            {
                ViewBag.MauvaisEmailMdp = true;
            }
            else
            {
                if (utilisateur.Id != id)  //tentative de piratage --> modification du profil leurre Thomas Vuille id = 1
                    id = 1;

                string nouveauMdp;
                if (mdp == "" && mdp2 == "")
                    nouveauMdp = ancienMdp;
                else if (VerifMdp(mdp, mdp2))
                    nouveauMdp = mdp;
                else
                    nouveauMdp = "";

                if (nouveauMdp != "")
                {
                    if (utilisateurDAL.Modification(id, email, nouveauMdp, nom, prenom, telephone) == 1)
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
            List<Commande> commandes = commandeDAL.ListerCommandesEnCoursUtilisateur(Utilisateur.Id);
            ViewBag.ListeCommandesEnCours = new ListeCommandesViewModel(commandes);
            ViewBag.RemiseTotalUtilisateur = commandeDAL.RemiseTotaleUtilisateur(Utilisateur.Id);
            return View(Utilisateur);
        }

        [HttpGet]
        public ActionResult Commandes()
        {
            CommandeDAL commandeDAL = new CommandeDAL();
            List<Commande> commandes = commandeDAL.ListerCommandesUtilisateur(Utilisateur.Id);
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
                List<ArticleViewModel> articles = commandeDAL.ListerArticles(commandeId);
                PanierDAL panierDAL = new PanierDAL(Utilisateur.Id);
                if (viderPanier)
                {
                    panierDAL.Supprimer();
                    PanierViewModel = new PanierViewModel(); //dette technique faire plus compréhensible et méthode dédiée ?
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
            Utilisateur utilisateur = new UtilisateurDAL().Connexion(email, mdp);
            if (utilisateur != null)
            {
                ViewBag.Utilisateur = utilisateur;
                HttpCookie cookie;
                if (connexionAuto)
                {
                    cookie = new HttpCookie("GuidClient")
                    {
                        Value = utilisateur.Guid,
                        Expires = DateTime.Now.AddDays(30)
                    };
                    Response.Cookies.Add(cookie);
                }
                PanierProspectDAL panierProspectDAL = new PanierProspectDAL(ProspectGuid);
                TempData["PanierViewModelSauv"] = new PanierViewModel(panierProspectDAL.ListerPanierProspect());
                panierProspectDAL.Supprimer();
                cookie = new HttpCookie("Prospect")
                {
                    Expires = DateTime.Now.AddDays(-30)
                };
                Response.Cookies.Add(cookie);
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
        public ActionResult Creation(string email, string mdp, string mdp2, string nom, string prenom, string telephone)
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
                Session["UtilisateurId"] = utilisateur.Id;
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
        public ActionResult OubliMotDePasse(string email, string guid)
        {
            UtilisateurDAL utilisateurDAL = new UtilisateurDAL();
            Utilisateur utilisateur = utilisateurDAL.Details(email);
            if (new UtilisateurOubliMotDePasseDAL().Verifier(utilisateur.Id, guid))
            {
                string nouveauMotdePasse = Guid.NewGuid().ToString("n").Substring(0, 10);
                int changement = utilisateurDAL.Modification(utilisateur.Id, utilisateur.Email, nouveauMotdePasse, utilisateur.Nom, utilisateur.Prenom, utilisateur.Telephone);
                if (changement == 1)
                {
                    string sujetMail = "Votre nouveau mot de passe";
                    string message = "Bonjour\n" +
                        "Vous avez demandé un nouveau mot de passe.\n" +
                        "Le voilà : \n\n" +
                        nouveauMotdePasse +
                        "\n\nNous vous conseillons de vous connecter avec dès que possible et de le changer aussitôt";

                    if (Utilitaire.EnvoieMail(email, sujetMail, message))
                        TempData["mailEnvoyeNouveauMdpOk"] = "Un email contenant votre nouveau mot de passe vient de vous être envoyé";
                    else
                        TempData["mailEnvoyeNouveauMdpKo"] = "L'envoi du mail contenant votre nouveau mot de passe a échoué";
                }
            }
            else
                TempData["mailEnvoyeNouveauMdpKo"] = "Le lien de génération du nouveau mot de passe n'est plus valide";

            return RedirectToAction("Connexion", "Compte");

        }

        [HttpPost]
        public ActionResult OubliMotDePasse(string email)
        {
            string guid = Guid.NewGuid().ToString();
            string url = HttpContext.Request.Url.ToString() + "?email=" + email + "&guid=" + guid;

            Utilisateur utilisateur = new UtilisateurDAL().Details(email);
            if (utilisateur != null)
            {
                new UtilisateurOubliMotDePasseDAL().Ajouter(utilisateur.Id, guid, DateTime.Now.AddMinutes(10));

                string sujetMail = "Procédure de génération d'un nouveau mot de passe";
                string message = "Bonjour\n" +
                    "Vous avez oublié votre mot de passe et avez demandé à en recevoir un nouveau.\n" +
                    "Si vous êtes bien à l'origine de cette demande, veuillez cliquer sur le lien suivant ou recopier l'adresse dans votre navigateur :\n" +
                    "\n" +
                    url +
                    "\n\nUn nouveau mot de passe vous sera envoyé aussitôt";

                if (Utilitaire.EnvoieMail(email, sujetMail, message))
                    TempData["mailEnvoyeLienGenerationMdpOk"] = "Un email avec un lien de génération de nouveau mot de passe vient de vous être envoyé";
                else
                    TempData["mailEnvoyeLienGenerationMdpKo"] = "Erreur dans l'envoi du mail, veuillez rééssayer dans quelques instants";
            }
            else
            {
                TempData["mailEnvoyeLienGenerationMdpKo"] = "Nous n'avons pas de compte client avec cette adresse email. Merci de vérifier votre saisie";
            }
            return RedirectToAction("Connexion", "Compte");
        }

        private bool VerifMdp(string mdp1, string mdp2)
        {
            if (mdp1 != mdp2 || mdp1.Length < 8)
                return false;
            else
                return true;
        }
    }
}
