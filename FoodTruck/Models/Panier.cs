//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré à partir d'un modèle.
//
//     Des modifications manuelles apportées à ce fichier peuvent conduire à un comportement inattendu de votre application.
//     Les modifications manuelles apportées à ce fichier sont remplacées si le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FoodTruck.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Panier
    {
        public int ClientId { get; set; }
        public int ArticleId { get; set; }
        public int Quantite { get; set; }
        public double PrixTotal { get; set; }
    
        public virtual Article Article { get; set; }
        public virtual Client Client { get; set; }
    }
}
