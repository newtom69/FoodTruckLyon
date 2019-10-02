using FoodTruck.DAL;
using FoodTruck.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FoodTruck.Controllers
{
    public class PanierController : Controller
    {
        // GET: Panier
        public ActionResult Index()
        {
            ViewBag.PanierAbsent = true;

            Utilisateur lUtilisateur=null;
            if (this.Session["Utilisateur"] != null)
            {
                lUtilisateur = (Utilisateur)this.Session["Utilisateur"];
                ViewBag.lUtilisateur = lUtilisateur;
            }

            Panier lePanier;
            if (this.Session["MonPanier"] == null)
                lePanier = new Panier();
            else
                lePanier = (Panier)this.Session["MonPanier"];
            this.Session["MonPanier"] = lePanier;
            ViewBag.Panier = lePanier;

            VisiteController visite = new VisiteController();
            visite.Enregistrer(lUtilisateur != null ? lUtilisateur.Id : 0);

            return View();
        }

        // GET: Panier/Ajouter/5
        public ActionResult Ajouter(int Id)
        {
            bool sauvPanier = false;
            Utilisateur lUtilisateur=null;
            if (this.Session["Utilisateur"] != null)
            {
                lUtilisateur = (Utilisateur)this.Session["Utilisateur"];
                ViewBag.lUtilisateur = lUtilisateur;
                sauvPanier = true;
            }

            Panier lePanier;
            if (this.Session["MonPanier"] == null)
                lePanier = new Panier();
            else
                lePanier = (Panier)this.Session["MonPanier"];

            ArticleDAL lArticleDAL = new ArticleDAL();
            Article lArticle = lArticleDAL.Details(Id);
            PanierDAL lePanierDAL;

            var t = lePanier.listeArticles.Find(art => art.Id == lArticle.Id);
            if (t == null)
            {
                lArticle.Quantite = 1;
                lePanier.listeArticles.Add(lArticle);
                if (sauvPanier)
                {
                    lePanierDAL = new PanierDAL(lUtilisateur.Id);
                    lePanierDAL.Ajouter(lArticle);
                }
            }
            else
            {
                t.Quantite++;
                if (sauvPanier)
                {
                    lePanierDAL= new PanierDAL(lUtilisateur.Id);
                    lePanierDAL.ModifierQuantite(lArticle,1);
                }
            }

            lePanier.PrixTotal += lArticle.Prix;
            this.Session["MonPanier"] = lePanier;
            ViewBag.Panier = lePanier;

            VisiteController visite = new VisiteController();
            visite.Enregistrer(lUtilisateur != null ? lUtilisateur.Id : 0);

            return RedirectToAction("../Article");
        }

        // GET: Panier/Retirer/0
        public ActionResult Retirer(int id)
        {
            bool sauvPanier = false;
            Utilisateur lUtilisateur=null;
            if (this.Session["Utilisateur"] != null)
            {
                lUtilisateur = (Utilisateur)this.Session["Utilisateur"];
                ViewBag.lUtilisateur = lUtilisateur;
                sauvPanier = true;
            }

            Panier lePanier;
            if (this.Session["MonPanier"] == null)
                lePanier = new Panier();
            else
                lePanier = (Panier)this.Session["MonPanier"];

            ArticleDAL lArticleDAL = new ArticleDAL();
            Article lArticle = lArticleDAL.Details(lePanier.listeArticles[id].Id);
            PanierDAL lePanierDAL;
            lePanier.PrixTotal -= lArticle.Prix;

            if (lePanier.listeArticles[id].Quantite > 1)
            {
                lePanier.listeArticles[id].Quantite--;
                if (sauvPanier)
                {
                    lePanierDAL = new PanierDAL(lUtilisateur.Id);
                    lePanierDAL.ModifierQuantite(lArticle, -1);
                }
            }
            else
            {
                lePanier.listeArticles.RemoveAt(id);
                if (sauvPanier)
                {
                    lePanierDAL = new PanierDAL(lUtilisateur.Id);
                    lePanierDAL.Supprimer(lArticle);
                }
            }
                
            this.Session["MonPanier"] = lePanier;
            ViewBag.Panier = lePanier;

            VisiteController visite = new VisiteController();
            visite.Enregistrer(lUtilisateur != null ? lUtilisateur.Id : 0);

            return RedirectToAction("../Article");
        }
    }
}
