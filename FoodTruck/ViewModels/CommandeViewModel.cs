using System.Collections.Generic;

namespace FoodTruck.ViewModels
{
    public class CommandeViewModel
    {
        public Commande Commande { get; set; }
        public Utilisateur Utilisateur { get; set; }
        public List<ArticleViewModel> ListArticlesVM { get; set; }

        public CommandeViewModel(Commande commande, Utilisateur utilisateur)
        {
            Commande = commande;
            Utilisateur = utilisateur;
        }
        public CommandeViewModel()
        {
        }
    }
}