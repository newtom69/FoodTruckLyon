using FoodTruck.DAL;
using FoodTruck.Models;
using System;
using System.Web.Mvc;

namespace FoodTruck.Controllers
{
    public class PanierController : Controller
    {
        // GET: Panier
        public ActionResult Index()
        {
            ViewBag.PanierAbsent = true;

            Utilisateur lUtilisateur = null;
            if (this.Session["Utilisateur"] != null)
            {
                lUtilisateur = (Utilisateur)this.Session["Utilisateur"];
                ViewBag.lUtilisateur = lUtilisateur;
            }

            PanierUI lePanier;
            if (this.Session["Panier"] == null)
                lePanier = new PanierUI();
            else
                lePanier = (PanierUI)this.Session["Panier"];
            this.Session["Panier"] = lePanier;
            ViewBag.Panier = lePanier;

            using (VisiteController visite = new VisiteController())
            {
                visite.Enregistrer(lUtilisateur != null ? lUtilisateur.Id : 0);
            }

            return View();
        }

        // GET: Panier/Ajouter/5
        public ActionResult Ajouter(int Id)
        {
            bool sauvPanier = false;
            Utilisateur lUtilisateur = null;
            if (this.Session["Utilisateur"] != null)
            {
                lUtilisateur = (Utilisateur)this.Session["Utilisateur"];
                ViewBag.lUtilisateur = lUtilisateur;
                sauvPanier = true;
            }

            PanierUI lePanier;
            if (this.Session["Panier"] == null)
                lePanier = new PanierUI();
            else
                lePanier = (PanierUI)this.Session["Panier"];

            ArticleDAL lArticleDAL = new ArticleDAL();
            Article lArticle = lArticleDAL.Details(Id);
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
            this.Session["Panier"] = lePanier;
            ViewBag.Panier = lePanier;

            using (VisiteController visite = new VisiteController())
            {
                visite.Enregistrer(lUtilisateur != null ? lUtilisateur.Id : 0);
            }

            return RedirectToAction("../Article");
        }

        // GET: Panier/Retirer/0
        public ActionResult Retirer(int id)
        {
            bool sauvPanier = false;
            Utilisateur lUtilisateur = null;
            if (this.Session["Utilisateur"] != null)
            {
                lUtilisateur = (Utilisateur)this.Session["Utilisateur"];
                ViewBag.lUtilisateur = lUtilisateur;
                sauvPanier = true;
            }

            PanierUI panierUI;
            if (this.Session["Panier"] == null)
                panierUI = new PanierUI();
            else
                panierUI = (PanierUI)Session["Panier"];

            ArticleDAL lArticleDAL = new ArticleDAL();
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

            this.Session["Panier"] = panierUI;
            ViewBag.Panier = panierUI;

            using (VisiteController visite = new VisiteController())
            {
                visite.Enregistrer(lUtilisateur != null ? lUtilisateur.Id : 0);
            }

            return RedirectToAction("../Article");
        }
    }
}
