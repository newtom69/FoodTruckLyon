﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoodTruck.ViewModels
{
    public class PanierViewModel
    {
        public double PrixTotal { get; set; }
        public List<ArticleDetailsViewModel> ArticlesDetailsViewModel { get; set; }

        public PanierViewModel()
        {
            ArticlesDetailsViewModel = new List<ArticleDetailsViewModel>();
        }

        public void Trier()
        {
            ArticlesDetailsViewModel=ArticlesDetailsViewModel.OrderBy(x => x.Article.FamilleId).ThenBy(x => x.Article.Nom).ToList();
        }
    }
}