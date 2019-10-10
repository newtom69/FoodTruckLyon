﻿using FoodTruck.DAL;
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
            SessionVariables session = new SessionVariables();
            ViewBag.Panier = session.PanierViewModel;
            ViewBag.Utilisateur = session.Utilisateur;
            VisiteDAL.Enregistrer(session.Utilisateur.Id);
            TempData["PanierLatteralDesactive"] = true;
            return View(session.PanierViewModel);
        }

        [HttpPost]
        public ActionResult Ajouter(string nom)
        {
            SessionVariables session = new SessionVariables();
            ViewBag.Panier = session.PanierViewModel;
            ViewBag.Utilisateur = session.Utilisateur;

            bool sauvPanier = false;
            if (session.Utilisateur.Id != 0)
                sauvPanier = true;

            ArticleDAL lArticleDAL = new ArticleDAL();
            Article lArticle = lArticleDAL.Details(nom);
            if (lArticle == null || !lArticle.DansCarte)
            {
                return Redirect(Request.UrlReferrer.ToString());
            }
            else
            {
                PanierDAL lePanierDAL;
                ArticleDetailsViewModel artcl = session.PanierViewModel.ArticlesDetailsViewModel.Find(art => art.Article.Id == lArticle.Id);
                if (artcl == null)
                {
                    ArticleDetailsViewModel article = new ArticleDetailsViewModel(lArticle);
                    session.PanierViewModel.ArticlesDetailsViewModel.Add(article);
                    if (sauvPanier)
                    {
                        lePanierDAL = new PanierDAL(session.Utilisateur.Id);
                        lePanierDAL.Ajouter(lArticle);
                    }
                }
                else
                {
                    artcl.Quantite++;
                    artcl.PrixTotal = Math.Round(artcl.PrixTotal + artcl.Article.Prix, 2);
                    if (sauvPanier)
                    {
                        lePanierDAL = new PanierDAL(session.Utilisateur.Id);
                        lePanierDAL.ModifierQuantite(lArticle, 1);
                    }
                }
                session.PanierViewModel.PrixTotal += lArticle.Prix;
                Session["Panier"] = session.PanierViewModel;

                VisiteDAL.Enregistrer(session.Utilisateur.Id);
                return Redirect(Request.UrlReferrer.ToString());
            }
        }

        [HttpPost]
        public ActionResult Retirer(int id)
        {
            SessionVariables session = new SessionVariables();
            ViewBag.Panier = session.PanierViewModel;
            ViewBag.Utilisateur = session.Utilisateur;

            bool sauvPanier = false;
            if (session.Utilisateur.Id != 0)
                sauvPanier = true;




            ArticleDAL lArticleDAL = new ArticleDAL();
            if (id >= session.PanierViewModel.ArticlesDetailsViewModel.Count)
            {
                return Redirect(Request.UrlReferrer.ToString());
            }
            else
            {
                Article lArticle = lArticleDAL.Details(session.PanierViewModel.ArticlesDetailsViewModel[id].Article.Id);
                PanierDAL lePanierDAL;
                session.PanierViewModel.PrixTotal = Math.Round(session.PanierViewModel.PrixTotal - lArticle.Prix, 2);

                if (session.PanierViewModel.ArticlesDetailsViewModel[id].Quantite > 1)
                {
                    session.PanierViewModel.ArticlesDetailsViewModel[id].Quantite--;
                    session.PanierViewModel.ArticlesDetailsViewModel[id].PrixTotal = Math.Round(session.PanierViewModel.ArticlesDetailsViewModel[id].PrixTotal - session.PanierViewModel.ArticlesDetailsViewModel[id].Article.Prix, 2);
                    if (sauvPanier)
                    {
                        lePanierDAL = new PanierDAL(session.Utilisateur.Id);
                        lePanierDAL.ModifierQuantite(lArticle, -1);
                    }
                }
                else
                {
                    session.PanierViewModel.ArticlesDetailsViewModel.RemoveAt(id);
                    if (sauvPanier)
                    {
                        lePanierDAL = new PanierDAL(session.Utilisateur.Id);
                        lePanierDAL.Supprimer(lArticle);
                    }
                }
                Session["Panier"] = session.PanierViewModel;
                VisiteDAL.Enregistrer(session.Utilisateur.Id);
                return Redirect(Request.UrlReferrer.ToString());
            }
        }
    }
}
