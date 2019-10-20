using FoodTruck.DAL;
using FoodTruck.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoodTruck.ViewModels
{
    public class ArticleIndexViewModel
    {
        public List<ArticleViewModel> ArticlesEntree { get; set; }
        public List<ArticleViewModel> ArticlesPlat { get; set; }
        public List<ArticleViewModel> ArticlesDessert { get; set; }
        public List<ArticleViewModel> ArticlesBoissonFraiche { get; set; }
        public List<ArticleViewModel> ArticlesBoissonChaude { get; set; }
        public List<ArticleViewModel> Articles { get; set; }

        public ArticleIndexViewModel()
        {
            ArticleDAL articleDAL = new ArticleDAL();

            ArticlesEntree = new List<ArticleViewModel>();
            ArticlesPlat = new List<ArticleViewModel>();
            ArticlesDessert = new List<ArticleViewModel>();
            ArticlesBoissonFraiche = new List<ArticleViewModel>();
            ArticlesBoissonChaude = new List<ArticleViewModel>();
            foreach (Article article in articleDAL.ListerArticles("Entrée", true))
            {
                ArticlesEntree.Add(new ArticleViewModel(article));
            }
            foreach (Article article in articleDAL.ListerArticles("Plat", true))
            {
                ArticlesPlat.Add(new ArticleViewModel(article));
            }
            foreach (Article article in articleDAL.ListerArticles("Dessert", true))
            {
                ArticlesDessert.Add(new ArticleViewModel(article));
            }
            foreach (Article article in articleDAL.ListerArticles("Boisson Fraiche", true))
            {
                ArticlesBoissonFraiche.Add(new ArticleViewModel(article));
            }
            foreach (Article article in articleDAL.ListerArticles("Boisson Chaude", true))
            {
                ArticlesBoissonChaude.Add(new ArticleViewModel(article));
            }


            Articles = new List<ArticleViewModel>();
            foreach (Article article in articleDAL.ListerArticles(true))
            {
                Articles.Add(new ArticleViewModel(article));
            }
        }
    }
}