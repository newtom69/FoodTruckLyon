using FoodTruck.DAL;
using FoodTruck.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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