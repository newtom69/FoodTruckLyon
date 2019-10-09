using System.Collections.Generic;
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
    }
}
