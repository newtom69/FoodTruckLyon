﻿using FoodTruck.DAL;
using FoodTruck.Models;
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
            session.PanierViewModel.DatesRetraitPossibles = ObtenirDatesRetraitPossibles();
            TempData["PanierLatteralDesactive"] = true;
            return View(session.PanierViewModel);
        }

        [HttpPost]
        public ActionResult Ajouter(string nom)
        {
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
                ArticleViewModel artcl = session.PanierViewModel.ArticlesDetailsViewModel.Find(art => art.Article.Id == lArticle.Id);
                if (artcl == null)
                {
                    ArticleViewModel article = new ArticleViewModel(lArticle);
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
                return Redirect(Request.UrlReferrer.ToString());
            }
        }

        [HttpPost]
        public ActionResult Retirer(int id)
        {
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
                return Redirect(Request.UrlReferrer.ToString());
            }
        }

        private List<DateTime> ObtenirDatesRetraitPossibles()
        {
            int heurePremierCreneauxDejeuner = 0, minutePremierCreneauxDejeuner = 0;
            int heureDernierCreneauxDejeuner = 0, minuteDernierCreneauxDejeuner = 0;
            int heurePremierCreneauxDiner = 0, minutePremierCreneauxDiner = 0;
            int heureDernierCreneauxDiner = 0, minuteDernierCreneauxDiner = 0;
            string[] premierCreneauDejeuner = ConfigurationManager.AppSettings["PremierCreneauDejeuner"].Split(':');
            string[] dernierCreneauDejeuner = ConfigurationManager.AppSettings["DernierCreneauDejeuner"].Split(':');
            string[] premierCreneauDiner = ConfigurationManager.AppSettings["PremierCreneauDiner"].Split(':');
            string[] dernierCreneauDiner = ConfigurationManager.AppSettings["DernierCreneauDiner"].Split(':');
            if (!ObtenirHeureMinute(premierCreneauDejeuner, ref heurePremierCreneauxDejeuner, ref minutePremierCreneauxDejeuner))
            {
                heurePremierCreneauxDejeuner = 12;
                minutePremierCreneauxDejeuner = 0;
            }
            if (!ObtenirHeureMinute(dernierCreneauDejeuner, ref heureDernierCreneauxDejeuner, ref minuteDernierCreneauxDejeuner))
            {
                heureDernierCreneauxDejeuner = 14;
                minuteDernierCreneauxDejeuner = 0;
            }
            if (!ObtenirHeureMinute(premierCreneauDiner, ref heurePremierCreneauxDiner, ref minutePremierCreneauxDiner))
            {
                heurePremierCreneauxDiner = 19;
                minutePremierCreneauxDiner = 0;
            }
            if (!ObtenirHeureMinute(dernierCreneauDiner, ref heureDernierCreneauxDiner, ref minuteDernierCreneauxDiner))
            {
                heureDernierCreneauxDiner = 22;
                minuteDernierCreneauxDiner = 0;
            }

            DateTime maintenant = DateTime.Now;
            int jAnnee = maintenant.Year;
            int jMois = maintenant.Month;
            int jJour = maintenant.Day;

            DateTime maintenantTest8h = new DateTime(jAnnee, jMois, jJour, 8, 0, 0);
            DateTime maintenantTest23h = new DateTime(jAnnee, jMois, jJour, 23, 0, 0);
            DateTime maintenantTest21h = new DateTime(jAnnee, jMois, jJour, 21, 0, 0);
            DateTime maintenantTest19h = new DateTime(jAnnee, jMois, jJour, 19, 2, 0);
            DateTime maintenantTest15h = new DateTime(jAnnee, jMois, jJour, 15, 0, 0);

            //maintenant = maintenantTest19h; //TODO TEST !

            DateTime premierCreneauxDejeuner = new DateTime(jAnnee, jMois, jJour, heurePremierCreneauxDejeuner, minutePremierCreneauxDejeuner, 0);
            DateTime dernierCreneauxDejeuner = new DateTime(jAnnee, jMois, jJour, heureDernierCreneauxDejeuner, minuteDernierCreneauxDejeuner, 0);
            premierCreneauxDejeuner = ObtenirCreneauCourant(premierCreneauxDejeuner);
            dernierCreneauxDejeuner = ObtenirCreneauCourant(dernierCreneauxDejeuner);
            if (dernierCreneauxDejeuner < maintenant)
            {
                premierCreneauxDejeuner = premierCreneauxDejeuner.AddDays(1);
                dernierCreneauxDejeuner = dernierCreneauxDejeuner.AddDays(1);
            }
            if (premierCreneauxDejeuner <= ObtenirCreneauCourant(maintenant))
                premierCreneauxDejeuner = ObtenirCreneauSuivant(maintenant);

            List<DateTime> creneauxDejeuner = ConstruireCreneaux(premierCreneauxDejeuner, dernierCreneauxDejeuner);

            DateTime premierCreneauxDiner = new DateTime(jAnnee, jMois, jJour, heurePremierCreneauxDiner, minutePremierCreneauxDiner, 0);
            DateTime dernierCreneauxDiner = new DateTime(jAnnee, jMois, jJour, heureDernierCreneauxDiner, minuteDernierCreneauxDiner, 0);
            premierCreneauxDiner = ObtenirCreneauCourant(premierCreneauxDiner);
            dernierCreneauxDiner = ObtenirCreneauCourant(dernierCreneauxDiner);
            if (dernierCreneauxDiner < maintenant)
            {
                premierCreneauxDiner = premierCreneauxDiner.AddDays(1);
                dernierCreneauxDiner = dernierCreneauxDiner.AddDays(1);
            }
            if (premierCreneauxDiner <= ObtenirCreneauCourant(maintenant))
                premierCreneauxDiner = ObtenirCreneauSuivant(maintenant);

            List<DateTime> creneauxDiner = ConstruireCreneaux(premierCreneauxDiner, dernierCreneauxDiner);

            if (premierCreneauxDejeuner < premierCreneauxDiner)
            {
                creneauxDejeuner.AddRange(creneauxDiner);
                return creneauxDejeuner;
            }
            else
            {
                creneauxDiner.AddRange(creneauxDejeuner);
                return creneauxDiner;
            }
        }

        private bool ObtenirHeureMinute(string[] heureEtMinute, ref int heure, ref int minute)
        {
            bool retour = true;
            if (heureEtMinute.Length == 2)
            {
                if (!int.TryParse(heureEtMinute[0], out heure))
                    retour = false;
                if (!int.TryParse(heureEtMinute[1], out minute))
                    retour = false;
            }
            else
            {
                retour = false;
            }
            return retour;
        }

        private List<DateTime> ConstruireCreneaux(DateTime premierCreneau, DateTime dernierCreneau)
        {
            DateTime creneauCourant = premierCreneau;
            List<DateTime> creneaux = new List<DateTime>();
            while (creneauCourant <= dernierCreneau)
            {
                creneaux.Add(creneauCourant);
                creneauCourant = ObtenirCreneauSuivant(creneauCourant);
            }
            return creneaux;
        }
        private DateTime ObtenirCreneauCourant(DateTime date)
        {
            int pas = int.Parse(ConfigurationManager.AppSettings["IntervalleCreneaux"]);
            int annee = date.Year;
            int mois = date.Month;
            int jour = date.Day;
            int heure = date.Hour;
            int minute = date.Minute;
            int minuteCreneauCourant = (minute / pas) * pas;
            return new DateTime(annee, mois, jour, heure, minuteCreneauCourant, 0);
        }
        private DateTime ObtenirCreneauSuivant(DateTime date)
        {
            int pas = int.Parse(ConfigurationManager.AppSettings["IntervalleCreneaux"]);
            return ObtenirCreneauCourant(date).AddMinutes(pas);
        }

    }
}
