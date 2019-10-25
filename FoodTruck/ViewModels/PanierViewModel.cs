using FoodTruck.DAL;
using FoodTruck.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FoodTruck.ViewModels
{
    public class PanierViewModel
    {
        public List<ArticleViewModel> ArticlesDetailsViewModel { get; set; }
        public double PrixTotal { get; set; }
        public List<DateTime> DatesRetraitPossibles { get; set; }

        public PanierViewModel()
        {
            ArticlesDetailsViewModel = new List<ArticleViewModel>();
        }

        public PanierViewModel(List<Panier> panierUtilisateur)
        {
            ArticlesDetailsViewModel = new List<ArticleViewModel>();
            foreach (Panier lePanier in panierUtilisateur)
            {
                PrixTotal += lePanier.PrixTotal;
                ArticleViewModel article = ArticlesDetailsViewModel.Find(art => art.Article.Id == lePanier.ArticleId);
                ArticleDAL articleDAL = new ArticleDAL();
                ArticlesDetailsViewModel.Add(new ArticleViewModel(articleDAL.Details(lePanier.ArticleId), lePanier.Quantite));
            }
        }

        public PanierViewModel(List<PanierProspect> panierProspect)
        {
            ArticlesDetailsViewModel = new List<ArticleViewModel>();
            foreach (PanierProspect lePanier in panierProspect)
            {
                PrixTotal += lePanier.PrixTotal;
                ArticleViewModel article = ArticlesDetailsViewModel.Find(art => art.Article.Id == lePanier.ArticleId);
                ArticleDAL articleDAL = new ArticleDAL();
                ArticlesDetailsViewModel.Add(new ArticleViewModel(articleDAL.Details(lePanier.ArticleId), lePanier.Quantite));
            }
        }


        public void Trier()
        {
            ArticlesDetailsViewModel = ArticlesDetailsViewModel.OrderBy(x => x.Article.FamilleId).ThenBy(x => x.Article.Nom).ToList();
        }
    }
}