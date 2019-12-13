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
            //Article lArticle;
            //using (foodtruckEntities db = new foodtruckEntities())
            //{
            //    lArticle = (from article in db.Article
            //                where article.Id == id
            //                select article).FirstOrDefault();
            //}
            return new Article();
        }
        internal Article Details(string nom)
        {
            //Article lArticle;
            //using (foodtruckEntities db = new foodtruckEntities())
            //{
            //    lArticle = (from article in db.Article
            //                where article.Nom == nom
            //                select article).FirstOrDefault();
            //}
            return new Article();
        }
        /// <summary>
        /// retourne la liste des familles d'articles. Trié par Id
        /// </summary>
        /// <returns></returns>
        public List<FamilleArticle> FamillesArticle()
        {
            //List<FamilleArticle> famillesArticle;
            //using (foodtruckEntities db = new foodtruckEntities())
            //{
            //    famillesArticle = (from fa in db.FamilleArticle
            //                       orderby fa.Id
            //                       select fa).ToList();
            //}
            OmniFW.Business.CollectionEntite<FamilleArticle> famillesArticle = new OmniFW.Business.CollectionEntite<FamilleArticle>();
            famillesArticle.Rechercher();
            return famillesArticle.Liste.OrderBy(fa => fa.Id).ToList();


        }

        internal void AugmenterQuantiteVendue(int id, int nbre)
        {
            //using (foodtruckEntities db = new foodtruckEntities())
            //{
            //    Article lArticle = (from article in db.Article
            //                        where article.Id == id
            //                        select article).FirstOrDefault();
            //    lArticle.NombreVendus += nbre;
            //    db.SaveChanges();
            //}
        }
        /// <summary>
        /// Ajoute l'article lArticle en base
        /// </summary>
        /// <param name="lArticle"></param>
        internal void Ajouter(Article lArticle)
        {
            //using (foodtruckEntities db = new foodtruckEntities())
            //{
            //    db.Article.Add(lArticle);
            //    try
            //    {
            //        db.SaveChanges();
            //    }
            //    catch (Exception ex)
            //    {
            //        string messageErreur = DALExceptions.HandleException(ex);
            //        throw new Exception(messageErreur);
            //    }
            //}
        }

        public List<Article> Random(int nombreRetour, int nombreTop)
        {
            //using (foodtruckEntities db = new foodtruckEntities())
            //{
            //    List<Article> articles = (from article in db.Article
            //                              where article.DansCarte == true && article.FamilleId <= 3
            //                              orderby article.NombreVendus descending
            //                              select article)
            //                              .Take(nombreTop)
            //                              .OrderBy(random => Guid.NewGuid())
            //                              .Take(nombreRetour)
            //                              .ToList();
            //    return articles;
            //}
            return new List<Article>();
        }

        public List<Article> Articles(bool dansCarteSeulement)
        {
            OmniFW.Business.CollectionEntite<Article> articles = new OmniFW.Business.CollectionEntite<Article>();
            OmniFW.Data.Critere crit = new OmniFW.Data.Critere();
            crit.Parametres.Add(new OmniFW.Data.ParametreSQL("DansCarte", true, System.Data.DbType.Boolean));
            crit.Parametres.Add(new OmniFW.Data.ParametreSQL("DansCarte", dansCarteSeulement, System.Data.DbType.Boolean));
            articles.Rechercher(crit);
            List<Article> listArticles = articles.Liste.OrderBy(art => art.FamilleId).ThenByDescending(art => art.DansCarte).ThenBy(art => art.Nom).ToList();
            return listArticles;
        }

        internal bool NomExiste(string nom, int id = 0)
        {
            //Article lArticle;
            //using (foodtruckEntities db = new foodtruckEntities())
            //{
            //    lArticle = (from article in db.Article
            //                where article.Nom == nom && article.Id != id
            //                select article).FirstOrDefault();
            //}
            //return lArticle != null ? true : false;

            return true;


        }

        public List<Article> Tous(int nombreMax = 1000)
        {
            //using (foodtruckEntities db = new foodtruckEntities())
            //{
            //    List<Article> articles = (from article in db.Article
            //                              orderby article.FamilleId, article.Nom
            //                              select article)
            //                              .Take(nombreMax)
            //                              .ToList();
            //    return articles;
            //}
            return new List<Article>();
        }

        internal void Modifier(Article article)
        {
            //using (foodtruckEntities db = new foodtruckEntities())
            //{
            //    Article articleAModifier = (from art in db.Article
            //                                where art.Id == article.Id
            //                                select art).FirstOrDefault();

            //    articleAModifier.Nom = article.Nom;
            //    articleAModifier.Description = article.Description;
            //    articleAModifier.PrixTTC = article.PrixTTC;
            //    articleAModifier.Allergenes = article.Allergenes;
            //    articleAModifier.DansCarte = article.DansCarte;
            //    articleAModifier.FamilleId = article.FamilleId;
            //    articleAModifier.Grammage = article.Grammage;
            //    articleAModifier.Litrage = article.Litrage;
            //    articleAModifier.Image = article.Image;
            //    try
            //    {
            //        db.SaveChanges();
            //    }
            //    catch (Exception ex)
            //    {
            //        string messageErreur = DALExceptions.HandleException(ex);
            //        throw new Exception(messageErreur);
            //    }
            //}
        }
    }
}
