using FoodTruck.DAL;
using System;
using System.Collections.Generic;

namespace FoodTruck.ViewModels
{
    public class ListeCommandesViewModel
    {
        public List<CommandeViewModel> Commandes { get; private set; }

        public ListeCommandesViewModel(List<Commande> commandes)
        {
            Commandes = new List<CommandeViewModel>();
            foreach (Commande commande in commandes)
            {
                Client client = new ClientDAL().Details(commande.ClientId);
                CommandeViewModel commandeVM = new CommandeViewModel(commande, client);
                Commandes.Add(commandeVM);
            }
        }
    }
}