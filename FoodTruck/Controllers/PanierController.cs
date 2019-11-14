﻿using FoodTruck.DAL;
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
            ViewBag.PanierLatteralDesactive = true;
            return View(PanierViewModel);
        }

        [HttpPost]
        public ActionResult Index(string codePromo)
        {
            TempData["CodePromo"] = codePromo;
            TempData["RemiseCommercialeValide"] = false;
            TempData["RemiseCommercialeMontant"] = (double)0;
            ValiditeCodePromo code = new CodePromoDAL().Validite(codePromo, PanierViewModel.PrixTotal, out double montantRemise);
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
            return Redirect("~/Panier/Index#codePromoUtilisateur");
        }

        [HttpPost]
        public ActionResult Ajouter(string nom, string ancre, bool? home)
        {
            Article article = new ArticleDAL().Details(nom);
            if (article != null && article.DansCarte)
            {
                PanierViewModel.Ajouter(article, 1, Utilisateur.Id, ProspectGuid);
                ViewBag.Panier = PanierViewModel;
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
            bool sauvPanierClient = Utilisateur.Id != 0 ? true : false;
            if (id < PanierViewModel.ArticlesDetailsViewModel.Count)
            {
                Article article = new ArticleDAL().Details(PanierViewModel.ArticlesDetailsViewModel[id].Article.Id);
                PanierDAL panierDAL;
                PanierProspectDAL panierProspectDAL;
                PanierViewModel.PrixTotal = Math.Round(PanierViewModel.PrixTotal - article.Prix, 2);

                if (PanierViewModel.ArticlesDetailsViewModel[id].Quantite > 1)
                {
                    PanierViewModel.ArticlesDetailsViewModel[id].Quantite--;
                    PanierViewModel.ArticlesDetailsViewModel[id].PrixTotal = Math.Round(PanierViewModel.ArticlesDetailsViewModel[id].PrixTotal - PanierViewModel.ArticlesDetailsViewModel[id].Article.Prix, 2);
                    if (sauvPanierClient)
                    {
                        panierDAL = new PanierDAL(Utilisateur.Id);
                        panierDAL.ModifierQuantite(article, -1);
                    }
                    else
                    {
                        panierProspectDAL = new PanierProspectDAL(ProspectGuid);
                        panierProspectDAL.ModifierQuantite(article, -1);
                    }
                }
                else
                {
                    PanierViewModel.ArticlesDetailsViewModel.RemoveAt(id);
                    if (sauvPanierClient)
                    {
                        panierDAL = new PanierDAL(Utilisateur.Id);
                        panierDAL.Supprimer(article);
                    }
                    else
                    {
                        panierProspectDAL = new PanierProspectDAL(ProspectGuid);
                        panierProspectDAL.Supprimer(article);
                    }
                }
                ViewBag.Panier = PanierViewModel;
            }
            return Redirect(Request.UrlReferrer.AbsolutePath);
        }
    }
}
