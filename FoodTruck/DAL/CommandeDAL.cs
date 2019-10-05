using System.Collections.Generic;
using System.Linq;
using FoodTruck.Models;

namespace FoodTruck.DAL
{
    class CommandeDAL
    {
        public void Ajouter(Commande laCommande, List<ArticleUI> listeArticlesUI)
        {
            using (foodtruckEntities db = new foodtruckEntities())
            {
                db.Commande.Add(laCommande);
                db.SaveChanges();
                int idCommande = (from cmd in db.Commande
                                  where cmd.UtilisateurId == laCommande.UtilisateurId
                                  orderby cmd.Id descending
                                  select cmd.Id).FirstOrDefault();

                foreach (var article in listeArticlesUI)
                {
                    int quantite = article.Quantite;
                    double prixTotal = (article.Prix * quantite);

                    Commande_Article cmdArt = new Commande_Article();
                    cmdArt.CommandeId = idCommande;
                    cmdArt.ArticleId = article.Id;
                    cmdArt.Quantite = quantite;
                    cmdArt.PrixTotal = prixTotal;
                    db.Commande_Article.Add(cmdArt);
                }
                db.SaveChanges();
            }
        }
    }
}
