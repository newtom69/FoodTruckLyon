using OmniFW.Business;
using System;

namespace FoodTruck
{
    public partial class PanierProspect : Entite
    {
        public PanierProspect(string prospectGuid) : base()
        {
            ProspectGuid = prospectGuid;
        }

        public PanierProspect() : base()
        {
        }

        [ID]
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        [ParentId("Prospect", "Guid")]
        public string ProspectGuid { get; set; }

        [ParentId("Article", "Id")]
        public int ArticleId { get; set; }
        public int Quantite { get; set; }
        public double PrixTotal { get; set; }
        public DateTime DateAjout { get; set; }
    
    }
}
