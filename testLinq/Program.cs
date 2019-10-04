using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using testLinq.DAL;
using testLinq.Models;

namespace testLinq
{
    class Program
    {
        static void Main(string[] args)
        {



            //Commande laCommande = new Commande();
            //laCommande.UtilisateurId = 1;
            //laCommande.DateCommande = DateTime.Now;
            //laCommande.DateLivraison = DateTime.Now;
            //laCommande.ModeLivraison = "Sur place";
            //laCommande.PrixTotal = 47.25;

            //CommandeDAL cmdDAL = new CommandeDAL();
            //cmdDAL.Ajouter(laCommande);

            int utilisateurId = 37;
            ArticleDAL articleDAL = new ArticleDAL();
            Article larticle = articleDAL.Details(1);
            PanierDAL panierDAL = new PanierDAL(utilisateurId);

            panierDAL.Lister();
            panierDAL.ModifierQuantite(larticle, 1);
            panierDAL.Supprimer(larticle);
            panierDAL.Ajouter(larticle);
            panierDAL.Supprimer();


        }


    }
}

