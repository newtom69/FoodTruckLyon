using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FoodTruck.Models;

namespace FoodTruck.DAL
{
    class ArticlesDAL
    {
        public List<Article> ListerRandom(int nombreRetour, int nombreTop)
        {
            using (foodtruckEntities db = new foodtruckEntities())
            {
                List<Article> articles = (from article in db.Article
                                          where article.DansCarte == true && article.FamilleId <= 3
                                          orderby article.NombreVendus descending
                                          select article)
                                          .Take(nombreTop)
                                          .OrderBy(random => Guid.NewGuid())
                                          .Take(nombreRetour)
                                          .ToList();
                return articles;
            }
        }
        public List<Article> Lister(int nombreMax, int familleId)
        {
            using (foodtruckEntities db = new foodtruckEntities())
            {
                if (nombreMax == 0) nombreMax = 200;
                List<Article> articles = (from article in db.Article
                                          where article.DansCarte == true && article.FamilleId == familleId
                                          orderby article.Nom
                                          select article)
                                          .Take(nombreMax)
                                          .ToList();
                return articles;
            }
        }
    }
}
