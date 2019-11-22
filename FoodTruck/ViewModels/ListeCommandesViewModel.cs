using FoodTruck.DAL;
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
                Client utilisateur = new ClientDAL().Details(commande.ClientId);
                CommandeViewModel commandeVM = new CommandeViewModel()
                {
                    Commande = commande,
                    Client = utilisateur,
                    ListArticlesVM = new CommandeDAL().Articles(commande.Id),
                };
                Commandes.Add(commandeVM);
            }
        }
    }
}