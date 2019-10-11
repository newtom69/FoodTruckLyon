using FoodTruck.DAL;
using FoodTruck.Models;
using FoodTruck.ViewModels;
using System;
using System.Web;
using System.Web.Mvc;

namespace FoodTruck.Controllers
{
    public class SessionVariables
    {
        public Utilisateur Utilisateur { get; set; }
        public PanierViewModel PanierViewModel { get; set; }

        public SessionVariables()
        {
            if (HttpContext.Current.Session["Panier"] == null)
                HttpContext.Current.Session["Panier"] = PanierViewModel = new PanierViewModel();
            else
            {
                PanierViewModel = (PanierViewModel)HttpContext.Current.Session["Panier"];
                PanierViewModel.Trier();
            }

            if (HttpContext.Current.Session["Utilisateur"] == null)
            {
                HttpCookie cookie = HttpContext.Current.Request.Cookies.Get("Email");
                if (cookie != null)
                {
                    UtilisateurDAL utilisateurDAL = new UtilisateurDAL();
                    HttpContext.Current.Session["Utilisateur"] = Utilisateur = utilisateurDAL.ConnexionCookies(cookie.Value);
                    SynchroniserPanier();
                }
                else
                    HttpContext.Current.Session["Utilisateur"] = Utilisateur = new Utilisateur();
            }
            else
                Utilisateur = (Utilisateur)HttpContext.Current.Session["Utilisateur"];
        }
        public SessionVariables(int pourSurchargeUniquement)
        {
            HttpContext.Current.Session["Utilisateur"] = Utilisateur = new Utilisateur();
            HttpContext.Current.Session["Panier"] = PanierViewModel = new PanierViewModel();
        }

        public void SynchroniserPanier()
        {
            if (Utilisateur != null && Utilisateur.Id != 0)
            {
                PanierDAL lePanierDal = new PanierDAL(Utilisateur.Id);
                foreach (ArticleDetailsViewModel lArticle in PanierViewModel.ArticlesDetailsViewModel)
                {
                    Panier panier = lePanierDal.ListerPanierUtilisateur().Find(pan => pan.ArticleId == lArticle.Article.Id);
                    if (panier == null)
                        lePanierDal.Ajouter(lArticle.Article, lArticle.Quantite);
                    else
                        lePanierDal.ModifierQuantite(lArticle.Article, lArticle.Quantite);
                }
                PanierViewModel = new PanierViewModel();
                foreach (Panier lePanier in lePanierDal.ListerPanierUtilisateur())
                {
                    PanierViewModel.PrixTotal += lePanier.PrixTotal;
                    ArticleDetailsViewModel article = PanierViewModel.ArticlesDetailsViewModel.Find(art => art.Article.Id == lePanier.ArticleId);
                    ArticleDAL articleDAL = new ArticleDAL();
                    PanierViewModel.ArticlesDetailsViewModel.Add(new ArticleDetailsViewModel(articleDAL.Details(lePanier.ArticleId), lePanier.Quantite));
                    HttpContext.Current.Session["Panier"] = PanierViewModel;
                }
            }
        }

    }
}