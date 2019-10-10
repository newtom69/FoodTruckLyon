using FoodTruck.DAL;
using FoodTruck.Models;
using FoodTruck.ViewModels;
using System;
using System.Web.Mvc;

namespace FoodTruck.Controllers
{
    public class PanierController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.PanierAbsent = true;

            Utilisateur lUtilisateur = null;
            if (Session["Utilisateur"] != null)
            {
                lUtilisateur = (Utilisateur)Session["Utilisateur"];
                ViewBag.lUtilisateur = lUtilisateur;
            }
            VisiteDAL.Enregistrer(lUtilisateur != null ? lUtilisateur.Id : 0);

            if (Session["Panier"] == null)
                return View(new PanierViewModel());
            else
                return View((PanierViewModel)Session["Panier"]);
        }
   
        [HttpPost]
        public ActionResult Ajouter(string nom)
        {
            bool sauvPanier = false;

            Utilisateur lUtilisateur = null;
            if (Session["Utilisateur"] != null)
            {
                lUtilisateur = (Utilisateur)Session["Utilisateur"];
                ViewBag.lUtilisateur = lUtilisateur;
                sauvPanier = true;
            }
            
            PanierViewModel panierViewModel;
            if (Session["Panier"] == null)
                panierViewModel = new PanierViewModel();
            else
                panierViewModel = (PanierViewModel)Session["Panier"];
            Session["Panier"] = panierViewModel;

            ArticleDAL lArticleDAL = new ArticleDAL();
            Article lArticle = lArticleDAL.Details(nom);
            if (lArticle == null || !lArticle.DansCarte)
            {
                return Redirect(Request.UrlReferrer.ToString());
            }
            else
            {
                PanierDAL lePanierDAL;
                ArticleDetailsViewModel artcl = panierViewModel.ArticlesDetailsViewModel.Find(art => art.Article.Id == lArticle.Id);
                if (artcl == null)
                {
                    ArticleDetailsViewModel article = new ArticleDetailsViewModel(lArticle);
                    panierViewModel.ArticlesDetailsViewModel.Add(article);
                    if (sauvPanier)
                    {
                        lePanierDAL = new PanierDAL(lUtilisateur.Id);
                        lePanierDAL.Ajouter(lArticle);
                    }
                }
                else
                {
                    artcl.Quantite++;
                    artcl.PrixTotal = Math.Round(artcl.PrixTotal + artcl.Article.Prix, 2);
                    if (sauvPanier)
                    {
                        lePanierDAL = new PanierDAL(lUtilisateur.Id);
                        lePanierDAL.ModifierQuantite(lArticle, 1);
                    }
                }
                panierViewModel.PrixTotal += lArticle.Prix;

                Session["Panier"] = panierViewModel;
                VisiteDAL.Enregistrer(lUtilisateur != null ? lUtilisateur.Id : 0);
                return Redirect(Request.UrlReferrer.ToString());
            }
        }

        [HttpPost]
        public ActionResult Retirer(int id)
        {
            bool sauvPanier = false;
            Utilisateur lUtilisateur = null;
            if (Session["Utilisateur"] != null)
            {
                lUtilisateur = (Utilisateur)Session["Utilisateur"];
                ViewBag.lUtilisateur = lUtilisateur;
                sauvPanier = true;
            }

            PanierViewModel panierViewModel;
            if (Session["Panier"] == null)
                panierViewModel = new PanierViewModel();
            else
                panierViewModel = (PanierViewModel)Session["Panier"];

            ArticleDAL lArticleDAL = new ArticleDAL();
            if (id >= panierViewModel.ArticlesDetailsViewModel.Count)
            {
                return Redirect(Request.UrlReferrer.ToString());
            }
            else
            {
                Article lArticle = lArticleDAL.Details(panierViewModel.ArticlesDetailsViewModel[id].Article.Id);
                PanierDAL lePanierDAL;
                panierViewModel.PrixTotal = Math.Round(panierViewModel.PrixTotal - lArticle.Prix, 2);

                if (panierViewModel.ArticlesDetailsViewModel[id].Quantite > 1)
                {
                    panierViewModel.ArticlesDetailsViewModel[id].Quantite--;
                    panierViewModel.ArticlesDetailsViewModel[id].PrixTotal = Math.Round(panierViewModel.ArticlesDetailsViewModel[id].PrixTotal - panierViewModel.ArticlesDetailsViewModel[id].Article.Prix, 2);
                    if (sauvPanier)
                    {
                        lePanierDAL = new PanierDAL(lUtilisateur.Id);
                        lePanierDAL.ModifierQuantite(lArticle, -1);
                    }
                }
                else
                {
                    panierViewModel.ArticlesDetailsViewModel.RemoveAt(id);
                    if (sauvPanier)
                    {
                        lePanierDAL = new PanierDAL(lUtilisateur.Id);
                        lePanierDAL.Supprimer(lArticle);
                    }
                }
                Session["Panier"] = panierViewModel;
                VisiteDAL.Enregistrer(lUtilisateur != null ? lUtilisateur.Id : 0);
                return Redirect(Request.UrlReferrer.ToString());
            }
        }
    }
}
