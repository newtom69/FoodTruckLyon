using FoodTruck.Models;
using FoodTruck.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace FoodTruck.DAL
{
    class CommandeDAL
    {
        internal Commande Detail(int id)
        {
            using (FoodTruckEntities db = new FoodTruckEntities())
            {
                Commande commande = (from cmd in db.Commande
                                     where cmd.Id == id
                                     select cmd).FirstOrDefault();
                return commande;
            }
        }
        internal void Annuler(int id)
        {
            using (FoodTruckEntities db = new FoodTruckEntities())
            {
                Commande commande = (from cmd in db.Commande
                                     where cmd.Id == id
                                     select cmd).FirstOrDefault();
                if (commande != null)
                {
                    commande.Annulation = true;
                    db.SaveChanges();
                }
            }
        }

        public void Ajouter(Commande laCommande, List<ArticleViewModel> articles)
        {
            using (FoodTruckEntities db = new FoodTruckEntities())
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
        internal void MettreAJourStatut(int id, bool retire, bool annule)
        {
            using (FoodTruckEntities db = new FoodTruckEntities())
            {
                Commande commande = (from cmd in db.Commande
                                     where cmd.Id == id
                                     select cmd).FirstOrDefault();
                if (commande != null)
                {
                    commande.Annulation = annule;
                    commande.Retrait = retire;
                }
                db.SaveChanges();
            }
        }

        internal List<Commande> ListerCommandesAStatuer()
        {
            using (FoodTruckEntities db = new FoodTruckEntities())
            {
                DateTime now = DateTime.Now;
                const int intervalleMax = 1;
                var commandes = (from cmd in db.Commande
                                 where !cmd.Retrait && !cmd.Annulation && DbFunctions.DiffHours(cmd.DateRetrait, now) >= intervalleMax
                                 orderby cmd.DateRetrait
                                 select cmd).ToList();
                return commandes;
            }
        }

        public List<Commande> ListerCommandesEnCours()
        {
            using (FoodTruckEntities db = new FoodTruckEntities())
            {
                DateTime now = DateTime.Now;
                const int intervalleMax = 5;
                var commandes = (from cmd in db.Commande
                                 where !cmd.Retrait && Math.Abs((int)DbFunctions.DiffHours(now, cmd.DateRetrait)) < intervalleMax
                                 orderby cmd.DateRetrait
                                 select cmd).ToList();
                return commandes;
            }
        }

        internal List<Commande> ListerCommandesUtilisateur(int id)
        {
            using (FoodTruckEntities db = new FoodTruckEntities())
            {
                DateTime now = DateTime.Now;
                List<Commande> commandes = (from cmd in db.Commande
                                            where cmd.UtilisateurId == id
                                            orderby cmd.Annulation, cmd.Retrait, Math.Abs((int)DbFunctions.DiffHours(now, cmd.DateRetrait))
                                            select cmd).ToList();
                return commandes;
            }
        }
        internal List<Commande> ListerCommandesEnCoursUtilisateur(int id)
        {
            using (FoodTruckEntities db = new FoodTruckEntities())
            {
                DateTime now = DateTime.Now;
                List<Commande> commandes = (from cmd in db.Commande
                                            where cmd.UtilisateurId == id && !cmd.Annulation && !cmd.Retrait && DbFunctions.DiffHours(now, cmd.DateRetrait) > -1
                                            orderby Math.Abs((int)DbFunctions.DiffHours(now, cmd.DateRetrait))
                                            select cmd).ToList();
                return commandes;
            }
        }

        internal List<Commande> ListerCommandesToutes()
        {
            using (FoodTruckEntities db = new FoodTruckEntities())
            {
                List<Commande> commandes = (from cmd in db.Commande
                                            orderby cmd.Id descending
                                            select cmd).ToList();
                return commandes;
            }
        }

        public List<ArticleViewModel> ListerArticles(int commandeId)
        {
            using (FoodTruckEntities db = new FoodTruckEntities())
            {
                var listArticlesQuantites = (from cmd in db.Commande
                                             join ca in db.Commande_Article on cmd.Id equals ca.CommandeId
                                             join article in db.Article on ca.ArticleId equals article.Id
                                             where cmd.Id == commandeId
                                             orderby article.FamilleId, article.Nom
                                             select new { article, ca.Quantite }).ToList();

                List<ArticleViewModel> listArticles = new List<ArticleViewModel>();
                foreach (var articleQuantite in listArticlesQuantites)
                {
                    listArticles.Add(new ArticleViewModel(articleQuantite.article, articleQuantite.Quantite));
                }
                return listArticles;
            }
        }
    }
}
