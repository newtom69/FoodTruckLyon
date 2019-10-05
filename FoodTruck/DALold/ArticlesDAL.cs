using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using FoodTruck.Models;

namespace FoodTruck.DAL2
{
    public class ArticlesDAL
    {
        public List<Article> listeArticles { get; set; }

        //Random de nombreRetour des nombreTop articles les plus vendus
        public void ListerRandom(int nombreRetour, int nombreTop)
        {
            using (SqlConnection connection = new SqlConnection())
            {
                ConnectionStringSettings connex = ConfigurationManager.ConnectionStrings["ServeurTestUser"];
                connection.ConnectionString = connex.ConnectionString;

                connection.Open();

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText =
                       $"select top {nombreRetour} Id, Nom, Image, Description, Prix, Grammage, Litrage " +
                        "from Article " +
                       $"where DansCarte='true' and Id in(select top { nombreTop} Id from Article where familleId<=3 order by NombreVendus desc) " +
                        "order by newid()";

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        listeArticles = new List<Article>();
                        while (reader.Read())
                        {
                            Article articleEnCours = new Article
                            {
                                Id = (int)reader["Id"],
                                Nom = reader["Nom"].ToString(),
                                Image = reader["Image"].ToString(),
                                Description = reader["Description"].ToString(),
                                Prix = (double)reader["Prix"],
                                Grammage = (int)reader["Grammage"],
                                Litrage = (int)reader["Litrage"]
                            };
                            listeArticles.Add(articleEnCours);
                        }
                    }
                }
            }
        }

        //Lister les nombreMax articles de la familleId et ordonner par Nom
        public void Lister(int nombreMax, int familleId)
        {
            if (nombreMax == 0) nombreMax = 200;

            using (SqlConnection connection = new SqlConnection())
            {
                ConnectionStringSettings connex = ConfigurationManager.ConnectionStrings["ServeurTestUser"];
                connection.ConnectionString = connex.ConnectionString;
                connection.Open();

                using (SqlCommand command = connection.CreateCommand())
                {
                        if (familleId == 0)
                            command.CommandText =
                               $" SELECT TOP {nombreMax} Id, Nom, Image, Description, Prix, Grammage, Litrage" +
                                " FROM Article" +
                                " WHERE DansCarte = 'true'"+
                                " ORDER BY Nom";
                        else
                            command.CommandText =
                                $" SELECT TOP {nombreMax} Id, Nom, Image, Description, Prix, Grammage, Litrage" +
                                 " FROM Article" +
                                $" WHERE DansCarte='true' and FamilleId = {familleId}" +
                                 " ORDER BY Nom";
                    
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        listeArticles = new List<Article>();
                        while (reader.Read())
                        {
                            Article articleEnCours = new Article
                            {
                                Id = (int)reader["Id"],
                                Nom = reader["Nom"].ToString(),
                                Image = reader["Image"].ToString(),
                                Description = reader["Description"].ToString(),
                                Prix = (double)reader["Prix"],
                                Grammage = (int)reader["Grammage"],
                                Litrage = (int)reader["Litrage"]
                            };
                            listeArticles.Add(articleEnCours);
                        }
                    }
                }
            }
        }
        public void Lister(int nombreMax)
        {
            Lister(nombreMax, 0);
        }
        public void Lister()
        {
            Lister(0, 0);
        }

    }
}