using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using FoodTruck.Models;

namespace FoodTruck.DAL2
{
    public class VisiteDAL
    {
        public VisiteDAL(Visite laVisite)
        {
            using (SqlConnection connection = new SqlConnection())
            {
                ConnectionStringSettings connex = ConfigurationManager.ConnectionStrings["ServeurTestUser"];
                connection.ConnectionString = connex.ConnectionString;
                connection.Open();

                using (SqlCommand command = connection.CreateCommand())
                {
                    string stringDateVisite = laVisite.DateTimeVisite.ToString("yyyy-MM-dd HH:mm:ss");

                    if (laVisite.UtilisateurId != 0)
                        command.CommandText = $"INSERT INTO Visite (Url, AdresseIp, UtilisateurId, UrlOrigine, Navigateur, DateTimeVisite)" +
                                              $" VALUES('{laVisite.Url}', '{laVisite.AdresseIp}',{laVisite.UtilisateurId}, '{laVisite.UrlOrigine}', '{laVisite.Navigateur}', '{stringDateVisite}')"; 
                    else
                        command.CommandText = $"INSERT INTO Visite (Url, AdresseIp, UrlOrigine, Navigateur, DateTimeVisite)" +
                                              $" VALUES('{laVisite.Url}', '{laVisite.AdresseIp}','{laVisite.UrlOrigine}', '{laVisite.Navigateur}', '{stringDateVisite}')";

                    command.ExecuteNonQuery();
                }
            }
        }
    }
}