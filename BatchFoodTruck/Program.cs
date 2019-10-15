using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FoodTruck.DAL;

namespace BatchFoodTruck
{
    class Program
    {
        static void Main(string[] args)
        {

            // Purge de la table PanierProspect
            PanierProspectDAL panierProspectDAL = new PanierProspectDAL("");
            int nb = panierProspectDAL.Purger(1);
            Console.WriteLine("Nombre d'enregistrement de PanierProspect supprimés : " + nb);


        }
    }
}
