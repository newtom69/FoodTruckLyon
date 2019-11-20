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
            using (foodtruckEntities db = new foodtruckEntities())
            {
                List<Panier> paniers = (from panier in db.Panier
                                        join article in db.Article on panier.ArticleId equals article.Id
                                        where panier.UtilisateurId == UtilisateurId
                                        select panier).ToList();
                return paniers;
            }
        }

        ///Ajouter un article non présent au panier en base d'un utilisateur
        public void Ajouter(Article article, int quantite = 1)
        {
            Panier lePanier = new Panier
            {
                ArticleId = article.Id,
                UtilisateurId = UtilisateurId,
                Quantite = quantite,
                PrixTotal = Math.Round(quantite * article.Prix, 2)
            };
            using (foodtruckEntities db = new foodtruckEntities())
            {
                db.Panier.Add(lePanier);
                db.SaveChanges();
            }
        }

        ///Modifier la quantité d'un article du panier en base d'un utilisateur
        public void ModifierQuantite(Article article, int quantite)
        {
            using (foodtruckEntities db = new foodtruckEntities())
            {
                Panier lePanier = (from panier in db.Panier
                                   where panier.UtilisateurId == UtilisateurId && panier.ArticleId == article.Id
                                   select panier).FirstOrDefault();
                lePanier.Quantite += quantite;
                lePanier.PrixTotal = Math.Round(lePanier.PrixTotal + quantite * article.Prix, 2);
                db.SaveChanges();
            }
        }

        /// Supprimer l'article du panier en base de l'utilisateur
        public void Supprimer(Article article)
        {
            using (foodtruckEntities db = new foodtruckEntities())
            {
                Panier lePanier = (from panier in db.Panier
                                   where panier.UtilisateurId == UtilisateurId && panier.ArticleId == article.Id
                                   select panier).FirstOrDefault();

                db.Panier.Remove(lePanier);
                db.SaveChanges();
            }
        }

        /// Supprimer le panier en base de l'utilisateur
        public void Supprimer()
        {
            using (foodtruckEntities db = new foodtruckEntities())
            {
                var lePanier = from panier in db.Panier
                               where panier.UtilisateurId == UtilisateurId
                               select panier;

                db.Panier.RemoveRange(lePanier);
                db.SaveChanges();
            }
        }

        public List<Article> ArticlesPanierUtilisateur()
        {
            using (foodtruckEntities db = new foodtruckEntities())
            {
                List<Article> articles = (from panier in db.Panier
                                          join article in db.Article on panier.ArticleId equals article.Id
                                          where panier.UtilisateurId == UtilisateurId
                                          select article).ToList();
                return articles;
            }
        }

        internal List<Article> SupprimerArticlesPasDansCarte()
        {
            List<Article> articles = ArticlesPanierUtilisateur();
            List<Article> articlesPasDansCarte = articles.FindAll(art => !art.DansCarte);

            using (foodtruckEntities db = new foodtruckEntities())
            {
                var paniersASupprimer = (from panier in db.Panier
                                         join article in db.Article on panier.ArticleId equals article.Id
                                         where panier.UtilisateurId == UtilisateurId && !article.DansCarte
                                         select panier).ToList();

                db.Panier.RemoveRange(paniersASupprimer);
                db.SaveChanges();
            }
            return articlesPasDansCarte;
        }
    }
}