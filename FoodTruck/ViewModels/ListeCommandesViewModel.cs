using FoodTruck.DAL;
using System;
using System.Collections.Generic;

namespace FoodTruck.ViewModels
{
    public class ListeCommandesViewModel
    {
        public List<CommandeViewModel> Commandes { get; private set; }

        public ListeCommandesViewModel(List<Commande> commandes, Uri uri)
        {
            Commandes = new List<CommandeViewModel>();
            foreach (Commande commande in commandes)
            {
                Client client = new ClientDAL().Details(commande.ClientId);
                CommandeViewModel commandeVM = new CommandeViewModel(commande, client, uri);
                Commandes.Add(commandeVM);
            }
        }
    }
}