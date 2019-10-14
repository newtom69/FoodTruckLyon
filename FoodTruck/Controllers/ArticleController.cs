using FoodTruck.Models;
using System.Web.Mvc;
using FoodTruck.DAL;
using System;
using System.Web;
using System.IO;
using FoodTruck.Extensions;
using FoodTruck.ViewModels;
using System.Globalization;
using System.Drawing;
using System.Configuration;

namespace FoodTruck.Controllers
{
    public class ArticleController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            SessionVariables session = new SessionVariables();
            ViewBag.Panier = session.PanierViewModel;
            ViewBag.Utilisateur = session.Utilisateur;
            TempData["Admin"] = VerifierDroit(session.Utilisateur);
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
            TempData["Admin"] = VerifierDroit(session.Utilisateur);

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
        public bool VerifierDroit(Utilisateur utilisateur)
        {
            if (utilisateur.AdminArticle || utilisateur.AdminTotal)
                return true;
            else
                return false;
        }

    }
}
