using FoodTruck.Models;
using System.Web.Mvc;
using FoodTruck.DAL;
using System;
using System.Web;
using System.IO;
using FoodTruck.Extensions;
using FoodTruck.ViewModels;
using System.Globalization;

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
        public ActionResult AjouterEnBase(string nom, string description, string prix, int grammage, int litrage, string allergenes, int familleId, bool dansCarte, HttpPostedFileBase file)
        {
            bool droitPage = VerifierDroit();
            TempData["DroitPage"] = droitPage;
            if (droitPage)
            {
                //TODO : vérif et formattage entrée utilisateur
                Article lArticle = new Article
                {
                    Nom = nom,
                    Description = description,
                    Prix = Math.Round(float.Parse(prix, CultureInfo.InvariantCulture.NumberFormat), 2),
                    Grammage = grammage,
                    Litrage = litrage,
                    Allergenes = allergenes,
                    FamilleId = familleId,
                    DansCarte = dansCarte,
                };
                ArticleDAL articleDAL = new ArticleDAL();
                try
                {
                    string fileName = nom.NomPourUrl() + Path.GetFileName(file.FileName);
                    lArticle.Image = fileName;
                    var path = Path.Combine(Server.MapPath("/Content/Images"), fileName);
                    //TODO : resize file
                    file.SaveAs(path);

                    lArticle.Image = fileName;
                    articleDAL.AjouterArticleEnBase(lArticle);
                }
                catch (Exception ex)
                {
                    TempData["Erreur"] = ex.Message;
                }
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
