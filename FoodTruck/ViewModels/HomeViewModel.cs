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
        public List<ArticleDetailsViewModel> ArticlesTopRandom { get; set; }

        public HomeViewModel()
        {
            ArticlesTopRandom = new List<ArticleDetailsViewModel>();

            ArticleDAL articleDAL = new ArticleDAL();
            foreach (Article article in articleDAL.ListerRandom(3, 7))
            {
                ArticlesTopRandom.Add(new ArticleDetailsViewModel(article));
            }
        }
    }
}