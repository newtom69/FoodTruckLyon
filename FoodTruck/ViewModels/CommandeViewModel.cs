using FoodTruck.Extensions;
using FoodTruck.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoodTruck.ViewModels
{
    public class CommandeViewModel
    {
        public Commande Commande { get; set; }
        public Utilisateur Utilisateur { get; set; }
        public List<Article> ListArticles { get; set; }

        public CommandeViewModel(Commande commande, Utilisateur utilisateur)
        {
            Commande = commande;
            Utilisateur = utilisateur;
        }
        public CommandeViewModel()
        {
        }
    }
}