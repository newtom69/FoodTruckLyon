
namespace FoodTruck
{
    using OmniFW.Business;
    using System;
    using System.Collections.Generic;
    
    public partial class JourExceptionnel : Entite
    {
        public int Id { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }
        public bool Ouvert { get; set; }
    }
}
