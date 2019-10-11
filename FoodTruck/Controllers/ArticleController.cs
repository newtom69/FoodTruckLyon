using FoodTruck.Models;
using System.Web.Mvc;
using FoodTruck.DAL;
using System;
using System.Web;
using System.IO;
using FoodTruck.Extensions;
using FoodTruck.ViewModels;

namespace FoodTruck.Controllers
{
    public class ArticleController : Controller
    {
        // GET: Article
        [HttpGet]
        public ActionResult Index()
        {
            SessionVariables session = new SessionVariables();
            ViewBag.Panier = session.PanierViewModel;
            ViewBag.Utilisateur = session.Utilisateur;

            VisiteDAL.Enregistrer(session.Utilisateur.Id);
            return View(new ArticleIndexViewModel());
        }

        [HttpGet]
        public ActionResult Details(string nom)
        {
            nom = nom.UrlVersNom();

            SessionVariables session = new SessionVariables();
            ViewBag.Panier = session.PanierViewModel;
            ViewBag.Utilisateur = session.Utilisateur;

            ArticleDAL lArticleDAL = new ArticleDAL();
            Article articleCourant;
            articleCourant = lArticleDAL.Details(nom);
            if (articleCourant == null)
            {
                TempData["ArticleOk"] = false;
            }
            else if (!articleCourant.DansCarte)
            {
                TempData["ArticleOk"] = true;
                TempData["ArticleDansCarte"] = false;
            }
            else
            {
                TempData["ArticleDansCarte"] = true;
                TempData["ArticleOk"] = true;
            }

            VisiteDAL.Enregistrer(session.Utilisateur.Id);
            return View(new ArticleDetailsViewModel(articleCourant));
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
            TempData["DroitPage"] = droitPage;
            if (droitPage)
            {
                ArticleDAL articleDAL = new ArticleDAL();
                try
                {
                    articleDAL.AjouterArticleEnBase(lArticle);

                    if (Request.Files.Count != 1 || Request.Files[0].ContentLength == 0 || Request.Files[0].ContentLength > 1024 * 1024 * 2)
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

            SessionVariables session = new SessionVariables();
            ViewBag.Panier = session.PanierViewModel;
            ViewBag.Utilisateur = session.Utilisateur;

            VisiteDAL.Enregistrer(session.Utilisateur.Id);
            return View();
        }

        private bool VerifierDroit()
        {
            SessionVariables session = new SessionVariables();

            if (session.Utilisateur.AdminArticle || session.Utilisateur.AdminTotal)
                return true;
            else
            #if DEBUG
                return true;
            #endif
                return false;
        }
    }
}
