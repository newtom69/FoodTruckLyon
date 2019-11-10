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
                return View(new ArticleIndexViewModel());
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
            return View(new ArticleViewModel(articleCourant));
        }
    }
}
