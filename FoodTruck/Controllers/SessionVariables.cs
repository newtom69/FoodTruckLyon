﻿using FoodTruck.DAL;
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
        public bool AdminSuper { get; set; }
        public bool AdminArticle { get; set; }
        public bool AdminCommande { get; set; }
        public bool AdminUtilisateur { get; set; }


        public SessionVariables()
        {
            MettrelUrlEnSession();
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

            if (Utilisateur.Id != 0)
            {
                DonnerLesDroitsdAcces();
            }
            else
            {
                RetirerLesDroitsdAcces();
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
                        List<PanierProspect> paniers = panierProspectDAL.ListerPanierProspect();
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
            RetirerLesDroitsdAcces();
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
                foreach (ArticleViewModel lArticle in PanierViewModel.ArticlesDetailsViewModel)
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
                    ArticleViewModel article = PanierViewModel.ArticlesDetailsViewModel.Find(art => art.Article.Id == lePanier.ArticleId);
                    ArticleDAL articleDAL = new ArticleDAL();
                    PanierViewModel.ArticlesDetailsViewModel.Add(new ArticleViewModel(articleDAL.Details(lePanier.ArticleId), lePanier.Quantite));
                    HttpContext.Current.Session["Panier"] = PanierViewModel;
                }
            }
            else
            {
                PanierProspectDAL lePanierProspectDal = new PanierProspectDAL(ProspectGuid); //todo
                PanierViewModel = new PanierViewModel();
                foreach (PanierProspect lePanier in lePanierProspectDal.ListerPanierProspect())
                {
                    PanierViewModel.PrixTotal += lePanier.PrixTotal;
                    ArticleViewModel article = PanierViewModel.ArticlesDetailsViewModel.Find(art => art.Article.Id == lePanier.ArticleId);
                    ArticleDAL articleDAL = new ArticleDAL();
                    PanierViewModel.ArticlesDetailsViewModel.Add(new ArticleViewModel(articleDAL.Details(lePanier.ArticleId), lePanier.Quantite));
                    HttpContext.Current.Session["Panier"] = PanierViewModel;
                }
            }
        }

        private void DonnerLesDroitsdAcces()
        {
            if (Utilisateur.AdminSuper)
                HttpContext.Current.Session["AdminSuper"] = AdminSuper = true;
            if (Utilisateur.AdminArticle)
                HttpContext.Current.Session["AdminArticle"] = AdminArticle = true;
            if (Utilisateur.AdminCommande)
                HttpContext.Current.Session["AdminCommande"] = AdminCommande = true;
            if (Utilisateur.AdminUtilisateur)
                HttpContext.Current.Session["AdminUtilisateur"] = AdminUtilisateur = true;
        }

        private void RetirerLesDroitsdAcces()
        {
            HttpContext.Current.Session["AdminSuper"] = AdminSuper = false;
            HttpContext.Current.Session["AdminArticle"] = AdminArticle = false;
            HttpContext.Current.Session["AdminCommande"] = AdminCommande = false;
            HttpContext.Current.Session["AdminUtilisateur"] = AdminUtilisateur = false;
        }
        private void MettrelUrlEnSession()
        {
            string controller = HttpContext.Current.Request.RequestContext.RouteData.Values["controller"].ToString();
            string getOrPost = HttpContext.Current.Request.HttpMethod;
            if (controller != "Compte" && getOrPost == "GET")
                HttpContext.Current.Session["Url"] = HttpContext.Current.Request.Url.ToString();
            if (HttpContext.Current.Session["Url"] == null)
                HttpContext.Current.Session["Url"] = "~/";
        }
    }

}