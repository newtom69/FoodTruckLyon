using FoodTruck.DAL;
using FoodTruck.ViewModels;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace FoodTruck.Controllers
{
    public class ControllerParent : Controller
    {
        protected string ActionNom { get; set; }
        protected string ControllerNom { get; set; }
        protected Utilisateur Utilisateur { get; set; }
        protected string ProspectGuid { get; set; }
        protected PanierViewModel PanierViewModel { get; set; } 
        protected bool AdminArticle { get; set; }
        protected bool AdminCommande { get; set; }
        protected bool AdminUtilisateur { get; set; }
        protected bool AdminPlanning { get; set; }

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);

            ActionNom = RouteData.Values["action"].ToString();
            ControllerNom = RouteData.Values["controller"].ToString();
            MettrelUrlEnSession();

            if (Session["UtilisateurId"] == null || (int)Session["UtilisateurId"] == 0)
            {
                HttpCookie cookie = Request.Cookies.Get("GuidClient");
                if (cookie != null)
                {
                    ViewBag.Utilisateur = Utilisateur = new UtilisateurDAL().ConnexionCookies(cookie.Value);
                    Session["UtilisateurId"] = Utilisateur.Id;
                    PanierViewModel = new PanierViewModel(); //Todo effacer
                    AgregerPanierEnBase();
                    RecupererPanierEnBase();
                }
                else
                {
                    Session["UtilisateurId"] = 0;
                    ViewBag.Utilisateur = Utilisateur = new Utilisateur();
                }
            }
            else
                ViewBag.Utilisateur = Utilisateur = new UtilisateurDAL().Details((int)Session["UtilisateurId"]);

            VisiteDAL.Enregistrer(Utilisateur.Id);
            if (Utilisateur.Id != 0)
            {
                DonnerLesDroitsdAcces();
                PanierViewModel = new PanierViewModel(new PanierDAL(Utilisateur.Id).ListerPanierUtilisateur());
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
                        List<PanierProspect> paniers = new PanierProspectDAL(ProspectGuid).ListerPanierProspect();
                        if (paniers.Count > 0)
                        {
                            RecupererPanierEnBase();
                        }
                    }
                    else
                    {
                        string guid = Guid.NewGuid().ToString();
                        Session["ProspectGuid"] = ProspectGuid = guid;
                        cookie = new HttpCookie("Prospect")
                        {
                            Value = guid,
                            Expires = DateTime.Now.AddDays(30)
                        };
                        Response.Cookies.Add(cookie);
                    }
                }
                PanierViewModel = new PanierViewModel(new PanierProspectDAL(ProspectGuid).ListerPanierProspect());
            }
            PanierViewModel.Trier();
            ViewBag.Panier = PanierViewModel;

            if (Utilisateur.AdminArticle || Utilisateur.AdminCommande || Utilisateur.AdminUtilisateur || Utilisateur.AdminPlanning)
                ViewBag.MenuAdmin = true;
            if (Utilisateur.AdminArticle)
                ViewBag.AdminArticle = true;
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
                foreach (ArticleViewModel article in PanierViewModel.ArticlesDetailsViewModel)
                {
                    Panier panier = lePanierDal.ListerPanierUtilisateur().Find(pan => pan.ArticleId == article.Article.Id);
                    if (panier == null)
                        lePanierDal.Ajouter(article.Article, article.Quantite);
                    else
                        lePanierDal.ModifierQuantite(article.Article, article.Quantite);
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
            if (Utilisateur.AdminArticle) AdminArticle = true;
            if (Utilisateur.AdminCommande) AdminCommande = true;
            if (Utilisateur.AdminUtilisateur) AdminUtilisateur = true;
            if (Utilisateur.AdminPlanning) AdminPlanning = true;
        }

        private void RetirerLesDroitsdAcces()
        {
            AdminArticle = AdminCommande = AdminUtilisateur = AdminPlanning = false;
        }
        private void MettrelUrlEnSession()
        {
            if (ControllerNom != "Compte" && Request.HttpMethod == "GET")
                Session["Url"] = Request.Url.LocalPath;
            if (Session["Url"] == null)
                Session["Url"] = "~/";
        }
    }
}