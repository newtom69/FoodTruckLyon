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
            using (FoodTruckEntities db = new FoodTruckEntities())
            {
                List<PanierProspect> paniers = (from panier in db.PanierProspect
                                                join article in db.Article on panier.ArticleId equals article.Id
                                                where panier.ProspectGuid == ProspectGuid
                                                select panier).ToList();
                return paniers;
            }
        }

        ///Ajouter un article non présent au panier en base d'un prospect
        public void Ajouter(Article lArticle, int quantite = 1)
        {
            PanierProspect panierProspect = new PanierProspect
            {
                ArticleId = lArticle.Id,
                ProspectGuid = this.ProspectGuid,
                Quantite = quantite,
                PrixTotal = Math.Round(quantite * lArticle.Prix, 2),
                DateAjout = DateTime.Now,
            };
            using (FoodTruckEntities db = new FoodTruckEntities())
            {
                db.PanierProspect.Add(panierProspect);
                db.SaveChanges();
            }
        }

        ///Modifier la quantité d'un article du panier en base d'un utilisateur
        public void ModifierQuantite(Article lArticle, int quantite)
        {
            using (FoodTruckEntities db = new FoodTruckEntities())
            {
                PanierProspect panierProspect = (from panier in db.PanierProspect
                                                 where panier.ProspectGuid == ProspectGuid && panier.ArticleId == lArticle.Id
                                                 select panier).FirstOrDefault();
                panierProspect.Quantite += quantite;
                panierProspect.PrixTotal = Math.Round(panierProspect.PrixTotal + quantite * lArticle.Prix, 2);
                panierProspect.DateAjout = DateTime.Now;
                db.SaveChanges();
            }
        }

        /// Supprimer l'article du panier en base de l'utilisateur
        public void Supprimer(Article lArticle)
        {
            using (FoodTruckEntities db = new FoodTruckEntities())
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
            using (FoodTruckEntities db = new FoodTruckEntities())
            {
                var panierProspect = from panier in db.PanierProspect
                                     where panier.ProspectGuid == ProspectGuid
                                     select panier;

                db.PanierProspect.RemoveRange(panierProspect);
                db.SaveChanges();
            }
        }

        public List<Article> ListerArticlesPanierProspect()
        {
            using (FoodTruckEntities db = new FoodTruckEntities())
            {
                List<Article> articles = (from panier in db.PanierProspect
                                          join article in db.Article on panier.ArticleId equals article.Id
                                          where panier.ProspectGuid == ProspectGuid
                                          select article).ToList();
                return articles;
            }
        }
        /// <summary>
        /// Purge les entrées de la table PanierProspect dont le prospect à ajouter à son panier un article depuis plus de ageEnJours jours
        /// </summary>
        /// <param name="ageEnJours"></param>
        /// <returns></returns>
        public int Purger(int ageEnJours)
        {
            using (FoodTruckEntities db = new FoodTruckEntities())
            {
                DateTime now = DateTime.Now;

                // requête SQL :
                //select*
                //from PanierProspect
                //where ProspectGuid not in (SELECT DISTINCT ProspectGuid
                //                          from PanierProspect
                //                          where DATEDIFF(day, DateAjout, CURRENT_TIMESTAMP) < ageEnJours)

                var GuidsAGarder = (from panier in db.PanierProspect
                                    where DbFunctions.DiffDays(panier.DateAjout, now) < ageEnJours
                                    select panier.ProspectGuid).Distinct();

                var paniersAPurger = (from panier in db.PanierProspect
                                      where !GuidsAGarder.Any(guid => panier.ProspectGuid.Contains(guid))
                                      select panier).ToList();


                db.PanierProspect.RemoveRange(paniersAPurger);
                db.SaveChanges();
                return paniersAPurger.Count;
            }
        }
    }
}