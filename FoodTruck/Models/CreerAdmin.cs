using OmniFW.Business;
using System;
using System.Collections.Generic;

namespace FoodTruck 
{
    public partial class CreerAdmin : Entite
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string CodeVerification { get; set; }
        public System.DateTime DateFinValidite { get; set; }
    }
}
