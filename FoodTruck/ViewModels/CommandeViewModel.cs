using System.Collections.Generic;

namespace FoodTruck.ViewModels
{
    public class CommandeViewModel
    {
        public Commande Commande { get; set; }
        public Client Client { get; set; }
        public List<ArticleViewModel> ListArticlesVM { get; set; }

        public CommandeViewModel(Commande commande, Client utilisateur)
        {
            Commande = commande;
            Client = utilisateur;
        }
        public CommandeViewModel()
        {
        }
    }
}