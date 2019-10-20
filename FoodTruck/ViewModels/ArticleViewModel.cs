using FoodTruck.Extensions;
using FoodTruck.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoodTruck.ViewModels
{
    public class ArticleViewModel
    {
        public int Quantite { get; set; }
        public double PrixTotal { get; set; }
        public string NomPourUrl { get; set; }
        public Article Article { get; set; }

        public ArticleViewModel(Article article, int quantite = 1)
        {
            if (article != null)
            {
                Quantite = quantite;
                PrixTotal = Math.Round(quantite * article.Prix, 2);
                Article = article;
                NomPourUrl = Article.Nom.ToUrl();
            }
        }
    }
}