﻿using FoodTruck.DAL;
using System.Collections.Generic;

namespace FoodTruck.ViewModels
{
    public class HomeViewModel
    {
        public List<ArticleViewModel> ArticlesTopRandom { get; set; }

        public HomeViewModel()
        {
            ArticlesTopRandom = new List<ArticleViewModel>();
            ArticleDAL articleDAL = new ArticleDAL();
            foreach (Article article in articleDAL.ListerRandom(3, 7))
            {
                ArticlesTopRandom.Add(new ArticleViewModel(article));
            }
        }
    }
}