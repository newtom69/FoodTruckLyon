using OmniFW.Business;
using System;
using System.Collections.Generic;

namespace FoodTruck
{
    public partial class Facture : Entite
    {
        public int Id { get; set; }
        public int CommandeId { get; set; }
        public string Guid { get; set; }
    
        public virtual Commande Commande { get; set; }
    }
}
