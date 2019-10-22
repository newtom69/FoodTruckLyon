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
    public class ArticleController : ControllerParent
    {
        [HttpGet]
        public ActionResult Index()
        {
            if(AdminArticle || AdminSuper)
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
