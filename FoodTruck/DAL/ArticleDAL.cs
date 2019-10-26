using FoodTruck.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FoodTruck.DAL
{
    public class ArticleDAL
    {
        internal Article Details(int id)
        {
            Article lArticle;
            using (dbEntities db = new dbEntities())
            {
                lArticle = (from article in db.Article
                            where article.Id == id
                            select article).FirstOrDefault();
            }
            return lArticle;
        }
        internal Article Details(string nom)
        {
            Article lArticle;
            using (dbEntities db = new dbEntities())
            {
                lArticle = (from article in db.Article
                            where article.Nom == nom
                            select article).FirstOrDefault();
            }
            return lArticle;
        }
        /// <summary>
        /// retourne la liste des familles d'articles. Trié par Id
        /// </summary>
        /// <returns></returns>
        public List<FamilleArticle> FamillesArticle()
        {
            List<FamilleArticle> famillesArticle;
            using (dbEntities db = new dbEntities())
            {
                famillesArticle = (from fa in db.FamilleArticle
                                   orderby fa.Id
                                   select fa).ToList();
            }
            return famillesArticle;
        }

        internal void AugmenterQuantiteVendue(int id, int nbre)
        {
            using (dbEntities db = new dbEntities())
            {
                Article lArticle = (from article in db.Article
                                    where article.Id == id
                                    select article).FirstOrDefault();
                lArticle.NombreVendus += nbre;
                db.SaveChanges();
            }
        }
        /// <summary>
        /// Ajoute l'article lArticle en base
        /// </summary>
        /// <param name="lArticle"></param>
        internal void Ajouter(Article lArticle)
        {
            using (dbEntities db = new dbEntities())
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

        public List<Article> ListerRandom(int nombreRetour, int nombreTop)
        {
            using (dbEntities db = new dbEntities())
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

        public List<Article> ListerArticles(string nomFamille, bool dansCarte, int nombreMax = 200)
        {
            using (dbEntities db = new dbEntities())
            {
                List<Article> articles = (from article in db.Article
                                          join famille in db.FamilleArticle on article.FamilleId equals famille.Id
                                          where article.DansCarte == dansCarte && famille.Nom == nomFamille
                                          orderby article.Nom
                                          select article)
                                          .Take(nombreMax)
                                          .ToList();
                return articles;
            }
        }
        public List<Article> ListerTousArticles(string nomFamille, int nombreMax = 200)
        {
            var articles = ListerArticles(nomFamille, true, nombreMax);
            var articlesPasDansCarte = ListerArticles(nomFamille, false, nombreMax);
            articles.AddRange(articlesPasDansCarte);
            articles = articles.OrderBy(a => a.Nom).ToList();
            return articles;
        }
        public List<Article> ListerArticles(bool dansCarte, int nombreMax = 200)
        {
            using (dbEntities db = new dbEntities())
            {
                List<Article> articles = (from article in db.Article
                                          join famille in db.FamilleArticle on article.FamilleId equals famille.Id
                                          where article.DansCarte == dansCarte
                                          orderby article.FamilleId, article.Nom
                                          select article)
                                          .Take(nombreMax)
                                          .ToList();
                return articles;
            }
        }
        public List<Article> ListerTousArticles(int nombreMax = 200)
        {
            var articles = ListerArticles(true, nombreMax);
            var articlesPasDansCarte = ListerArticles(false, nombreMax);
            articles.AddRange(articlesPasDansCarte);
            articles = articles.OrderBy(a => a.FamilleId).ThenBy(a => a.Nom).ToList();
            return articles;
        }

        internal bool NomExiste(string nom, int id = 0)
        {
            Article lArticle;
            using (dbEntities db = new dbEntities())
            {
                lArticle = (from article in db.Article
                            where article.Nom == nom && article.Id != id
                            select article).FirstOrDefault();
            }
            return lArticle != null ? true : false;
        }

        public List<Article> ListerTout(int nombreMax = 200)
        {
            using (dbEntities db = new dbEntities())
            {
                List<Article> articles = (from article in db.Article
                                          orderby article.FamilleId, article.Nom
                                          select article)
                                          .Take(nombreMax)
                                          .ToList();
                return articles;
            }
        }

        internal void Modifier(Article lArticle)
        {
            using (dbEntities db = new dbEntities())
            {
                Article articleAModifier = (from article in db.Article
                                            where article.Id == lArticle.Id
                                            select article).FirstOrDefault();

                articleAModifier.Nom = lArticle.Nom;
                articleAModifier.Description = lArticle.Description;
                articleAModifier.Prix = lArticle.Prix;
                articleAModifier.Allergenes = lArticle.Allergenes;
                articleAModifier.DansCarte = lArticle.DansCarte;
                articleAModifier.FamilleId = lArticle.FamilleId;
                articleAModifier.Grammage = lArticle.Grammage;
                articleAModifier.Litrage = lArticle.Litrage;
                articleAModifier.Image = lArticle.Image;
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
    }
}
