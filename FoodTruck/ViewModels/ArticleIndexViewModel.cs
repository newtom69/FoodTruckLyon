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
        public List<ArticleDetailsViewModel> ArticlesEntree { get; set; }
        public List<ArticleDetailsViewModel> ArticlesPlat { get; set; }
        public List<ArticleDetailsViewModel> ArticlesDessert { get; set; }
        public List<ArticleDetailsViewModel> ArticlesBoissonFraiche { get; set; }
        public List<ArticleDetailsViewModel> ArticlesBoissonChaude { get; set; }

        public ArticleIndexViewModel()
        {
            ArticleDAL articleDAL = new ArticleDAL();

            ArticlesEntree = new List<ArticleDetailsViewModel>();
            ArticlesPlat = new List<ArticleDetailsViewModel>();
            ArticlesDessert = new List<ArticleDetailsViewModel>();
            ArticlesBoissonFraiche = new List<ArticleDetailsViewModel>();
            ArticlesBoissonChaude = new List<ArticleDetailsViewModel>();

            foreach (Article article in articleDAL.Lister("Entrée"))
            {
                ArticlesEntree.Add(new ArticleDetailsViewModel(article));
            }

            foreach (Article article in articleDAL.Lister("Plat"))
            {
                ArticlesPlat.Add(new ArticleDetailsViewModel(article));
            }

            foreach (Article article in articleDAL.Lister("Dessert"))
            {
                ArticlesDessert.Add(new ArticleDetailsViewModel(article));
            }

            foreach (Article article in articleDAL.Lister("Boisson Fraiche"))
            {
                ArticlesBoissonFraiche.Add(new ArticleDetailsViewModel(article));
            }

            foreach (Article article in articleDAL.Lister("Boisson Chaude"))
            {
                ArticlesBoissonChaude.Add(new ArticleDetailsViewModel(article));
            }
        }
    }
}