using FoodTruck.DAL;
using FoodTruck.Models;
using FoodTruck.ViewModels;
using System;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace FoodTruck.Controllers
{
    public class CompteController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            SessionVariables session = new SessionVariables();
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
            SessionVariables session = new SessionVariables();
            ViewBag.Panier = session.PanierViewModel;
            ViewBag.Utilisateur = session.Utilisateur;
            return View(session.Utilisateur);
        }

        [HttpGet]
        public ActionResult Connexion()
        {
            SessionVariables session = new SessionVariables();
            ViewBag.Panier = session.PanierViewModel;
            ViewBag.Utilisateur = session.Utilisateur; // todo

            if (session.Utilisateur.Id == 0)
                return View();
            else
                return RedirectToAction("Profil");
        }

        [HttpPost]
        public ActionResult Connexion(string Email, string Mdp, bool connexionAuto)
        {
            SessionVariables session = new SessionVariables();
            ViewBag.Panier = session.PanierViewModel;

            Utilisateur lUtilisateur;
            UtilisateurDAL lUtilisateurDAL;
            lUtilisateurDAL = new UtilisateurDAL();
            lUtilisateur = lUtilisateurDAL.Connexion(Email, Mdp);
            Session["Utilisateur"] = lUtilisateur;
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
                    //todo page pécedente l'appel à connexion ?
                    TempData["DemandeRestaurationPanier"] = true;
                    return RedirectToAction("RestaurerPanier", "Compte");
                }
                else
                {
                    //todo page pécedente l'appel à connexion ?
                    //return RedirectToAction("Profil", "Compte");
                    return Redirect(Session["UrlNonCompte"].ToString());
                }
            }
            else
            {
                Session["Utilisateur"] = null;
                ViewBag.Utilisateur = null;
                ViewBag.MauvaisEmailMdp = true;
                VisiteDAL.Enregistrer(0);
                return View();
            }
        }

        [HttpGet]
        public ActionResult RestaurerPanier()
        {
            SessionVariables session = new SessionVariables();
            ViewBag.Panier = session.PanierViewModel;
            ViewBag.Utilisateur = session.Utilisateur;
            return View(session.Utilisateur);
        }

        [HttpPost]
        public ActionResult RestaurerPanier(string reponse)
        {
            SessionVariables session = new SessionVariables();
            ViewBag.Panier = session.PanierViewModel;
            ViewBag.Utilisateur = session.Utilisateur;
            if (reponse == "Oui")
            {
                //recupération du panier en base et agrégation avec celui de la session
                session.AgregerPanierEnBase();
                session.RecupererPanierEnBase();
                ViewBag.Panier = session.PanierViewModel;
                ViewBag.Utilisateur = session.Utilisateur;
            }
            else
            {
                //effacement panier en base puis enregistrement de celui de la session
                PanierDAL panierDAL = new PanierDAL(session.Utilisateur.Id);
                panierDAL.Supprimer();
                session.AgregerPanierEnBase();
                ViewBag.Panier = session.PanierViewModel;
                ViewBag.Utilisateur = session.Utilisateur;
            }
            VisiteDAL.Enregistrer(session.Utilisateur.Id);
            return Redirect(Session["UrlNonCompte"].ToString());
        }

        [HttpGet]
        public ActionResult Deconnexion()
        {
            HttpCookie newCookie = new HttpCookie("GuidClient")
            {
                Expires = DateTime.Now.AddDays(-30)
            };
            Response.Cookies.Add(newCookie);

            SessionVariables session = new SessionVariables(0);
            ViewBag.Panier = session.PanierViewModel;
            ViewBag.Utilisateur = session.Utilisateur;
            return Redirect(Session["UrlNonCompte"].ToString());
        }

        [HttpGet]
        public ActionResult Creation()
        {
            SessionVariables session = new SessionVariables();
            ViewBag.Panier = session.PanierViewModel;
            ViewBag.Utilisateur = session.Utilisateur;
            return View();
        }

        [HttpPost]
        public ActionResult Creation(string Email, string Mdp, string Mdp2, string Nom, string Prenom, string Telephone)
        {
            SessionVariables session = new SessionVariables();
            ViewBag.Panier = session.PanierViewModel;
            ViewBag.Utilisateur = session.Utilisateur;

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
            ViewBag.Utilisateur = lUtilisateur;
            VisiteDAL.Enregistrer(lUtilisateur != null ? lUtilisateur.Id : 0);
            if (lUtilisateur != null)
            {
                return RedirectToAction($"./Profil");
            }
            else
            {
                Session["Utilisateur"] = null;
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
