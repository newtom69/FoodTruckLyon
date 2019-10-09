using FoodTruck.Extensions;
using FoodTruck.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoodTruck.ViewModels
{
    public class ArticleDetailsViewModel
    {
        public int Quantite { get; set; }
        public string NomPourUrl { get; set; }
        public Article Article { get; set; }

        public ArticleDetailsViewModel(Article article)
        {
            Quantite = 1;
            Article = article;
            NomPourUrl = Article.Nom.NomVersUrl();
        }
        public ArticleDetailsViewModel(Article article, int quantite)
        {
            Quantite = quantite;
            Article = article;
            NomPourUrl = Article.Nom.NomVersUrl();
        }
    }
}