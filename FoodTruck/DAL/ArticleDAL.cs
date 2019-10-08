using System;
using System.Linq;
using FoodTruck.Models;

namespace FoodTruck.DAL
{
    class ArticleDAL
    {
        public Article Details(int id)
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
        public void AugmenterQuantiteVendue(int id, int nbre)
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
    }
}
