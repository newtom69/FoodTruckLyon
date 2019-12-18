using OmniFW.Business;
using OmniFW.Data;
using System;
using System.Collections.Generic;

namespace FoodTruck
{
    public partial class Panier : Entite
    {
        public Panier() : base() { }

        [ParentId("Client", "Id")]
        public int ClientId { get; set; }

        [ParentId("Article", "Id")]
        public int ArticleId { get; set; }

        public int Quantite { get; set; }
        public double PrixTotal { get; set; }
    }
}
