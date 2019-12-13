using OmniFW.Business;
using System;
using System.Collections.Generic;

namespace FoodTruck
{
    public partial class OuvertureHebdomadaire : Entite
    {
        public int Id { get; set; }
        public int JourSemaineId { get; set; }
        public TimeSpan Debut { get; set; }
        public TimeSpan Fin { get; set; }
    }
}
