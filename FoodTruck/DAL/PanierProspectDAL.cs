using FoodTruck.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace FoodTruck.DAL
{
    public class PanierProspectDAL
    {
        public string ProspectGuid { get; set; }

        public PanierProspectDAL(string prospectGuid)
        {
            ProspectGuid = prospectGuid;
        }

        public List<PanierProspect> ListerPanierProspect()
        {
            //using (foodtruckEntities db = new foodtruckEntities())
            //{
            //    List<PanierProspect> paniers = (from panier in db.PanierProspect
            //                                    join article in db.Article on panier.ArticleId equals article.Id
            //                                    where panier.ProspectGuid == ProspectGuid
            //                                    select panier).ToList();
            //    return paniers;
            //}

            //Article article = new Article(ProspectGuid);
            //article.Lire();

            //TEST OMNIFW
            //PanierProspect panierProspect = new PanierProspect(ProspectGuid);
            //panierProspect.Lire();
            List<PanierProspect> paniersProspect = new List<PanierProspect>();
            return paniersProspect;
        }

        ///Ajouter un article non présent au panier en base d'un prospect
        public void Ajouter(Article lArticle, int quantite = 1)
        {
            PanierProspect panierProspect = new PanierProspect
            {
                ArticleId = lArticle.Id,
                ProspectGuid = this.ProspectGuid,
                Quantite = quantite,
                PrixTotal = Math.Round(quantite * lArticle.PrixTTC, 2),
                DateAjout = DateTime.Now,
            };
            //using (foodtruckEntities db = new foodtruckEntities())
            //{
            //    db.PanierProspect.Add(panierProspect);
            //    db.SaveChanges();
            //}
            throw new NotImplementedException();
        }

        ///Modifier la quantité d'un article du panier en base d'un client
        public void ModifierQuantite(Article lArticle, int quantite)
        {
            //using (foodtruckEntities db = new foodtruckEntities())
            //{
            //    PanierProspect panierProspect = (from panier in db.PanierProspect
            //                                     where panier.ProspectGuid == ProspectGuid && panier.ArticleId == lArticle.Id
            //                                     select panier).FirstOrDefault();
            //    panierProspect.Quantite += quantite;
            //    panierProspect.PrixTotal = Math.Round(panierProspect.PrixTotal + quantite * lArticle.PrixTTC, 2);
            //    panierProspect.DateAjout = DateTime.Now;
            //    db.SaveChanges();
            //}
            throw new NotImplementedException();
        }

        /// Supprimer l'article du panier en base du client
        public void Supprimer(Article lArticle)
        {
            //using (foodtruckEntities db = new foodtruckEntities())
            //{
            //    PanierProspect panierProspect = (from panier in db.PanierProspect
            //                                     where panier.ProspectGuid == ProspectGuid && panier.ArticleId == lArticle.Id
            //                                     select panier).FirstOrDefault();

            //    db.PanierProspect.Remove(panierProspect);
            //    db.SaveChanges();
            //}
            throw new NotImplementedException();
        }

        /// Supprimer le panier en base du client
        public void Supprimer()
        {

            //    var panierProspect = from panier in db.PanierProspect
            //                         where panier.ProspectGuid == ProspectGuid
            //                         select panier;

            //    db.PanierProspect.RemoveRange(panierProspect);


            //TODO OMNIFW
        }

        public List<Article> ArticlesPanierProspect()
        {
            //using (foodtruckEntities db = new foodtruckEntities())
            //{
            //    List<Article> articles = (from panier in db.PanierProspect
            //                              join article in db.Article on panier.ArticleId equals article.Id
            //                              where panier.ProspectGuid == ProspectGuid
            //                              select article).ToList();
            //    return articles;
            //}
            //TODO OMNIFW
            return new List<Article>();
        }
        /// <summary>
        /// Purge les entrées de la table PanierProspect dont le prospect à ajouter à son panier un article depuis plus de ageEnJours jours
        /// </summary>
        /// <param name="ageEnJours"></param>
        /// <returns></returns>
        public int Purger(int ageEnJours)
        {
            //using (foodtruckEntities db = new foodtruckEntities())
            //{
            //    IQueryable<string> GuidsAGarder = (from panier in db.PanierProspect
            //                                       where DbFunctions.DiffDays(panier.DateAjout, DateTime.Today) < ageEnJours
            //                                       select panier.ProspectGuid).Distinct();

            //    List<PanierProspect> paniersAPurger = (from panier in db.PanierProspect
            //                                           where !GuidsAGarder.Any(guid => panier.ProspectGuid.Contains(guid))
            //                                           select panier).ToList();

            //    db.PanierProspect.RemoveRange(paniersAPurger);
            //    return db.SaveChanges();
            //}
            throw new NotImplementedException();
        }
        internal List<Article> SupprimerArticlesPasDansCarte()
        {
            List<Article> articles = ArticlesPanierProspect();
            List<Article> articlesPasDansCarte = articles.FindAll(art => !art.DansCarte);

            //using (foodtruckEntities db = new foodtruckEntities())
            //{
            //    var paniersASupprimer = (from panier in db.PanierProspect
            //                             join article in db.Article on panier.ArticleId equals article.Id
            //                             where panier.ProspectGuid == ProspectGuid && !article.DansCarte
            //                             select panier).ToList();

            //    db.PanierProspect.RemoveRange(paniersASupprimer);
            //    db.SaveChanges();
            //}
            //return articlesPasDansCarte;

            //TODO OMNIFW
            return articles;
        }
    }
}