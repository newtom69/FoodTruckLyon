using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FoodTruck.DAL;
using FoodTruck.Models;

namespace FoodTruck.ViewModels
{
    public class AdministrationViewModel
    {
        public List<CommandeViewModel> CommandesEnCours { get; internal set; }

        public AdministrationViewModel(List<Commande> commandes)
        {
            CommandesEnCours = new List<CommandeViewModel>();
            foreach (Commande commande in commandes)
            {
                Utilisateur utilisateur = new UtilisateurDAL().Details(commande.UtilisateurId);
                CommandeViewModel commandeVM = new CommandeViewModel()
                {
                    Commande = commande,
                    Utilisateur = utilisateur,
                    ListArticles = new CommandeDAL().ListerArticles(commande.Id),
                };


                CommandesEnCours.Add(commandeVM);
            }
        }


    }
}