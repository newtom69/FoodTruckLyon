using OmniFW.Business;

namespace FoodTruck
{
    public partial class Commande_Article : Entite
    {
        public int CommandeId { get; set; }
        public int ArticleId { get; set; }
        public int Quantite { get; set; }
        public double PrixTotalHT { get; set; }
        public double PrixTotalTTC { get; set; }
    
    }
}
