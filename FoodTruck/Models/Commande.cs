using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;

namespace FoodTruck.Models
{
    public class Commande
    {
        public int Id { get; set; }
        public int UtilisateurId { get; set; }
        public DateTime DateCommande { get; set; }
        public DateTime DateLivraison { get; set; }
        public string ModeLivraison { get; set; }
        public double PrixTotal { get; set; }
        public List<Article> listeArticles { get; set; }
    }
}