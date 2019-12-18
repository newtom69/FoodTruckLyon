using OmniFW.Business;
using System;

namespace FoodTruck
{
    public partial class OuvertureHebdomadaire : Entite
    {
        public OuvertureHebdomadaire() : base() { }
        public OuvertureHebdomadaire(int id) : base(id) { }

        [ID]
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
        public int JourSemaineId { get; set; }
        public TimeSpan Debut { get; set; }
        public TimeSpan Fin { get; set; }
    }
}
