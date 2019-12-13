namespace FoodTruck
{
    using OmniFW.Business;
    using System;
    using System.Collections.Generic;
    
    public partial class OubliMotDePasse : Entite
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string CodeVerification { get; set; }
        public DateTime DateFinValidite { get; set; }
    }
}
