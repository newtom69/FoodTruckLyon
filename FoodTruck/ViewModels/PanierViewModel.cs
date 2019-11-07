using FoodTruck.DAL;
using System.Collections.Generic;
using System.Linq;

namespace FoodTruck.ViewModels
{
    public class PanierViewModel
    {
        public List<ArticleViewModel> ArticlesDetailsViewModel { get; set; }
        public double PrixTotal { get; set; }
        public List<Creneau> Creneaux { get; set; }

        internal PanierViewModel()
        {
            ArticlesDetailsViewModel = new List<ArticleViewModel>();
        }

        internal PanierViewModel(List<Panier> panierUtilisateur)
        {
            ArticlesDetailsViewModel = new List<ArticleViewModel>();
            foreach (Panier panier in panierUtilisateur)
            {
                PrixTotal += panier.PrixTotal;
                ArticlesDetailsViewModel.Add(new ArticleViewModel(new ArticleDAL().Details(panier.ArticleId), panier.Quantite));
            }
        }

        internal PanierViewModel(List<PanierProspect> panierProspect)
        {
            ArticlesDetailsViewModel = new List<ArticleViewModel>();
            foreach (PanierProspect panier in panierProspect)
            {
                PrixTotal += panier.PrixTotal;
                ArticlesDetailsViewModel.Add(new ArticleViewModel(new ArticleDAL().Details(panier.ArticleId), panier.Quantite));
            }
        }

        internal void Initialiser()
        {
            ArticlesDetailsViewModel = new List<ArticleViewModel>();
            Creneaux = new List<Creneau>();
            PrixTotal = 0;
        }

        internal void Trier()
        {
            ArticlesDetailsViewModel = ArticlesDetailsViewModel.OrderBy(x => x.Article.FamilleId).ThenBy(x => x.Article.Nom).ToList();
        }
    }
}