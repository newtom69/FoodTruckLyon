using FoodTruck.DAL;
using FoodTruck.ViewModels;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using FoodTruck.Models;

namespace FoodTruck.Controllers
{
    public class ControllerParent : Controller
    {
        protected string ActionNom { get; set; }
        protected string ControllerNom { get; set; }
        protected Utilisateur Utilisateur { get; set; }
        protected string ProspectGuid { get; set; }
        protected PanierViewModel PanierViewModel { get; set; }
        protected bool AdminSuper { get; set; }
        protected bool AdminArticle { get; set; }
        protected bool AdminCommande { get; set; }
        protected bool AdminUtilisateur { get; set; }

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);

            ActionNom = RouteData.Values["action"].ToString();
            ControllerNom = RouteData.Values["controller"].ToString();

            if (Session["UtilisateurId"] != null)
            {
                ViewBag.Utilisateur = new UtilisateurDAL().Details((int)Session["UtilisateurId"]);
                //TODO ajouter ViewBag.Panier ? et les autres ?
            }
            else
            {
                ViewBag.Utilisateur = null;
                //TODO ajouter ViewBag.Panier par prospect
            }

            MettrelUrlEnSession();

            if (Session["UtilisateurId"] == null || (int)Session["UtilisateurId"] == 0)
            {
                HttpCookie cookie = Request.Cookies.Get("GuidClient");
                if (cookie != null)
                {
                    Utilisateur = new UtilisateurDAL().ConnexionCookies(cookie.Value);
                    Session["UtilisateurId"] = Utilisateur.Id;
                    PanierViewModel = new PanierViewModel(); //Todo effacer
                    AgregerPanierEnBase();
                    RecupererPanierEnBase();
                }
                else
                {
                    Session["UtilisateurId"] = 0;
                    Utilisateur = new Utilisateur();
                }
            }
            else
                Utilisateur = new UtilisateurDAL().Details((int)Session["UtilisateurId"]);

            if (Utilisateur.Id != 0)
            {
                DonnerLesDroitsdAcces();
            }
            else
            {
                RetirerLesDroitsdAcces();
                if (Session["ProspectGuid"] != null)
                {
                    ProspectGuid = Session["ProspectGuid"].ToString();
                }
                else
                {
                    HttpCookie cookie = Request.Cookies.Get("Prospect");
                    if (cookie != null)
                    {
                        Session["ProspectGuid"] = ProspectGuid = cookie.Value;
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
                        Session["ProspectGuid"] = guid;
                        cookie = new HttpCookie("Prospect")
                        {
                            Value = guid,
                            Expires = DateTime.Now.AddDays(30)
                        };
                        Response.Cookies.Add(cookie);
                    }
                }
            }

            VisiteDAL.Enregistrer(Utilisateur.Id);

            if (Utilisateur.AdminSuper)
            {
                ViewBag.AdminArticle = true;
                ViewBag.AdminCommande = true;
                ViewBag.AdminUtilisateur = true;
            }
            else
            {
                if (Utilisateur.AdminArticle) ViewBag.AdminArticle = true;
                else ViewBag.AdminArticle = false;
                if (Utilisateur.AdminCommande) ViewBag.AdminCommande = true;
                else ViewBag.AdminCommande = false;
                if (Utilisateur.AdminUtilisateur) ViewBag.AdminUtilisateur = true;
                else ViewBag.AdminUtilisateur = false;
            }

            if (Utilisateur.Id != 0)
            {
                PanierDAL panierDAL = new PanierDAL(Utilisateur.Id);
                PanierViewModel = new PanierViewModel(panierDAL.ListerPanierUtilisateur());
            }
            else
            {
                PanierProspectDAL panierProspectDAL = new PanierProspectDAL(ProspectGuid);
                PanierViewModel = new PanierViewModel(panierProspectDAL.ListerPanierProspect());
            }
            PanierViewModel.Trier();
            ViewBag.Panier = PanierViewModel;
        }

        protected void InitialiserSession()
        {
            RetirerLesDroitsdAcces();
            Session["UtilisateurId"] = 0;
            Utilisateur = new Utilisateur();
            PanierViewModel = new PanierViewModel();
            string guid = Guid.NewGuid().ToString();
            Session["ProspectGuid"] = guid;
            HttpCookie cookie = new HttpCookie("Prospect")
            {
                Value = guid,
                Expires = DateTime.Now.AddDays(30)
            };
            Request.Cookies.Add(cookie);
        }

        private void AgregerPanierEnBase()
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
        protected void RecupererPanierEnBase()
        {
            if (Utilisateur.Id != 0)
                PanierViewModel = new PanierViewModel(new PanierDAL(Utilisateur.Id).ListerPanierUtilisateur());
            else
                PanierViewModel = new PanierViewModel(new PanierProspectDAL(ProspectGuid).ListerPanierProspect());
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
            string controller = Request.RequestContext.RouteData.Values["controller"].ToString();
            string getOrPost = Request.HttpMethod;
            if (controller != "Compte" && getOrPost == "GET")
                Session["Url"] = Request.Url.ToString();
            if (Session["Url"] == null)
                Session["Url"] = "~/";
        }




    }
}