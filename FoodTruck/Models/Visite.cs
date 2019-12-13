using OmniFW.Business;
using System;
using System.Collections.Generic;

namespace FoodTruck
{
    public partial class Visite : Entite
    {
        public int Id { get; set; }
        public string AdresseIp { get; set; }
        public DateTime Date { get; set; }
        public string Url { get; set; }
        public int ClientId { get; set; }
        public string Navigateur { get; set; }
        public bool NavigateurMobile { get; set; }
        public string UrlOrigine { get; set; }
    }
}
