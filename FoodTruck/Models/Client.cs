using OmniFW.Business;
using System;
using System.Collections.Generic;

namespace FoodTruck
{
    public partial class Client : Entite
    {
        public int Id { get; set; }
        public string Guid { get; set; }
        public string Email { get; set; }
        public string Mdp { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Telephone { get; set; }
        public int Cagnotte { get; set; }
        public bool AdminCommande { get; set; }
        public bool AdminArticle { get; set; }
        public bool AdminPlanning { get; set; }
        public System.DateTime Inscription { get; set; }
        public string Login { get; set; }
        public bool AdminClient { get; set; }
        }
}
