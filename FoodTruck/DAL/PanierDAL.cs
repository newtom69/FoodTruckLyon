using FoodTruck.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FoodTruck.DAL
{
    class PanierDAL
    {
        public int UtilisateurId { get; set; }

        public PanierDAL(int utilisateurId)
        {
            UtilisateurId = utilisateurId;
        }

        public List<Panier> ListerPanierUtilisateur()
        {
            using (dbEntities db = new dbEntities())
            {
                List<Panier> paniers = (from panier in db.Panier
                                        join article in db.Article on panier.ArticleId equals article.Id
                                        where panier.UtilisateurId == UtilisateurId
                                        select panier).ToList();
                return paniers;
            }
        }

        ///Ajouter un article non présent au panier en base d'un utilisateur
        public void Ajouter(Article lArticle, int quantite = 1)
        {
            Panier lePanier = new Panier
            {
                ArticleId = lArticle.Id,
                UtilisateurId = UtilisateurId,
                Quantite = quantite,
                PrixTotal = Math.Round(quantite * lArticle.Prix, 2)
            };
            using (dbEntities db = new dbEntities())
            {
                db.Panier.Add(lePanier);
                db.SaveChanges();
            }
        }

        ///Modifier la quantité d'un article du panier en base d'un utilisateur
        public void ModifierQuantite(Article lArticle, int quantite)
        {
            using (dbEntities db = new dbEntities())
            {
                Panier lePanier = (from panier in db.Panier
                                   where panier.UtilisateurId == UtilisateurId && panier.ArticleId == lArticle.Id
                                   select panier).FirstOrDefault();
                lePanier.Quantite += quantite;
                lePanier.PrixTotal = Math.Round(lePanier.PrixTotal + quantite * lArticle.Prix, 2);
                db.SaveChanges();
            }
        }

        /// Supprimer l'article du panier en base de l'utilisateur
        public void Supprimer(Article lArticle)
        {
            using (dbEntities db = new dbEntities())
            {
                Panier lePanier = (from panier in db.Panier
                                   where panier.UtilisateurId == UtilisateurId && panier.ArticleId == lArticle.Id
                                   select panier).FirstOrDefault();

                db.Panier.Remove(lePanier);
                db.SaveChanges();
            }
        }

        /// Supprimer le panier en base de l'utilisateur
        public void Supprimer()
        {
            using (dbEntities db = new dbEntities())
            {
                var lePanier = from panier in db.Panier
                               where panier.UtilisateurId == UtilisateurId
                               select panier;

                db.Panier.RemoveRange(lePanier);
                db.SaveChanges();
            }
        }

        public List<Article> ListerArticlesPanierUtilisateur()
        {
            using (dbEntities db = new dbEntities())
            {
                List<Article> articles = (from panier in db.Panier
                                          join article in db.Article on panier.ArticleId equals article.Id
                                          where panier.UtilisateurId == UtilisateurId
                                          select article).ToList();
                return articles;
            }
        }

    }
}