using FoodTruck.DAL;
using FoodTruck.Models;
using FoodTruck.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
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
            if(TempData["PanierViewModelSauv"] != null)
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
                return RedirectToAction("Profil", "Compte");
            }
            return View(Utilisateur);
        }

        [HttpGet]
        public ActionResult Commandes()
        {
            CommandeDAL commandeDAL = new CommandeDAL();
            List<Commande> commandes = commandeDAL.ListerCommandesUtilisateur(Utilisateur.Id);
            AdministrationViewModel administrationViewModel = new AdministrationViewModel(commandes);
            return View(administrationViewModel);
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
                PanierController panierController = new PanierController();
                foreach (var a in articles)
                {
                    panierController.Ajouter(a.Article, a.Quantite); // todo ne pas instancier de controller si possible
                    PanierViewModel.PrixTotal += a.Quantite * a.Article.Prix;
                    ViewBag.Panier = PanierViewModel;
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
        public ActionResult Connexion(string Email, string Mdp, bool connexionAuto)
        {
            Utilisateur lUtilisateur;
            UtilisateurDAL lUtilisateurDAL;
            lUtilisateurDAL = new UtilisateurDAL();
            lUtilisateur = lUtilisateurDAL.Connexion(Email, Mdp);
            ViewBag.Utilisateur = lUtilisateur;
            if (lUtilisateur != null)
            {
                HttpCookie cookie;
                if (connexionAuto)
                {
                    cookie = new HttpCookie("GuidClient")
                    {
                        Value = lUtilisateur.Guid,
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
                ViewBag.Utilisateur = null;
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
            return Redirect(Session["Url"].ToString());

        }

        [HttpGet]
        public ActionResult Creation()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Creation(string Email, string Mdp, string Mdp2, string Nom, string Prenom, string Telephone)
        {
            Utilisateur lUtilisateur;
            UtilisateurDAL lUtilisateurDAL;
            if (Utilisateur.Id == 0)
            {
                lUtilisateurDAL = new UtilisateurDAL();
                if (!VerifMdp(Mdp, Mdp2))
                {
                    ViewBag.MdpIncorrect = true;
                    ViewBag.Nom = Nom;
                    ViewBag.Prenom = Prenom;
                    ViewBag.Email = Email;
                    ViewBag.Telephone = Telephone;
                    return View();
                }
                lUtilisateur = lUtilisateurDAL.Creation(Email, Mdp, Nom, Prenom, Telephone);
            }
            else
            {
                lUtilisateur = Utilisateur;
            }
            ViewBag.Utilisateur = lUtilisateur;
            if (lUtilisateur != null)
            {
                return RedirectToAction($"./Profil");
            }
            else
            {
                ViewBag.Utilisateur = null;
                ViewBag.MauvaisEmailMdp = true;
                return View();
            }
        }

        bool VerifMdp(string mdp1, string mdp2)
        {
            bool valeurRetour = true;
            if (mdp1 != mdp2) valeurRetour = false;
            if (mdp1.Length < 8) valeurRetour = false;
            return valeurRetour;
        }
    }
}
