//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré à partir d'un modèle.
//
//     Des modifications manuelles apportées à ce fichier peuvent conduire à un comportement inattendu de votre application.
//     Les modifications manuelles apportées à ce fichier sont remplacées si le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FoodTruck
{
    using System;
    using System.Collections.Generic;
    
    public partial class Facture
    {
        public int Id { get; set; }
        public int CommandeId { get; set; }
        public string Guid { get; set; }
    
        public virtual Commande Commande { get; set; }
    }
}
