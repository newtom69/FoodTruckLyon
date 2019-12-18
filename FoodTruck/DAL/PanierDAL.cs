using FoodTruck.Models;
using OmniFW.Business;
using OmniFW.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FoodTruck.DAL
{
    class PanierDAL
    {
        public int ClientId { get; set; }

        public PanierDAL(int clientId)
        {
            ClientId = clientId;
        }

        public List<Panier> ListerPanierClient()
        {
            CollectionEntite<Panier> paniers = new CollectionEntite<Panier>();
            paniers.ProcedureRechercher = "ListerPanierClient";
            Critere critereClient = new Critere();
            critereClient.Parametres.Add(new ParametreSQL("ClientId", ClientId, System.Data.DbType.Int32));
            paniers.Rechercher(critereClient);
            return paniers.Liste;
        }

        ///Ajouter un article non présent au panier en base d'un client
        public void Ajouter(Article article, int quantite = 1)
        {
            Panier panier = new Panier
            {
                ArticleId = article.Id,
                ClientId = ClientId,
                Quantite = quantite,
                PrixTotal = Math.Round(quantite * article.PrixTTC, 2)
            };
            panier.Enregistrer();
        }

        ///Modifier la quantité d'un article du panier en base d'un client
        public void ModifierQuantite(Article article, int quantite)
        {
            CollectionEntite<Panier> paniers = new CollectionEntite<Panier>();
            Critere critereClient = new Critere();
            critereClient.Parametres.Add(new ParametreSQL("ClientId", ClientId, System.Data.DbType.Int32));
            Critere critereArticle = new Critere();
            critereArticle.Parametres.Add(new ParametreSQL("ArticleId", article.Id, System.Data.DbType.Int32));
            paniers.Rechercher(critereClient, critereArticle);
            Panier panier = paniers.Liste.FirstOrDefault();
            panier.Quantite += quantite;
            panier.PrixTotal = Math.Round(panier.PrixTotal + quantite * article.PrixTTC, 2);
            panier.Enregistrer();
        }

        /// Supprimer l'article du panier en base du client
        public void Supprimer(Article article)
        {
            Panier panier = new Panier();
            Critere critereClient = new Critere();
            critereClient.Parametres.Add(new ParametreSQL("ClientId", ClientId, System.Data.DbType.Int32));
            Critere critereArticle = new Critere();
            critereArticle.Parametres.Add(new ParametreSQL("ArticleId", article.Id, System.Data.DbType.Int32));
            panier.ProcedureSupprimer = "PanierSupprimer";
            panier.Supprimer(critereClient, critereArticle);
        }

        /// Supprimer le panier en base du client
        public void Supprimer()
        {
            //using (foodtruckEntities db = new foodtruckEntities())
            //{
            //    var panier = from p in db.Panier
            //                 where p.ClientId == ClientId
            //                 select p;

            //    db.Panier.RemoveRange(panier);
            //    db.SaveChanges();
            //}
            throw new NotImplementedException();
        }

        public List<Article> ArticlesPanierClient()
        {
            CollectionEntite<Article> articles = new CollectionEntite<Article>();
            articles.ProcedureRechercher = "ArticlesPanierClient";
            Critere critereClient = new Critere();
            critereClient.Parametres.Add(new ParametreSQL("ClientId", ClientId, System.Data.DbType.Int32));
            articles.Rechercher(critereClient);
            return articles.Liste;
        }

        internal List<Article> SupprimerArticlesPasDansCarte()
        {
            List<Article> articles = ArticlesPanierClient();
            List<Article> articlesPasDansCarte = articles.FindAll(art => !art.DansCarte);

            //using (foodtruckEntities db = new foodtruckEntities())
            //{
            //    var paniersASupprimer = (from panier in db.Panier
            //                             join article in db.Article on panier.ArticleId equals article.Id
            //                             where panier.ClientId == ClientId && !article.DansCarte
            //                             select panier).ToList();

            //    db.Panier.RemoveRange(paniersASupprimer);
            //    db.SaveChanges();
            //}
            //return articlesPasDansCarte;


            //TODO OMNIFW
            return articles;
        }
    }
}