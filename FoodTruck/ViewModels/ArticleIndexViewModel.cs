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

        public ArticleIndexViewModel()
        {
            ArticleDAL articleDAL = new ArticleDAL();

            ArticlesEntree = new List<ArticleViewModel>();
            ArticlesPlat = new List<ArticleViewModel>();
            ArticlesDessert = new List<ArticleViewModel>();
            ArticlesBoissonFraiche = new List<ArticleViewModel>();
            ArticlesBoissonChaude = new List<ArticleViewModel>();
            foreach (Article article in articleDAL.Lister("Entrée"))
            {
                ArticlesEntree.Add(new ArticleViewModel(article));
            }
            foreach (Article article in articleDAL.Lister("Plat"))
            {
                ArticlesPlat.Add(new ArticleViewModel(article));
            }
            foreach (Article article in articleDAL.Lister("Dessert"))
            {
                ArticlesDessert.Add(new ArticleViewModel(article));
            }
            foreach (Article article in articleDAL.Lister("Boisson Fraiche"))
            {
                ArticlesBoissonFraiche.Add(new ArticleViewModel(article));
            }
            foreach (Article article in articleDAL.Lister("Boisson Chaude"))
            {
                ArticlesBoissonChaude.Add(new ArticleViewModel(article));
            }
        }
    }
}