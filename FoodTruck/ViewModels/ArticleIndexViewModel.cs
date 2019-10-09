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
            ArticlesDAL articlesDAL = new ArticlesDAL();

            ArticlesEntree = new List<ArticleDetailsViewModel>();
            ArticlesPlat = new List<ArticleDetailsViewModel>();
            ArticlesDessert = new List<ArticleDetailsViewModel>();
            ArticlesBoissonFraiche = new List<ArticleDetailsViewModel>();
            ArticlesBoissonChaude = new List<ArticleDetailsViewModel>();

            foreach (Article article in articlesDAL.Lister("Entrée"))
            {
                ArticlesEntree.Add(new ArticleDetailsViewModel(article));
            }

            foreach (Article article in articlesDAL.Lister("Plat"))
            {
                ArticlesPlat.Add(new ArticleDetailsViewModel(article));
            }

            foreach (Article article in articlesDAL.Lister("Dessert"))
            {
                ArticlesDessert.Add(new ArticleDetailsViewModel(article));
            }

            foreach (Article article in articlesDAL.Lister("Boisson Fraiche"))
            {
                ArticlesBoissonFraiche.Add(new ArticleDetailsViewModel(article));
            }

            foreach (Article article in articlesDAL.Lister("Boisson Chaude"))
            {
                ArticlesBoissonChaude.Add(new ArticleDetailsViewModel(article));
            }
        }
    }
}