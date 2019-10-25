using FoodTruck.DAL;
using System;

namespace BatchFoodTruck
{
    class Program
    {
        static void Main(string[] args)
        {
            // Purge de la table PanierProspect
            PanierProspectDAL panierProspectDAL = new PanierProspectDAL("");
            int nb = panierProspectDAL.Purger(30);
            Console.WriteLine("Nombre d'enregistrement de PanierProspect supprimés : " + nb);


        }
    }
}
