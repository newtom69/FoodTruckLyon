using System.Collections.Generic;

namespace FoodTruck.Models2
{
    public class Panier
    {
        public List<Article> listeArticles { get; set; }
        public double PrixTotal;

        public Panier()
        {
            listeArticles = new List<Article>();
            PrixTotal = 0;
        }
    }
}