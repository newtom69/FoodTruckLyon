using FoodTruck.DAL;
using FoodTruck.Models;
using FoodTruck.ViewModels;
using System;
using System.Collections.Generic;
using System.Web;
using FoodTruck.Extensions;

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
            if (HttpContext.Current.Session["UtilisateurId"] == null || (int)HttpContext.Current.Session["UtilisateurId"] == 0)
            {
                HttpCookie cookie = HttpContext.Current.Request.Cookies.Get("GuidClient");
                if (cookie != null)
                {
                    Utilisateur = new UtilisateurDAL().ConnexionCookies(cookie.Value);
                    HttpContext.Current.Session["UtilisateurId"] = Utilisateur.Id;
                    PanierViewModel = new PanierViewModel(); //Todo effacer
                    AgregerPanierEnBase();
                    RecupererPanierEnBase();
                }
                else
                {
                    HttpContext.Current.Session["UtilisateurId"] = 0;
                    Utilisateur = new Utilisateur();
                }
            }
            else
                Utilisateur = new UtilisateurDAL().Details((int)HttpContext.Current.Session["UtilisateurId"]);

            if (Utilisateur.Id != 0)
            {
                DonnerLesDroitsdAcces();
            }
            else
            {
                RetirerLesDroitsdAcces();
                if (HttpContext.Current.Session["ProspectGuid"] != null)
                {
                    ProspectGuid = HttpContext.Current.Session["ProspectGuid"].ToString();
                }
                else
                {
                    HttpCookie cookie = HttpContext.Current.Request.Cookies.Get("Prospect");
                    if (cookie != null)
                    {
                        HttpContext.Current.Session["ProspectGuid"] = ProspectGuid = cookie.Value;
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
                        HttpContext.Current.Session["ProspectGuid"] = guid;
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
            HttpContext.Current.Session["UtilisateurId"] = 0;
            Utilisateur = new Utilisateur();
            PanierViewModel = new PanierViewModel();
            string guid = Guid.NewGuid().ToString();
            HttpContext.Current.Session["ProspectGuid"] = guid;
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
                PanierViewModel = new PanierViewModel(lePanierDal.ListerPanierUtilisateur());
            }
            else
            {
                PanierProspectDAL lePanierProspectDal = new PanierProspectDAL(ProspectGuid);
                PanierViewModel = new PanierViewModel(lePanierProspectDal.ListerPanierProspect());
            }
        }

        private void DonnerLesDroitsdAcces()
        {
            //TODO mettre dans Getter 
            if (Utilisateur.AdminSuper) AdminSuper = true;
            if (Utilisateur.AdminArticle) AdminArticle = true;
            if (Utilisateur.AdminCommande) AdminCommande = true;
            if (Utilisateur.AdminUtilisateur) AdminUtilisateur = true;
        }

        private void RetirerLesDroitsdAcces()
        {
            AdminSuper = AdminArticle = AdminCommande = AdminUtilisateur = false;
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