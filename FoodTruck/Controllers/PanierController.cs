using FoodTruck.DAL;
using FoodTruck.Models;
using FoodTruck.Outils;
using FoodTruck.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;

namespace FoodTruck.Controllers
{
    public class PanierController : ControllerParent
    {
        [HttpGet]
        public ActionResult Index()
        {
            DateTime maintenant = DateTime.Now;
            List<PlageHoraireRetrait> plagesHorairesRetrait = maintenant.PlageHoraireRetrait();

            // obtention nombre de commandes à retirer dans chaque creneaux ouvert et desactivation si = nombre max
            int maxCommandesHeure = int.Parse(ConfigurationManager.AppSettings["NombreDeCommandesMaxParHeure"]);
            CommandeDAL commandeDAL = new CommandeDAL();

            PanierViewModel.Creneaux = new List<Creneau>();
            foreach (PlageHoraireRetrait plage in plagesHorairesRetrait)
            {
                int maxCommandesCreneau = (int)Math.Ceiling(maxCommandesHeure * plage.Pas.TotalMinutes / 60);
                foreach (DateTime date in plage.Dates)
                {
                    Creneau creneau = new Creneau
                    {
                        DateRetrait = date,
                        CommandesPossiblesRestantes = maxCommandesCreneau - commandeDAL.NombreCommandes(date)
                    };
                    PanierViewModel.Creneaux.Add(creneau);
                }
            }
            TempData["PanierLatteralDesactive"] = true;
            return View(PanierViewModel);
        }

        [HttpPost]
        public ActionResult Index(string codePromo)
        {
            TempData["CodePromo"] = codePromo;
            TempData["RemiseCommercialeValide"] = false;
            TempData["RemiseCommercialeMontant"] = (double)0;
            CodePromoDAL codePromoDAL = new CodePromoDAL();
            ValiditeCodePromo code = codePromoDAL.Validite(codePromo, PanierViewModel.PrixTotal, out double montantRemise);
            switch (code)
            {
                case ValiditeCodePromo.Valide:
                    TempData["RemiseCommercialeValide"] = true;
                    TempData["RemiseCommercialeInfo"] = "code valide";
                    TempData["RemiseCommercialeMontant"] = montantRemise;
                    break;
                case ValiditeCodePromo.Inconnu:
                    TempData["RemiseCommercialeInfo"] = "code inconnu";
                    break;
                case ValiditeCodePromo.DateDepassee:
                    TempData["RemiseCommercialeInfo"] = "ce code n'est plus valable";
                    break;
                case ValiditeCodePromo.DateFuture:
                    TempData["RemiseCommercialeInfo"] = "ce code n'est pas encore valable";
                    break;
                case ValiditeCodePromo.MontantInsuffisant:
                    TempData["RemiseCommercialeInfo"] = "le montant de la commande est insuffisant";
                    break;
            }
            return Redirect("~/Panier/Index#remiseFidelite");
        }

        [HttpPost]
        public ActionResult Ajouter(string nom, string ancre, bool? home)
        {
            ArticleDAL lArticleDAL = new ArticleDAL();
            Article lArticle = lArticleDAL.Details(nom);
            if (lArticle != null && lArticle.DansCarte)
            {
                if (Ajouter(lArticle))
                {
                    PanierViewModel.PrixTotal += lArticle.Prix;
                    ViewBag.Panier = PanierViewModel;
                }
            }
            bool testHome = home ?? false;
            if (!testHome)
                return Redirect(Request.UrlReferrer.AbsolutePath + ancre);
            else
                return Redirect("/Article" + ancre);
        }

        [HttpPost]
        public ActionResult Retirer(int id)
        {
            bool sauvPanierClient = false;
            bool sauvPanierProspect = false;
            if (Utilisateur.Id != 0)
                sauvPanierClient = true;
            else
                sauvPanierProspect = true;

            ArticleDAL lArticleDAL = new ArticleDAL();
            if (id < PanierViewModel.ArticlesDetailsViewModel.Count)
            {
                Article lArticle = lArticleDAL.Details(PanierViewModel.ArticlesDetailsViewModel[id].Article.Id);
                PanierDAL lePanierDAL;
                PanierProspectDAL lePanierProspectDAL;
                PanierViewModel.PrixTotal = Math.Round(PanierViewModel.PrixTotal - lArticle.Prix, 2);

                if (PanierViewModel.ArticlesDetailsViewModel[id].Quantite > 1)
                {
                    PanierViewModel.ArticlesDetailsViewModel[id].Quantite--;
                    PanierViewModel.ArticlesDetailsViewModel[id].PrixTotal = Math.Round(PanierViewModel.ArticlesDetailsViewModel[id].PrixTotal - PanierViewModel.ArticlesDetailsViewModel[id].Article.Prix, 2);
                    if (sauvPanierClient)
                    {
                        lePanierDAL = new PanierDAL(Utilisateur.Id);
                        lePanierDAL.ModifierQuantite(lArticle, -1);
                    }
                    else if (sauvPanierProspect)
                    {
                        lePanierProspectDAL = new PanierProspectDAL(ProspectGuid);
                        lePanierProspectDAL.ModifierQuantite(lArticle, -1);
                    }
                }
                else
                {
                    PanierViewModel.ArticlesDetailsViewModel.RemoveAt(id);
                    if (sauvPanierClient)
                    {
                        lePanierDAL = new PanierDAL(Utilisateur.Id);
                        lePanierDAL.Supprimer(lArticle);
                    }
                    else if (sauvPanierProspect)
                    {
                        lePanierProspectDAL = new PanierProspectDAL(ProspectGuid);
                        lePanierProspectDAL.Supprimer(lArticle);
                    }
                }
                ViewBag.Panier = PanierViewModel;
            }
            return Redirect(Request.UrlReferrer.AbsolutePath);
        }

        internal bool Ajouter(Article article, int quantite = 1)
        {
            bool ajout;
            bool sauvPanierClient = false;
            bool sauvPanierProspect = false;
            if (Utilisateur.Id != 0)
                sauvPanierClient = true;
            else
                sauvPanierProspect = true;
            PanierDAL panierDAL;
            PanierProspectDAL panierProspectDAL;
            if (!article.DansCarte)
            {
                ajout = false;
            }
            else
            {
                ArticleViewModel artcl = PanierViewModel.ArticlesDetailsViewModel.Find(art => art.Article.Id == article.Id);
                if (artcl == null)
                {
                    ArticleViewModel articleViewModel = new ArticleViewModel(article);
                    PanierViewModel.ArticlesDetailsViewModel.Add(articleViewModel);
                    if (sauvPanierClient)
                    {
                        panierDAL = new PanierDAL(Utilisateur.Id);
                        panierDAL.Ajouter(article, quantite);
                    }
                    else if (sauvPanierProspect)
                    {
                        panierProspectDAL = new PanierProspectDAL(ProspectGuid);
                        panierProspectDAL.Ajouter(article, quantite);
                    }
                }
                else
                {
                    artcl.Quantite += quantite;
                    artcl.PrixTotal = Math.Round(artcl.PrixTotal + quantite * artcl.Article.Prix, 2);
                    if (sauvPanierClient)
                    {
                        panierDAL = new PanierDAL(Utilisateur.Id);
                        panierDAL.ModifierQuantite(article, quantite);
                    }
                    else if (sauvPanierProspect)
                    {
                        panierProspectDAL = new PanierProspectDAL(ProspectGuid);
                        panierProspectDAL.ModifierQuantite(article, quantite);
                    }
                }
                ajout = true;
            }
            return ajout;
        }
    }
}
