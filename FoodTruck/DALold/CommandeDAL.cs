using FoodTruck.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;

namespace FoodTruck.DAL2
{
    public class CommandeDAL
    {
        public string strPrixTotal { get; set; }

        public void Ajouter(Commande laCommande)
        {
            using (SqlConnection connection = new SqlConnection())
            {
                ConnectionStringSettings connex = ConfigurationManager.ConnectionStrings["ServeurTestUser"];
                connection.ConnectionString = connex.ConnectionString;
                connection.Open();
                strPrixTotal = laCommande.PrixTotal.ToString(CultureInfo.InvariantCulture);
                string stringDateCommande = laCommande.DateCommande.ToString("yyyy-MM-dd HH:mm:ss");
                string stringDateLivraison = laCommande.DateLivraison.ToString("yyyy-MM-dd HH:mm:ss");
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "INSERT INTO Commande " +
                    "(UtilisateurId, DateCommande, DateLivraison, ModeLivraison, PrixTotal) " +
                    "VALUES " +
                   $"('{laCommande.UtilisateurId}','{stringDateCommande}','{stringDateLivraison}','{laCommande.ModeLivraison}','{strPrixTotal}')";

                    command.ExecuteNonQuery();
                }
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = $"SELECT TOP 1 Id FROM Commande WHERE UtilisateurId={laCommande.UtilisateurId} ORDER BY Id DESC";
                    laCommande.Id = (int)command.ExecuteScalar();
                }

                foreach (var article in laCommande.listeArticles)
                {
                    int articleId = article.Id;
                    double articlePrix = article.Prix;
                    int quantite = article.Quantite;
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        string strArticlePrixTotal = (articlePrix * quantite).ToString(CultureInfo.InvariantCulture);
                        command.CommandText = "insert into Commande_Article " +
                            "(CommandeId, ArticleId, Quantite, PrixTotal) " +
                            $"values({laCommande.Id}, {articleId}, {quantite}, {strArticlePrixTotal})";

                        command.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}