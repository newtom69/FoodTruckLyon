using OmniFW.Business;
using System;

namespace FoodTruck
{
    public partial class CodePromo : Entite
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public double Remise { get; set; }
        public double? MontantMinimumCommande { get; set; }
        public DateTime? DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
    }
}
