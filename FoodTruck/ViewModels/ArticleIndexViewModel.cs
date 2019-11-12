using FoodTruck.DAL;
using System.Collections.Generic;

namespace FoodTruck.ViewModels
{
    public class ArticleIndexViewModel
    {
        public List<ArticleViewModel> Articles { get; set; }

        /// <summary>
        /// retourne tous les articles ou seulement ceux dans la carte selon dansCarteSeulement
        /// </summary>
        public ArticleIndexViewModel(bool dansCarteSeulement)
        {
            Articles = new List<ArticleViewModel>();
            foreach (Article article in new ArticleDAL().Articles(dansCarteSeulement))
            {
                Articles.Add(new ArticleViewModel(article));
            }
        }
    }
}