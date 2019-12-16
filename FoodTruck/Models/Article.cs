using OmniFW.Business;

namespace FoodTruck
{
    public partial class Article : Entite
    {
        public Article() : base() { }

        public Article(int id) : base(id) { }

        public Article(string nom) : base()
        {
            Id = OmniFW.Outils.Trans.NullToInt(GetIdByColonne("Nom", nom));
        }

        [ID]
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
        public string Nom { get; set; }
        public string Image { get; set; }

        [ParentId("FamilleArticle", "Id")]
        public int FamilleId { get; set; }
        public int NombreVendus { get; set; }
        public string Description { get; set; }
        public string Allergenes { get; set; }
        public int Grammage { get; set; }
        public int Litrage { get; set; }
        public bool DansCarte { get; set; }
        public double PrixHT { get; set; }
        public double PrixTTC { get; set; }

        //TODO VOIR AVEC NICO
        //[ChildId("ArticleId")]
        //public CollectionEntite<PanierProspect> PaniersProspect { get; set; }

    }
}
