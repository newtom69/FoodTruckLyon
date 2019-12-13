using OmniFW.Business;
using System;
using System.Collections.Generic;

namespace FoodTruck
{
    public partial class Tva : Entite
    {
        public int Id { get; set; }
        public double Taux { get; set; }
        public string Libelle { get; set; }
    }
}
