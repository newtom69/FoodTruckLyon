using System;
using System.Collections.Generic;
using System.Linq;
using FoodTruck.Models;

namespace FoodTruck.DAL
{
    class PanierProspectDAL
    {
        public string ProspectGuid { get; set; }

        public PanierProspectDAL(string prospectGuid)
        {
            ProspectGuid = prospectGuid;
        }

        ///Ajouter un article non présent au panier en base d'un prospect
        public void Ajouter(Article lArticle, int quantite = 1)
        {
            PanierProspect panierProspect = new PanierProspect
            {
                ArticleId = lArticle.Id,
                ProspectGuid = this.ProspectGuid,
                Quantite = quantite,
                PrixTotal = Math.Round(quantite * lArticle.Prix, 2)
            };
            using (foodtruckEntities db = new foodtruckEntities())
            {
                db.PanierProspect.Add(panierProspect);
                db.SaveChanges();
            }
        }

        ///Modifier la quantité d'un article du panier en base d'un utilisateur
        public void ModifierQuantite(Article lArticle, int quantite)
        {
            using (foodtruckEntities db = new foodtruckEntities())
            {
                PanierProspect panierProspect = (from panier in db.PanierProspect
                                                 where panier.ProspectGuid == ProspectGuid && panier.ArticleId == lArticle.Id
                                                 select panier).FirstOrDefault();
                panierProspect.Quantite += quantite;
                panierProspect.PrixTotal = Math.Round(panierProspect.PrixTotal + quantite * lArticle.Prix, 2);
                db.SaveChanges();
            }
        }

        /// Supprimer l'article du panier en base de l'utilisateur
        public void Supprimer(Article lArticle)
        {
            using (foodtruckEntities db = new foodtruckEntities())
            {
                PanierProspect panierProspect = (from panier in db.PanierProspect
                                                 where panier.ProspectGuid == ProspectGuid && panier.ArticleId == lArticle.Id
                                                 select panier).FirstOrDefault();

                db.PanierProspect.Remove(panierProspect);
                db.SaveChanges();
            }
        }

        /// Supprimer le panier en base de l'utilisateur
        public void Supprimer()
        {
            using (foodtruckEntities db = new foodtruckEntities())
            {
                var panierProspect = from panier in db.PanierProspect
                                     where panier.ProspectGuid == ProspectGuid
                                     select panier;

                db.PanierProspect.RemoveRange(panierProspect);
                db.SaveChanges();
            }
        }

        public List<PanierProspect> ListerPanierUtilisateur()
        {
            using (foodtruckEntities db = new foodtruckEntities())
            {
                List<PanierProspect> paniers = (from panier in db.PanierProspect
                                                join article in db.Article on panier.ArticleId equals article.Id
                                                where panier.ProspectGuid == ProspectGuid
                                                select panier).ToList();
                return paniers;
            }
        }

        public List<Article> ListerArticlesPanierUtilisateur()
        {
            using (foodtruckEntities db = new foodtruckEntities())
            {
                List<Article> articles = (from panier in db.PanierProspect
                                          join article in db.Article on panier.ArticleId equals article.Id
                                          where panier.ProspectGuid == ProspectGuid
                                          select article).ToList();
                return articles;
            }
        }

    }
}