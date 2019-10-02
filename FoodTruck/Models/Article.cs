using System.Configuration;
using System.Data.SqlClient;

namespace FoodTruck
{
    public class Article
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public string Image { get; set; }
        public double Prix { get; set; }
        public int FamilleId { get; set; }
        public int NombreVendus { get; set; }
        public string Description { get; set; }
        public string Allergenes { get; set; }
        public int Grammage { get; set; }
        public int Litrage { get; set; }
        public int Quantite { get; set; }
    }
}