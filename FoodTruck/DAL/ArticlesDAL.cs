using System;
using System.Collections.Generic;
using System.Linq;
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

        public List<Article> Lister(string nomFamille, int nombreMax=200)
        {
            using (foodtruckEntities db = new foodtruckEntities())
            {
                List<Article> articles = (from article in db.Article
                                          join famille in db.FamilleArticle on  article.FamilleId equals famille.Id
                                          where article.DansCarte == true && famille.Nom == nomFamille
                                          orderby article.Nom
                                          select article)
                                          .Take(nombreMax)
                                          .ToList();
                return articles;
            }
        }
    }
}
