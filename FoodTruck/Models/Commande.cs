using OmniFW.Business;
using System;

namespace FoodTruck
{
    public partial class Commande : Entite
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public DateTime DateCommande { get; set; }
        public DateTime DateRetrait { get; set; }
        public double RemiseFidelite { get; set; }
        public double RemiseCommerciale { get; set; }
        public bool Retrait { get; set; }
        public bool Annulation { get; set; }
        public double PrixTotalHT { get; set; }
        public double PrixTotalTTC { get; set; }
    }
}
