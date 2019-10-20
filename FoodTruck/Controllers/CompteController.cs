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
            if (session.Utilisateur.Id == 0)
            {
                return RedirectToAction("Connexion", "Compte");
            }
            else
            {
                return RedirectToAction("Profil");
            }
        }

        [HttpGet]
        public ActionResult Profil()
        {
            return View(session.Utilisateur);
        }

        [HttpGet]
        public ActionResult Commandes()
        {
            CommandeDAL commandeDAL = new CommandeDAL();
            List<Commande> commandes = commandeDAL.ListerCommandesUtilisateur(session.Utilisateur.Id);
            AdministrationViewModel administrationViewModel = new AdministrationViewModel(commandes);
            return View(administrationViewModel);
        }

        [HttpPost]
        public ActionResult ReprendreArticlesCommande(int commandeId, string viderPanier)
        {
            CommandeDAL commandeDAL = new CommandeDAL();
            Commande commande = commandeDAL.Detail(commandeId);
            if (commande != null && commande.UtilisateurId == session.Utilisateur.Id)
            {
                List<ArticleViewModel> articles = commandeDAL.ListerArticles(commandeId);
                PanierDAL panierDAL = new PanierDAL(session.Utilisateur.Id);
                if (viderPanier == "Oui")
                {
                    panierDAL.Supprimer();
                    session.RecupererPanierEnBase();
                }
                PanierController panierController = new PanierController();
                foreach (var a in articles)
                {
                    panierController.Ajouter(a.Article);
                    session.PanierViewModel.PrixTotal += a.Article.Prix;
                    Session["Panier"] = session.PanierViewModel;
                }
                session.RecupererPanierEnBase();
            }
            return RedirectToAction("Commandes", "Compte");
        }

        [HttpGet]
        public ActionResult Connexion()
        {
            if (session.Utilisateur.Id == 0)
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
            Session["Utilisateur"] = lUtilisateur;
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
                PanierProspectDAL panierProspectDAL = new PanierProspectDAL(session.ProspectGuid);
                panierProspectDAL.Supprimer();
                cookie = new HttpCookie("Prospect")
                {
                    Expires = DateTime.Now.AddDays(-30)
                };
                Response.Cookies.Add(cookie);

                bool panierPresentEnBase = new PanierDAL(lUtilisateur.Id).ListerPanierUtilisateur().Count > 0 ? true : false;
                if (panierPresentEnBase)
                {
                    TempData["DemandeRestaurationPanier"] = true;
                    return RedirectToAction("RestaurerPanier", "Compte");
                }
                else
                {
                    return Redirect(Session["Url"].ToString());
                }
            }
            else
            {
                Session["Utilisateur"] = null;
                ViewBag.MauvaisEmailMdp = true;
                return View();
            }
        }

        [HttpGet]
        public ActionResult RestaurerPanier()
        {
            return View(session.Utilisateur);
        }

        [HttpPost]
        public ActionResult RestaurerPanier(string reponse)
        {
            if (reponse == "Oui")
            {
                //recupération du panier en base et agrégation avec celui de la session
                session.AgregerPanierEnBase();
                session.RecupererPanierEnBase();
            }
            else
            {
                //effacement panier en base puis enregistrement de celui de la session
                PanierDAL panierDAL = new PanierDAL(session.Utilisateur.Id);
                panierDAL.Supprimer();
                session.AgregerPanierEnBase();
            }
            return Redirect(Session["Url"].ToString());
        }

        [HttpGet]
        public ActionResult Deconnexion()
        {
            if (session.Utilisateur.Id != 0)
            {
                HttpCookie newCookie = new HttpCookie("GuidClient")
                {
                    Expires = DateTime.Now.AddDays(-30)
                };
                Response.Cookies.Add(newCookie);
                new SessionVariables(0);
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
            if (session.Utilisateur.Id == 0)
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
                lUtilisateur = session.Utilisateur;
            }
            Session["Utilisateur"] = lUtilisateur;
            if (lUtilisateur != null)
            {
                return RedirectToAction($"./Profil");
            }
            else
            {
                Session["Utilisateur"] = null;
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
