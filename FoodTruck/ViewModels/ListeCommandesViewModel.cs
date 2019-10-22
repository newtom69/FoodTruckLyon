using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FoodTruck.DAL;
using FoodTruck.Models;

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
                Utilisateur utilisateur = new UtilisateurDAL().Details(commande.UtilisateurId);
                CommandeViewModel commandeVM = new CommandeViewModel()
                {
                    Commande = commande,
                    Utilisateur = utilisateur,
                    ListArticlesVM = new CommandeDAL().ListerArticles(commande.Id),
                };
                Commandes.Add(commandeVM);
            }
        }
    }
}