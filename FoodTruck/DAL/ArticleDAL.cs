using System;
using System.Collections.Generic;
using System.Linq;
using FoodTruck.Models;

namespace FoodTruck.DAL
{
    public class ArticleDAL
    {
        internal Article Details(int id)
        {
            Article larticle;
            using (foodtruckEntities db = new foodtruckEntities())
            {
                larticle = (from article in db.Article
                            where article.Id == id
                            select article).FirstOrDefault();
            }
            return larticle;
        }
        internal Article Details(string nom)
        {
            Article larticle;
            using (foodtruckEntities db = new foodtruckEntities())
            {
                larticle = (from article in db.Article
                            where article.Nom == nom
                            select article).FirstOrDefault();
            }
            return larticle;
        }

        public List<FamilleArticle> FamillesArticle()
        {
            List<FamilleArticle> famillesArticle;
            using (foodtruckEntities db = new foodtruckEntities())
            {
                famillesArticle = (from fa in db.FamilleArticle
                                   orderby fa.Id
                                   select fa).ToList();
            }
            return famillesArticle;
        }

        internal void AugmenterQuantiteVendue(int id, int nbre)
        {
            using (foodtruckEntities db = new foodtruckEntities())
            {
                Article larticle = (from article in db.Article
                                    where article.Id == id
                                    select article).FirstOrDefault();
                larticle.NombreVendus += nbre;
                db.SaveChanges();
            }
        }
        internal void AjouterArticleEnBase(Article lArticle)
        {
            using (foodtruckEntities db = new foodtruckEntities())
            {
                db.Article.Add(lArticle);

                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    string messageErreur = DALExceptions.HandleException(ex);
                    throw new Exception(messageErreur);
                }
            }
        }
        internal void ReferencerArticle(int id)
        {
            using (foodtruckEntities db = new foodtruckEntities())
            {
                Article larticle = (from article in db.Article
                                    where article.Id == id
                                    select article).FirstOrDefault();
                larticle.DansCarte = true;
                db.SaveChanges();
            }
        }
        internal void DereferencerArticle(int id)
        {
            using (foodtruckEntities db = new foodtruckEntities())
            {
                Article larticle = (from article in db.Article
                                    where article.Id == id
                                    select article).FirstOrDefault();
                larticle.DansCarte = false;
                db.SaveChanges();
            }
        }

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

        public List<Article> Lister(string nomFamille, int nombreMax = 200)
        {
            using (foodtruckEntities db = new foodtruckEntities())
            {
                List<Article> articles = (from article in db.Article
                                          join famille in db.FamilleArticle on article.FamilleId equals famille.Id
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
