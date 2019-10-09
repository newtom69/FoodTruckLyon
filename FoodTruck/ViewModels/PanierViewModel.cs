using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoodTruck.ViewModels
{
    public class PanierViewModel
    {
        public double PrixTotal { get; set; }
        public List<ArticleDetailsViewModel> Articles { get; set; }

        public PanierViewModel()
        {
            Articles = new List<ArticleDetailsViewModel>();
        }
    }
}