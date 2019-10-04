using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using testLinq.Models;

namespace testLinq.DAL
{
    class CommandeDAL
    {
        public void Ajouter(Commande laCommande)
        {
            foodtruckEntities db = new foodtruckEntities();
            db.Commande.Add(laCommande);
            db.SaveChanges();
            int idCommande = (from cmd in db.Commande
                              where cmd.UtilisateurId == laCommande.UtilisateurId
                              orderby cmd.Id descending
                              select cmd.Id).FirstOrDefault();

            List<Article> listeArticles = new List<Article>(); //TODO
            ArticleDAL articleDAL = new ArticleDAL();
            listeArticles.Add(articleDAL.Details(5));
            listeArticles.Add(articleDAL.Details(1));
            listeArticles.Add(articleDAL.Details(3)); 

            foreach (var article in listeArticles)
            {
                int quantite = 1; //= article.Quantite; //TODO
                double prixTotal = (article.Prix * quantite);

                Commande_Article cmdArt = new Commande_Article();
                cmdArt.CommandeId = idCommande;
                cmdArt.ArticleId = article.Id;
                cmdArt.Quantite = quantite;
                cmdArt.PrixTotal = prixTotal;
                db.Commande_Article.Add(cmdArt);
                //db.SaveChanges();
            }

            db.SaveChanges();
        }
    }
}
