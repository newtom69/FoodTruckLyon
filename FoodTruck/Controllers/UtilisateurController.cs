using FoodTruck.DAL;
using FoodTruck.Models;
using FoodTruck.ViewModels;
using System.Web.Mvc;

namespace FoodTruck.Controllers
{
    public class UtilisateurController : Controller
    {
        [HttpGet]
        public ActionResult Connexion()
        {
            ViewBag.PanierAbsent = false;
            PanierViewModel lePanier;
            if (Session["Panier"] == null) lePanier = new PanierViewModel();
            else lePanier = (PanierViewModel)Session["Panier"];
            Session["Panier"] = lePanier;
            ViewBag.Panier = lePanier;

            if (Session["Utilisateur"] == null)
                return View();
            else
                return RedirectToAction("../");
        }

        [HttpPost]
        public ActionResult Connexion(string Email, string Mdp)
        {
            ViewBag.PanierAbsent = false;
            PanierViewModel lePanier;
            if (Session["Panier"] == null) lePanier = new PanierViewModel();
            else lePanier = (PanierViewModel)Session["Panier"];
            Session["Panier"] = lePanier;
            ViewBag.Panier = lePanier;

            Utilisateur lUtilisateur;
            UtilisateurDAL lUtilisateurDAL;
            if (Session["Utilisateur"] == null)
            {
                lUtilisateurDAL = new UtilisateurDAL();
                lUtilisateur = lUtilisateurDAL.Connexion(Email, Mdp);
            }
            else
            {
                lUtilisateur = (Utilisateur)Session["Utilisateur"];
            }
            Session["Utilisateur"] = lUtilisateur;
            ViewBag.lUtilisateur = lUtilisateur;

            if (SynchroniserPanier(lUtilisateur)) 
            {
                return RedirectToAction("../");
            }

            Session["Utilisateur"] = null;
            ViewBag.lUtilisateur = null;
            ViewBag.MauvaisEmailMdp = true;

            VisiteDAL.Enregistrer(lUtilisateur != null ? lUtilisateur.Id : 0);
            return View();
        }

        [HttpGet]
        public ActionResult Deconnexion()
        {
            ViewBag.PanierAbsent = true;
            Session["Utilisateur"] = null;
            Session["Panier"] = null;
            return View();
        }

        [HttpGet]
        public ActionResult Creation()
        {
            ViewBag.PanierAbsent = false;
            PanierViewModel lePanier;
            if (Session["Panier"] == null) lePanier = new PanierViewModel();
            else lePanier = (PanierViewModel)Session["Panier"];
            Session["Panier"] = lePanier;
            ViewBag.Panier = lePanier;

            return View();
        }

        [HttpPost]
        public ActionResult Creation(string Email, string Mdp, string Mdp2, string Nom, string Prenom, string Telephone)
        {
            ViewBag.PanierAbsent = false;
            PanierViewModel lePanier;
            if (Session["Panier"] == null) lePanier = new PanierViewModel();
            else lePanier = (PanierViewModel)Session["Panier"];
            Session["Panier"] = lePanier;
            ViewBag.Panier = lePanier;

            Utilisateur lUtilisateur;
            UtilisateurDAL lUtilisateurDAL;
            if (Session["Utilisateur"] == null)
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
                lUtilisateur = (Utilisateur)Session["Utilisateur"];
            }
            Session["Utilisateur"] = lUtilisateur;
            ViewBag.lUtilisateur = lUtilisateur;
            VisiteDAL.Enregistrer(lUtilisateur != null ? lUtilisateur.Id : 0);
            if (lUtilisateur != null)
            {
                return RedirectToAction($"./Detail/{lUtilisateur.Id}");
            }
            else
            {
                Session["Utilisateur"] = null;
                ViewBag.lUtilisateur = null;
                ViewBag.MauvaisEmailMdp = true;
                return View();
            }
        }

        bool VerifMdp(string mdp1, string mdp2)
        {
            bool valeurRetour = true;
            if (mdp1 != mdp2) valeurRetour = false;
            if (mdp1.Length<8) valeurRetour = false;
            return valeurRetour;
        }

        bool SynchroniserPanier(Utilisateur lUtilisateur)
        {
            if (lUtilisateur != null && lUtilisateur.Id != 0)
            {
                PanierDAL lePanierDal = new PanierDAL(lUtilisateur.Id);

                PanierViewModel panierUI;
                if (Session["Panier"] == null)
                    panierUI = new PanierViewModel();
                else
                    panierUI = (PanierViewModel)Session["Panier"];

                foreach(ArticleDetailsViewModel lArticle in panierUI.Articles)
                {
                    Panier panier = lePanierDal.ListerPanierUtilisateur().Find(pan => pan.ArticleId == lArticle.Article.Id);
                    if(panier == null)
                        lePanierDal.Ajouter(lArticle.Article);
                    else
                        lePanierDal.ModifierQuantite(lArticle.Article, 1);
                }

                panierUI = new PanierViewModel();
                foreach (Panier lePanier in lePanierDal.ListerPanierUtilisateur())
                {
                    panierUI.PrixTotal += lePanier.PrixTotal;
                    ArticleDetailsViewModel article = panierUI.Articles.Find(art => art.Article.Id == lePanier.ArticleId);
                    if (article != null)
                    {
                        article.Quantite++;
                    }
                    else
                    {
                        ArticleDAL articleDAL = new ArticleDAL();
                        panierUI.Articles.Add(new ArticleDetailsViewModel(articleDAL.Details(lePanier.ArticleId), lePanier.Quantite));
                    }
                }

                Session["Panier"] = panierUI;
                return true;
            }
            return false;
        }
     }
}
