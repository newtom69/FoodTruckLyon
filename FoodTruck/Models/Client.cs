using OmniFW.Business;
using OmniFW.Outils;
using System;
using System.Collections.Generic;

namespace FoodTruck
{
    public partial class Client : Entite
    {
        public Client() : base() { }
        public Client(int id) : base(id) { }

        public Client(string loginEmail) : base()
        {
            Id = Trans.NullToInt(GetIdByColonne("Email", loginEmail));
            if (Id == -1 )
                Id = Trans.NullToInt(GetIdByColonne("Login", loginEmail));
        }

        [ID]
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
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
        public DateTime Inscription { get; set; }
        public string Login { get; set; }
        public bool AdminClient { get; set; }
        }
}
