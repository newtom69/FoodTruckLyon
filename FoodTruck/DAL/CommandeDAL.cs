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
            using (foodtruckEntities db = new foodtruckEntities())
            {
                Commande commande = (from cmd in db.Commande
                                     where cmd.Id == id
                                     select cmd).FirstOrDefault();
                return commande;
            }
        }
        internal void Annuler(int id)
        {
            MettreAJourStatut(id, false, true);
        }

        public void Ajouter(Commande commande, List<ArticleViewModel> articles)
        {
            using (foodtruckEntities db = new foodtruckEntities())
            {
                db.Commande.Add(commande);
                db.SaveChanges();
                int idCommande = (from cmd in db.Commande
                                  where cmd.UtilisateurId == commande.UtilisateurId
                                  orderby cmd.Id descending
                                  select cmd.Id).FirstOrDefault();

                foreach (var article in articles)
                {
                    int quantite = article.Quantite;
                    double prixTotal = Math.Round(article.Article.Prix * quantite, 2);
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
            using (foodtruckEntities db = new foodtruckEntities())
            {
                Commande commande = (from cmd in db.Commande
                                     where cmd.Id == id
                                     select cmd).FirstOrDefault();
                if (commande != null)
                {
                    commande.Annulation = annule;
                    commande.Retrait = retire;
                    Utilisateur utilisateur = (from u in db.Utilisateur
                                               where u.Id == commande.UtilisateurId
                                               select u).FirstOrDefault();
                    if (commande.Retrait)
                        utilisateur.Points += (int)commande.PrixTotal / 10;
                    if (commande.Annulation)
                        utilisateur.Points += (int)commande.RemiseFidelite;
                    db.SaveChanges();
                }
            }
        }

        internal List<Commande> ListerCommandesAStatuer()
        {
            using (foodtruckEntities db = new foodtruckEntities())
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

        public List<Commande> ListerCommandesEnCours(int fourchetteHeures)
        {
            using (foodtruckEntities db = new foodtruckEntities())
            {
                DateTime now = DateTime.Now;
                var commandes = (from cmd in db.Commande
                                 where !cmd.Retrait && Math.Abs((int)DbFunctions.DiffHours(now, cmd.DateRetrait)) < Math.Abs(fourchetteHeures)
                                 orderby cmd.DateRetrait
                                 select cmd).ToList();
                return commandes;
            }
        }

        internal List<Commande> ListerCommandesUtilisateur(int id)
        {
            using (foodtruckEntities db = new foodtruckEntities())
            {
                DateTime now = DateTime.Now;
                List<Commande> commandes = (from cmd in db.Commande
                                            where cmd.UtilisateurId == id
                                            orderby cmd.Annulation, cmd.Retrait, Math.Abs((int)DbFunctions.DiffHours(now, cmd.DateRetrait))
                                            select cmd).ToList();
                return commandes;
            }
        }

        internal double RemiseTotaleUtilisateur(int id)
        {
            double remise = 0;
            List<Commande> commandes = ListerCommandesUtilisateur(id);
            foreach (Commande commande in commandes)
            {
                remise += commande.RemiseCommerciale + commande.RemiseFidelite;
            }
            remise = Math.Round(remise, 2);
            return remise;
        }

        internal int NombreCommandes(DateTime date)
        {
            using (foodtruckEntities db = new foodtruckEntities())
            {
                int nbCommandes = (from cmd in db.Commande
                                   where cmd.DateRetrait == date && !cmd.Annulation && !cmd.Retrait
                                   select cmd.Id).Count();
                return nbCommandes;
            }
        }

        internal List<Commande> ListerCommandesEnCoursUtilisateur(int id)
        {
            using (foodtruckEntities db = new foodtruckEntities())
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
            using (foodtruckEntities db = new foodtruckEntities())
            {
                List<Commande> commandes = (from cmd in db.Commande
                                            orderby cmd.Id descending
                                            select cmd).ToList();
                return commandes;
            }
        }
        internal List<Commande> ListerCommandesAVenir()
        {
            DateTime maintenant = DateTime.Now;
            using (foodtruckEntities db = new foodtruckEntities())
            {
                List<Commande> commandes = (from cmd in db.Commande
                                            where DbFunctions.DiffDays(maintenant, cmd.DateRetrait) >= 0 && !cmd.Retrait && !cmd.Annulation
                                            orderby cmd.DateRetrait
                                            select cmd).ToList();
                return commandes;
            }
        }

        public List<ArticleViewModel> ListerArticles(int commandeId)
        {
            using (foodtruckEntities db = new foodtruckEntities())
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
