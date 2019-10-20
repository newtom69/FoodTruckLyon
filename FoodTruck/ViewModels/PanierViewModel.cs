using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoodTruck.ViewModels
{
    public class PanierViewModel
    {
        public List<ArticleViewModel> ArticlesDetailsViewModel { get; set; }
        public double PrixTotal { get; set; }
        public List<DateTime> DatesRetraitPossibles { get; set; }

        public PanierViewModel()
        {
            ArticlesDetailsViewModel = new List<ArticleViewModel>();
        }

        public void Trier()
        {
            ArticlesDetailsViewModel=ArticlesDetailsViewModel.OrderBy(x => x.Article.FamilleId).ThenBy(x => x.Article.Nom).ToList();
        }
    }
}