﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class FoodTruckEntities : DbContext
    {
        public FoodTruckEntities()
            : base("name=FoodTruckEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Article> Article { get; set; }
        public virtual DbSet<Commande> Commande { get; set; }
        public virtual DbSet<Commande_Article> Commande_Article { get; set; }
        public virtual DbSet<FamilleArticle> FamilleArticle { get; set; }
        public virtual DbSet<JourExceptionnel> JourExceptionnel { get; set; }
        public virtual DbSet<Panier> Panier { get; set; }
        public virtual DbSet<PanierProspect> PanierProspect { get; set; }
        public virtual DbSet<PlageRepas> PlageRepas { get; set; }
        public virtual DbSet<Utilisateur> Utilisateur { get; set; }
        public virtual DbSet<Visite> Visite { get; set; }
    }
}
