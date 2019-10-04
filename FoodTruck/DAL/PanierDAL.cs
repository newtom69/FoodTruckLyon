using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;

namespace FoodTruck.DAL
{
    public class PanierDAL
    {
        public int UtilisateurId;
        public List<Article> listeArticles { get; set; }


        public PanierDAL(int utilisateurId)
        {
            UtilisateurId = utilisateurId;
        }

        //Ajouter un article au panier en base d'un utilisateur
        public void Ajouter(Article lArticle)
        {
            int articleId = lArticle.Id;
            string prixTotal = lArticle.Prix.ToString(CultureInfo.InvariantCulture);

            using (SqlConnection connection = new SqlConnection())
            {
                ConnectionStringSettings connex = ConfigurationManager.ConnectionStrings["ServeurTestUser"];
                connection.ConnectionString = connex.ConnectionString;
                connection.Open();

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText =
                       " INSERT into Panier"+
                       " (UtilisateurId, ArticleId, Quantite, PrixTotal)" +
                      $" VALUES({UtilisateurId}, {articleId}, 1, {prixTotal})";

                    command.ExecuteNonQuery();
                }
            }
        }

        // augmenter d'1 la quantité d'un article du panier en base d'un utilisateur
        public void ModifierQuantite(Article lArticle, int quantite)
        {
            int articleId = lArticle.Id;
            string articlePrix = lArticle.Prix.ToString(CultureInfo.InvariantCulture);

            using (SqlConnection connection = new SqlConnection())
            {
                ConnectionStringSettings connex = ConfigurationManager.ConnectionStrings["ServeurTestUser"];
                connection.ConnectionString = connex.ConnectionString;
                connection.Open();
                
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText =
                        " UPDATE  Panier" +
                       $" SET Quantite = Quantite + {quantite}, PrixTotal = round(PrixTotal + {quantite}*{articlePrix},2)" +
                       $" WHERE UtilisateurId = {UtilisateurId} and ArticleId = {articleId}";

                    command.ExecuteNonQuery();
                }
            }
        }

        // Supprimer l'article du panier en base de l'utilisateur
        public void Supprimer(Article lArticle)
        {
            int articleId = lArticle.Id;
           
            using (SqlConnection connection = new SqlConnection())
            {
                ConnectionStringSettings connex = ConfigurationManager.ConnectionStrings["ServeurTestUser"];
                connection.ConnectionString = connex.ConnectionString;
                connection.Open();

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText =
                        " DELETE from Panier" +
                        $" WHERE UtilisateurId={UtilisateurId} and ArticleId = {articleId}";
                    
                    command.ExecuteNonQuery();
                }
            }
        }

        // Supprimer le panier en base de l'utilisateur
        public void Supprimer()
        {
            using (SqlConnection connection = new SqlConnection())
            {
                ConnectionStringSettings connex = ConfigurationManager.ConnectionStrings["ServeurTestUser"];
                connection.ConnectionString = connex.ConnectionString;
                connection.Open();

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText =
                        " DELETE from Panier" +
                        $" WHERE UtilisateurId={UtilisateurId}";

                    command.ExecuteNonQuery();
                }
            }
        }

        //Ajouter dans listeArticles les articles du PanierDAL
        public void Lister()
        {
            using (SqlConnection connection = new SqlConnection())
            {
                ConnectionStringSettings connex = ConfigurationManager.ConnectionStrings["ServeurTestUser"];
                connection.ConnectionString = connex.ConnectionString;
                connection.Open();

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = 
                        " SELECT UtilisateurId, ArticleId, Quantite, PrixTotal, Image, Nom" +
                        " FROM Panier" +
                        " inner join Article on Article.Id = ArticleId"+
                       $" WHERE UtilisateurId = {UtilisateurId}";

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        listeArticles = new List<Article>();
                        while (reader.Read())
                        {
                            Article articleEnCours = new Article();
                            articleEnCours.Id = (int)reader["ArticleId"];
                            articleEnCours.Quantite = (int)reader["Quantite"];
                            double prix = ((double)reader["PrixTotal"]) / articleEnCours.Quantite;
                            articleEnCours.Prix = Math.Round(prix, 2);
                            articleEnCours.Image = (string)reader["Image"];
                            articleEnCours.Nom = (string)reader["Nom"];
                            listeArticles.Add(articleEnCours);
                        }
                    }
                }
            }
        }
    }
}