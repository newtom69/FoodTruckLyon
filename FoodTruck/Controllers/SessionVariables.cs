using FoodTruck.DAL;
using FoodTruck.Models;
using FoodTruck.ViewModels;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace FoodTruck.Controllers
{
    public class SessionVariables
    {
        public Utilisateur Utilisateur { get; set; }
        public string ProspectGuid { get; set; }
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
                HttpCookie cookie = HttpContext.Current.Request.Cookies.Get("GuidClient");
                if (cookie != null)
                {
                    UtilisateurDAL utilisateurDAL = new UtilisateurDAL();
                    HttpContext.Current.Session["Utilisateur"] = Utilisateur = utilisateurDAL.ConnexionCookies(cookie.Value);
                    AgregerPanierEnBase();
                    RecupererPanierEnBase();
                }
                else
                    HttpContext.Current.Session["Utilisateur"] = Utilisateur = new Utilisateur();
            }
            else
                Utilisateur = (Utilisateur)HttpContext.Current.Session["Utilisateur"];

            if (Utilisateur.Id == 0)
            {
                if (HttpContext.Current.Session["Prospect"] != null)
                {
                    ProspectGuid = HttpContext.Current.Session["Prospect"].ToString();
                }
                else
                { 
                    HttpCookie cookie = HttpContext.Current.Request.Cookies.Get("Prospect");
                    if (cookie != null)
                    {
                        HttpContext.Current.Session["Prospect"] = ProspectGuid = cookie.Value;
                        PanierProspectDAL panierProspectDAL = new PanierProspectDAL(ProspectGuid);
                        List<PanierProspect> paniers = panierProspectDAL.ListerPanierUtilisateur();
                        if (paniers.Count > 0)
                        {
                            RecupererPanierEnBase();
                        }
                    }
                    else
                    {
                        string guid = Guid.NewGuid().ToString();
                        HttpContext.Current.Session["Prospect"] = guid;
                        cookie = new HttpCookie("Prospect")
                        {
                            Value = guid,
                            Expires = DateTime.Now.AddDays(30)
                        };
                        HttpContext.Current.Response.Cookies.Add(cookie);
                    }
                }
            }
        }
        public SessionVariables(int pourSurchargeUniquement)
        {
            HttpContext.Current.Session["Utilisateur"] = Utilisateur = new Utilisateur();
            HttpContext.Current.Session["Panier"] = PanierViewModel = new PanierViewModel();
            string guid = Guid.NewGuid().ToString();
            HttpContext.Current.Session["Prospect"] = guid;
            HttpCookie cookie = new HttpCookie("Prospect")
            {
                Value = guid,
                Expires = DateTime.Now.AddDays(30)
            };
            HttpContext.Current.Request.Cookies.Add(cookie);
        }

        public void AgregerPanierEnBase()
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
            }
        }
        public void RecupererPanierEnBase()
        {
            if (Utilisateur.Id != 0)
            {
                PanierDAL lePanierDal = new PanierDAL(Utilisateur.Id);
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
            else
            {
                PanierProspectDAL lePanierProspectDal = new PanierProspectDAL(ProspectGuid); //todo
                PanierViewModel = new PanierViewModel();
                foreach (PanierProspect lePanier in lePanierProspectDal.ListerPanierUtilisateur())
                {
                    PanierViewModel.PrixTotal += lePanier.PrixTotal;
                    ArticleDetailsViewModel article = PanierViewModel.ArticlesDetailsViewModel.Find(art => art.Article.Id == lePanier.ArticleId);
                    ArticleDAL articleDAL = new ArticleDAL();
                    PanierViewModel.ArticlesDetailsViewModel.Add(new ArticleDetailsViewModel(articleDAL.Details(lePanier.ArticleId), lePanier.Quantite));
                    HttpContext.Current.Session["Panier"] = PanierViewModel;
                }

            }
        }

        public bool VerifierDroit()
        {
            if (Utilisateur.AdminArticle || Utilisateur.AdminTotal)
                return true;
            else
#if DEBUG
                return true;
#endif
            return false;
        }
    }
}