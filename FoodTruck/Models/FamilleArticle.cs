using OmniFW.Business;
using System;
using System.Collections.Generic;

namespace FoodTruck
{
    public partial class FamilleArticle : Entite
    {
        public FamilleArticle() : base() { }
        public FamilleArticle(int id) : base(id) { }




        [ID]
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
        public string Nom { get; set; }
        public int TvaId { get; set; }

        [ChildId("FamilleId")]
        public CollectionEntite<Article> Articles { get; set; }
    }
}
