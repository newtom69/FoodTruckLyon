using FoodTruck.DAL;
using FoodTruck.Models;
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

            PanierUI lePanier;
            if (Session["Panier"] == null)
                lePanier = new PanierUI();
            else
                lePanier = (PanierUI)Session["Panier"];
            Session["Panier"] = lePanier;
            ViewBag.Panier = lePanier;

            VisiteDAL.Enregistrer(lUtilisateur != null ? lUtilisateur.Id : 0);
            return View();
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

            PanierUI lePanier;
            if (Session["Panier"] == null)
                lePanier = new PanierUI();
            else
                lePanier = (PanierUI)Session["Panier"];

            ArticleDAL lArticleDAL = new ArticleDAL();
            Article lArticle = lArticleDAL.Details(nom);
            if (lArticle == null || !lArticle.DansCarte)
            {
                return RedirectToAction("/");
            }
            else
            {
                PanierDAL lePanierDAL;
                ArticleUI artcl = lePanier.ListeArticlesUI.Find(art => art.Id == lArticle.Id);
                if (artcl == null)
                {
                    ArticleUI articleUI = new ArticleUI(lArticle);
                    lePanier.ListeArticlesUI.Add(articleUI);
                    if (sauvPanier)
                    {
                        lePanierDAL = new PanierDAL(lUtilisateur.Id);
                        lePanierDAL.Ajouter(lArticle);
                    }
                }
                else
                {
                    artcl.Quantite++;
                    if (sauvPanier)
                    {
                        lePanierDAL = new PanierDAL(lUtilisateur.Id);
                        lePanierDAL.ModifierQuantite(lArticle, 1);
                    }
                }
                lePanier.PrixTotal += lArticle.Prix;
                Session["Panier"] = lePanier;
                ViewBag.Panier = lePanier;

                VisiteDAL.Enregistrer(lUtilisateur != null ? lUtilisateur.Id : 0);
                return RedirectToAction("../Article");
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

            PanierUI panierUI;
            if (Session["Panier"] == null)
                panierUI = new PanierUI();
            else
                panierUI = (PanierUI)Session["Panier"];

            ArticleDAL lArticleDAL = new ArticleDAL();
            if (id >= panierUI.ListeArticlesUI.Count)
            {
                return RedirectToAction("../Article/Erreur404");
            }
            else
            {
                Article lArticle = lArticleDAL.Details(panierUI.ListeArticlesUI[id].Id);
                PanierDAL lePanierDAL;
                panierUI.PrixTotal = Math.Round(panierUI.PrixTotal - lArticle.Prix, 2);

                if (panierUI.ListeArticlesUI[id].Quantite > 1)
                {
                    panierUI.ListeArticlesUI[id].Quantite--;
                    if (sauvPanier)
                    {
                        lePanierDAL = new PanierDAL(lUtilisateur.Id);
                        lePanierDAL.ModifierQuantite(lArticle, -1);
                    }
                }
                else
                {
                    panierUI.ListeArticlesUI.RemoveAt(id);
                    if (sauvPanier)
                    {
                        lePanierDAL = new PanierDAL(lUtilisateur.Id);
                        lePanierDAL.Supprimer(lArticle);
                    }
                }

                Session["Panier"] = panierUI;
                ViewBag.Panier = panierUI;

                VisiteDAL.Enregistrer(lUtilisateur != null ? lUtilisateur.Id : 0);
                return RedirectToAction("../Article");
            }
        }
    }
}
