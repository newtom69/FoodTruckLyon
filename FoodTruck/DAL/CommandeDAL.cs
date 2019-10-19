using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using FoodTruck.Models;
using FoodTruck.ViewModels;

namespace FoodTruck.DAL
{
    class CommandeDAL
    {
        public void Ajouter(Commande laCommande, List<ArticleDetailsViewModel> articles)
        {
            using (foodtruckEntities db = new foodtruckEntities())
            {
                db.Commande.Add(laCommande);
                db.SaveChanges();
                int idCommande = (from cmd in db.Commande
                                  where cmd.UtilisateurId == laCommande.UtilisateurId
                                  orderby cmd.Id descending
                                  select cmd.Id).FirstOrDefault();

                foreach (var article in articles)
                {
                    int quantite = article.Quantite;
                    double prixTotal = (article.Article.Prix * quantite);

                    Commande_Article cmdArt = new Commande_Article
                    {
                        CommandeId = idCommande,
                        ArticleId = article.Article.Id,
                        Quantite = quantite,
                        PrixTotal = prixTotal
                    };
                    db.Commande_Article.Add(cmdArt);
                }
                db.SaveChanges();
            }
        }

        public List<Commande> ListerEnCours()
        {
            using (foodtruckEntities db = new foodtruckEntities())
            {
                DateTime now = DateTime.Now;
                List<Commande> commandes = (from cmd in db.Commande
                                            where cmd.Retrait == false && DbFunctions.DiffMinutes(now, cmd.DateRetrait) > 0
                                            orderby cmd.DateRetrait
                                            select cmd).ToList();
                return commandes;
            }
        }

        public List<Article> ListerArticles(int commandeId)
        {
            using (foodtruckEntities db = new foodtruckEntities())
            {
                List<Article> listArticles = (from cmd in db.Commande
                                              join ca in db.Commande_Article on cmd.Id equals ca.CommandeId
                                              join art in db.Article on ca.ArticleId equals art.Id
                                              where cmd.Id == commandeId
                                              orderby art.FamilleId, art.Nom
                                              select art).ToList();
                return listArticles;
            }
        }



    }
}
