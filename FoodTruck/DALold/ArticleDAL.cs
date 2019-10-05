using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace FoodTruck.DAL2
{
	public class ArticleDAL
	{
	    public Article Details(int id)
	    {
			Article lArticle = new Article();
	        using (SqlConnection connection = new SqlConnection())
	        {
	            ConnectionStringSettings connex = ConfigurationManager.ConnectionStrings["ServeurTestUser"];
	            connection.ConnectionString = connex.ConnectionString;
	            connection.Open();

	            using (SqlCommand command = connection.CreateCommand())
	            {
	                command.CommandText =
	                    " SELECT Id, Nom, Image, Prix, Description, Allergenes, Grammage, Litrage" +
	                    " FROM Article" +
	                    $" WHERE Id={id}";

	                using (SqlDataReader reader = command.ExecuteReader())
	                {
	                    while (reader.Read())
	                    {
	                        lArticle.Id = (int)reader["Id"];
	                        lArticle.Nom = reader["nom"].ToString();
	                        lArticle.Image = reader["Image"].ToString();
	                        lArticle.Prix = (double)reader["Prix"];
	                        lArticle.Description = reader["Description"].ToString();
	                        lArticle.Allergenes = reader["Allergenes"].ToString();
	                        lArticle.Grammage = (int)reader["Grammage"];
	                        lArticle.Litrage = (int)reader["Litrage"];
	                    }
	                    return lArticle;
	                }
	            }
	        }
	    }


	    public void AugmenterQuantiteVendue(int id, int nbre)
	    {
	        Article lArticle = new Article();
	        using (SqlConnection connection = new SqlConnection())
	        {
	            ConnectionStringSettings connex = ConfigurationManager.ConnectionStrings["ServeurTestUser"];
	            connection.ConnectionString = connex.ConnectionString;
	            connection.Open();

	            using (SqlCommand command = connection.CreateCommand())
	            {
	                command.CommandText = $"update Article set NombreVendus = NombreVendus + {nbre} where Id = {id}";
                    command.ExecuteNonQuery();
	            }
	        }
	    }

    }
}