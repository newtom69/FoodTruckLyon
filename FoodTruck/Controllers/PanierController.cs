using FoodTruck.DAL;
using FoodTruck.Models;
using FoodTruck.ViewModels;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace FoodTruck.Controllers
{
    public class PanierController : ControllerParent
    {
        [HttpGet]
        public ActionResult Index()
        {
            PanierViewModel.DatesRetraitPossibles = ObtenirDatesRetraitPossibles();
            TempData["PanierLatteralDesactive"] = true;
            return View(PanierViewModel);
        }

        [HttpPost]
        public ActionResult Ajouter(string nom, string ancre)
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
            return Redirect(Request.UrlReferrer.AbsolutePath + ancre);
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

        internal bool Ajouter(Article lArticle, int quantite = 1)
        {
            bool ajout;
            bool sauvPanierClient = false;
            bool sauvPanierProspect = false;
            if (Utilisateur.Id != 0)
                sauvPanierClient = true;
            else
                sauvPanierProspect = true;
            PanierDAL lePanierDAL;
            PanierProspectDAL lePanierProspectDAL;
            if (!lArticle.DansCarte)
            {
                ajout = false;
            }
            else
            {
                ArticleViewModel artcl = PanierViewModel.ArticlesDetailsViewModel.Find(art => art.Article.Id == lArticle.Id);
                if (artcl == null)
                {
                    ArticleViewModel article = new ArticleViewModel(lArticle);
                    PanierViewModel.ArticlesDetailsViewModel.Add(article);
                    if (sauvPanierClient)
                    {
                        lePanierDAL = new PanierDAL(Utilisateur.Id);
                        lePanierDAL.Ajouter(lArticle, quantite);
                    }
                    else if (sauvPanierProspect)
                    {
                        lePanierProspectDAL = new PanierProspectDAL(ProspectGuid);
                        lePanierProspectDAL.Ajouter(lArticle, quantite);
                    }
                }
                else
                {
                    artcl.Quantite += quantite;
                    artcl.PrixTotal = Math.Round(artcl.PrixTotal + quantite * artcl.Article.Prix, 2);
                    if (sauvPanierClient)
                    {
                        lePanierDAL = new PanierDAL(Utilisateur.Id);
                        lePanierDAL.ModifierQuantite(lArticle, quantite);
                    }
                    else if (sauvPanierProspect)
                    {
                        lePanierProspectDAL = new PanierProspectDAL(ProspectGuid);
                        lePanierProspectDAL.ModifierQuantite(lArticle, quantite);
                    }
                }
                ajout = true;
            }
            return ajout;
        }

        private List<DateTime> ObtenirDatesRetraitPossibles()
        {
            DateTime maintenant = DateTime.Now;
            List<PlageHoraireRetrait> plagesHorairesRetrait = maintenant.PlageHoraireRetrait();
            List<DateTime> retour = new List<DateTime>();
            foreach (var plage in plagesHorairesRetrait)
            {
                retour.AddRange(plage.Creneaux);
            }
            return retour;
        }
    }
}
