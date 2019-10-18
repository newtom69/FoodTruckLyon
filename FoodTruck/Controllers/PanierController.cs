using FoodTruck.DAL;
using FoodTruck.Models;
using FoodTruck.ViewModels;
using System;
using System.Web.Mvc;

namespace FoodTruck.Controllers
{
    public class PanierController : ControllerParent
    {
        [HttpGet]
        public ActionResult Index()
        {
            //SessionVariables session = new SessionVariables();
            //VisiteDAL.Enregistrer(session.Utilisateur.Id);
            TempData["PanierLatteralDesactive"] = true;
            return View(session.PanierViewModel);
        }

        [HttpPost]
        public ActionResult Ajouter(string nom)
        {
            //SessionVariables session = new SessionVariables();
            bool sauvPanierClient = false;
            bool sauvPanierProspect = false;
            if (session.Utilisateur.Id != 0)
                sauvPanierClient = true;
            else
                sauvPanierProspect = true;
            ArticleDAL lArticleDAL = new ArticleDAL();
            Article lArticle = lArticleDAL.Details(nom);
            if (lArticle == null || !lArticle.DansCarte)
            {
                return Redirect(Request.UrlReferrer.ToString());
            }
            else
            {
                PanierDAL lePanierDAL;
                PanierProspectDAL lePanierProspectDAL;
                ArticleDetailsViewModel artcl = session.PanierViewModel.ArticlesDetailsViewModel.Find(art => art.Article.Id == lArticle.Id);
                if (artcl == null)
                {
                    ArticleDetailsViewModel article = new ArticleDetailsViewModel(lArticle);
                    session.PanierViewModel.ArticlesDetailsViewModel.Add(article);
                    if (sauvPanierClient)
                    {
                        lePanierDAL = new PanierDAL(session.Utilisateur.Id);
                        lePanierDAL.Ajouter(lArticle);
                    }
                    else if (sauvPanierProspect)
                    {
                        lePanierProspectDAL = new PanierProspectDAL(session.ProspectGuid);
                        lePanierProspectDAL.Ajouter(lArticle);
                    }
                }
                else
                {
                    artcl.Quantite++;
                    artcl.PrixTotal = Math.Round(artcl.PrixTotal + artcl.Article.Prix, 2);
                    if (sauvPanierClient)
                    {
                        lePanierDAL = new PanierDAL(session.Utilisateur.Id);
                        lePanierDAL.ModifierQuantite(lArticle, 1);
                    }
                    else if (sauvPanierProspect)
                    {
                        lePanierProspectDAL = new PanierProspectDAL(session.ProspectGuid);
                        lePanierProspectDAL.ModifierQuantite(lArticle, 1);
                    }
                }
                session.PanierViewModel.PrixTotal += lArticle.Prix;
                Session["Panier"] = session.PanierViewModel;
                //VisiteDAL.Enregistrer(session.Utilisateur.Id);
                return Redirect(Request.UrlReferrer.ToString());
            }
        }

        [HttpPost]
        public ActionResult Retirer(int id)
        {
            //SessionVariables session = new SessionVariables();
            bool sauvPanierClient = false;
            bool sauvPanierProspect = false;
            if (session.Utilisateur.Id != 0)
                sauvPanierClient = true;
            else
                sauvPanierProspect = true;

            ArticleDAL lArticleDAL = new ArticleDAL();
            if (id >= session.PanierViewModel.ArticlesDetailsViewModel.Count)
            {
                return Redirect(Request.UrlReferrer.ToString());
            }
            else
            {
                Article lArticle = lArticleDAL.Details(session.PanierViewModel.ArticlesDetailsViewModel[id].Article.Id);
                PanierDAL lePanierDAL;
                PanierProspectDAL lePanierProspectDAL;
                session.PanierViewModel.PrixTotal = Math.Round(session.PanierViewModel.PrixTotal - lArticle.Prix, 2);

                if (session.PanierViewModel.ArticlesDetailsViewModel[id].Quantite > 1)
                {
                    session.PanierViewModel.ArticlesDetailsViewModel[id].Quantite--;
                    session.PanierViewModel.ArticlesDetailsViewModel[id].PrixTotal = Math.Round(session.PanierViewModel.ArticlesDetailsViewModel[id].PrixTotal - session.PanierViewModel.ArticlesDetailsViewModel[id].Article.Prix, 2);
                    if (sauvPanierClient)
                    {
                        lePanierDAL = new PanierDAL(session.Utilisateur.Id);
                        lePanierDAL.ModifierQuantite(lArticle, -1);
                    }
                    else if (sauvPanierProspect)
                    {
                        lePanierProspectDAL = new PanierProspectDAL(session.ProspectGuid);
                        lePanierProspectDAL.ModifierQuantite(lArticle, -1);
                    }
                }
                else
                {
                    session.PanierViewModel.ArticlesDetailsViewModel.RemoveAt(id);
                    if (sauvPanierClient)
                    {
                        lePanierDAL = new PanierDAL(session.Utilisateur.Id);
                        lePanierDAL.Supprimer(lArticle);
                    }
                    else if (sauvPanierProspect)
                    {
                        lePanierProspectDAL = new PanierProspectDAL(session.ProspectGuid);
                        lePanierProspectDAL.Supprimer(lArticle);
                    }
                }
                Session["Panier"] = session.PanierViewModel;
                //VisiteDAL.Enregistrer(session.Utilisateur.Id);
                return Redirect(Request.UrlReferrer.ToString());
            }
        }
    }
}
