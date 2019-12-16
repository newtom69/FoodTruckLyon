using OmniFW.Business;
using System;
using System.Collections.Generic;

namespace FoodTruck
{
    public partial class Panier : Entite
    {
        public Panier() : base() { }

        public int ClientId { get; set; }
        public int ArticleId { get; set; }
        public int Quantite { get; set; }
        public double PrixTotal { get; set; }
    }
}
