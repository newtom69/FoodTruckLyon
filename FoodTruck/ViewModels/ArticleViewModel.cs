using FoodTruck.Models;
using FoodTruck.Outils;
using System;

namespace FoodTruck.ViewModels
{
    public class ArticleViewModel
    {
        public int Quantite { get; set; }
        public double PrixTotalTTC { get; set; }
        public string NomPourUrl { get; set; }
        public Article Article { get; set; }

        public ArticleViewModel(Article article, int quantite = 1)
        {
            if (article != null)
            {
                Quantite = quantite;
                PrixTotalTTC = Math.Round(quantite * article.Prix, 2);
                Article = article;
                NomPourUrl = Article.Nom.ToUrl();
            }
        }
    }
}