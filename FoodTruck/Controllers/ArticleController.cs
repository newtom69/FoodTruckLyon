using FoodTruck.DAL;
using FoodTruck.Outils;
using FoodTruck.ViewModels;
using System.Web.Mvc;

namespace FoodTruck.Controllers
{
    public class ArticleController : ControllerParent
    {
        [HttpGet]
        public ActionResult Index()
        {
            if (AdminArticle)
                return View(new ArticleIndexViewModel(false));
            else
                return View(new ArticleIndexViewModel(true));
        }

        [HttpGet]
        public ActionResult Details(string nom)
        {
            nom = nom.UrlVersNom();
            ArticleDAL lArticleDAL = new ArticleDAL();
            Article articleCourant;
            articleCourant = lArticleDAL.Details(nom);
            if (articleCourant == null)
            {
                TempData["message"] = new Message("L'article que vous demandez n'existe pas !", TypeMessage.Erreur);
                return RedirectToAction("Index", "Article");
            }
            else if (!articleCourant.DansCarte)
            {
                TempData["message"] = new Message("L'article choisi n'est plus disponible dans votre foodtruck", TypeMessage.Avertissement);
            }
            return View(new ArticleViewModel(articleCourant));
        }
    }
}
