using FoodTruck.Models;
using System.Web.Mvc;
using FoodTruck.DAL;
using System;
using System.Web;
using System.IO;

namespace FoodTruck.Controllers
{
    public class ArticleController : Controller
    {
        // GET: Article
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.PanierAbsent = false;
            PanierUI lePanier;
            if (Session["Panier"] == null) lePanier = new PanierUI();
            else lePanier = (PanierUI)Session["Panier"];
            Session["Panier"] = lePanier;
            ViewBag.Panier = lePanier;

            Utilisateur lUtilisateur = null;
            if (Session["Utilisateur"] != null)
            {
                lUtilisateur = (Utilisateur)Session["Utilisateur"];
                ViewBag.lUtilisateur = lUtilisateur;
            }

            ArticlesDAL articlesDAL = new ArticlesDAL();
            ViewBag.articlesEntree = articlesDAL.Lister(0, 1);
            ViewBag.articlesPlat = articlesDAL.Lister(0, 2);
            ViewBag.articlesDessert = articlesDAL.Lister(0, 3);
            ViewBag.articlesBoissonFraiche = articlesDAL.Lister(0, 4);
            ViewBag.articlesBoissonChaude = articlesDAL.Lister(0, 5);

            VisiteDAL.Enregistrer(lUtilisateur != null ? lUtilisateur.Id : 0);
            return View();
        }

        [HttpGet]
        public ActionResult Details(string nom)
        {
            ViewBag.PanierAbsent = false;
            PanierUI lePanier;
            if (Session["Panier"] == null) lePanier = new PanierUI();
            else lePanier = (PanierUI)Session["Panier"];
            Session["Panier"] = lePanier;
            ViewBag.Panier = lePanier;
            Utilisateur lUtilisateur;
            if (Session["Utilisateur"] != null)
            {
                lUtilisateur = (Utilisateur)Session["Utilisateur"];
                ViewBag.lUtilisateur = lUtilisateur;
            }
            else lUtilisateur = new Utilisateur();
            ArticleDAL lArticleDAL = new ArticleDAL();
            Article articleCourant;
            articleCourant = lArticleDAL.Details(nom);
            if(articleCourant==null)
            {
                TempData["ArticleOk"] = false;
            }
            else if(!articleCourant.DansCarte)
            {
                TempData["ArticleOk"] = true;
                TempData["ArticleDansCarte"] = false;
            }
            else
            {
                TempData["ArticleDansCarte"] = true;
                TempData["ArticleOk"] = true;
            }
            VisiteDAL.Enregistrer(lUtilisateur != null ? lUtilisateur.Id : 0);
            return View(articleCourant);
        }
        [HttpGet]
        public ActionResult AjouterEnBase()
        {
            bool droitPage = VerifierDroit();
            TempData["DroitPage"] = droitPage;
            return View();
        }

        [HttpPost]
        public ActionResult AjouterEnBase(Article lArticle)
        {
            bool droitPage = VerifierDroit();
            if (droitPage)
            {
                ArticleDAL articleDAL = new ArticleDAL();
                try
                {
                    articleDAL.AjouterArticleEnBase(lArticle);

                    if(Request.Files.Count != 1 || Request.Files[0].ContentLength == 0 || Request.Files[0].ContentLength > 1024 * 1024 * 2)
                    {
                        ModelState.AddModelError("uploadError", "File's length is zero, or no files found");
                    }
                    else
                    {
                        // extract only the filename
                        string fileName = Path.GetFileName(Request.Files[0].FileName);
                        lArticle.Image += GetHashCode();
                        string path = Path.Combine(Server.MapPath("~/Content/Images"), lArticle.Image);
                        Request.Files[0].SaveAs(path);
                    }
                }
                catch (Exception ex)
                {
                    TempData["Erreur"] = ex.Message;
                }

                //TODO
            }
            Utilisateur lUtilisateur;
            if (Session["Utilisateur"] != null)
            {
                lUtilisateur = (Utilisateur)Session["Utilisateur"];
                ViewBag.lUtilisateur = lUtilisateur;
            }
            else
            {
                lUtilisateur = new Utilisateur();
            }
            VisiteDAL.Enregistrer(lUtilisateur != null ? lUtilisateur.Id : 0);
            TempData["DroitPage"] = droitPage;
            return View();
        }

        private bool VerifierDroit()
        {
            bool droitPage;
            Utilisateur lUtilisateur;
            if (Session["Utilisateur"] != null)
            {
                lUtilisateur = (Utilisateur)Session["Utilisateur"];
                ViewBag.lUtilisateur = lUtilisateur;
                if (lUtilisateur.AdminArticle || lUtilisateur.AdminTotal)
                    droitPage = true;
                else
                    droitPage = false;
            }
            else
            {
                droitPage = false;
            }

#if DEBUG
            return true;
#endif
            return droitPage;
        }
    }
}
