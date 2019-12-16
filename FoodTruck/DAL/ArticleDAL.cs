using FoodTruck.Models;
using OmniFW.Business;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FoodTruck.DAL
{
    public class ArticleDAL
    {
        internal Article Details(int id)
        {
            Article article = new Article(id);
            article.Lire();
            return article;
        }
        internal Article Details(string nom)
        {
            Article article = new Article(nom);
            article.Lire();
            return article;
        }
        /// <summary>
        /// retourne la liste des familles d'articles. Trié par Id
        /// </summary>
        /// <returns></returns>
        public List<FamilleArticle> FamillesArticle()
        {
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
            OmniFW.Business.CollectionEntite<Article> articles = new OmniFW.Business.CollectionEntite<Article>();
            OmniFW.Data.Critere critereCarte = new OmniFW.Data.Critere();
            critereCarte.Parametres.Add(new OmniFW.Data.ParametreSQL("DansCarte", true, System.Data.DbType.Boolean));
            OmniFW.Data.Critere critereFamille = new OmniFW.Data.Critere();
            critereFamille.Parametres.Add(new OmniFW.Data.ParametreSQL("FamilleId", 1, System.Data.DbType.Int32));
            critereFamille.Parametres.Add(new OmniFW.Data.ParametreSQL("FamilleId", 2, System.Data.DbType.Int32));
            critereFamille.Parametres.Add(new OmniFW.Data.ParametreSQL("FamilleId", 3, System.Data.DbType.Int32));
            articles.Rechercher(critereCarte, critereFamille);
            List<Article> listArticles = articles.Liste.OrderByDescending(art => art.NombreVendus).Take(nombreTop).OrderBy(random => Guid.NewGuid()).Take(nombreRetour).ToList();
            return listArticles;
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
            //    lArticle = (from article in db.Article
            //                where article.Nom == nom && article.Id != id
            //                select article).FirstOrDefault();
            //}
            //return lArticle != null ? true : false;

            Article article = new Article(nom);
            article.Lire();
            return article.Id != -1 && article.Id != id ? true : false;
        }

        public List<Article> Tous(int nombreMax = 1000)
        {
            CollectionEntite<Article> articles = new CollectionEntite<Article>();
            articles.Rechercher();
            List<Article> listArticles = articles.Liste.OrderBy(art => art.FamilleId).ThenBy(art => art.Nom).ToList().Take(nombreMax).ToList();
            return listArticles;
        }

        internal void Modifier(Article article)
        {
            article.Enregistrer();
        }
    }
}
