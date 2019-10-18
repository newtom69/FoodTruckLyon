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
            session.PanierViewModel.DatesPossiblesLivraison = ObtenirDatesPossiblesLivraison();
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

        private List<DateTime> ObtenirDatesPossiblesLivraison()
        {
            const int heurePremierCreneauxDejeuner = 11; //TODO conf
            const int minutePremierCreneauxDejeuner = 45;
            const int heureDernierCreneauxDejeuner = 14; //TODO conf
            const int minuteDernierCreneauxDejeuner = 00;
            const int heurePremierCreneauxDiner = 19; //TODO conf
            const int minutePremierCreneauxDiner = 00;
            const int heureDernierCreneauxDiner = 22; //TODO conf
            const int minuteDernierCreneauxDiner = 00;
            DateTime maintenant = DateTime.Now;
            int jAnnee = maintenant.Year;
            int jMois = maintenant.Month;
            int jJour = maintenant.Day;

            DateTime maintenantTest8h = new DateTime(jAnnee, jMois, jJour, 8, 0, 0);
            DateTime maintenantTest23h = new DateTime(jAnnee, jMois, jJour, 23, 0, 0);
            DateTime maintenantTest21h = new DateTime(jAnnee, jMois, jJour, 21, 0, 0);
            DateTime maintenantTest19h = new DateTime(jAnnee, jMois, jJour, 19, 0, 0);
            DateTime maintenantTest15h = new DateTime(jAnnee, jMois, jJour, 15, 0, 0);

            maintenant = maintenantTest8h; //TODO TEST !
            int jMoins1Annee = maintenant.AddDays(-1).Year;
            int jMoins1Mois = maintenant.AddDays(-1).Month;
            int jMoins1Jour = maintenant.AddDays(-1).Day;

            DateTime maxCommandePourDejeuner = new DateTime(jAnnee, jMois, jJour, 13, 45, 00); //TODO conf
            DateTime minCommandePourDejeuner = new DateTime(jMoins1Annee, jMoins1Mois, jMoins1Jour, 21, 00, 00);
            DateTime minCommandePourDiner = new DateTime(jAnnee, jMois, jJour, 13, 00, 00);
            DateTime maxCommandePourDiner = new DateTime(jAnnee, jMois, jJour, 21, 45, 00);

            DateTime premierCreneauxDejeuner = new DateTime(jAnnee, jMois, jJour, heurePremierCreneauxDejeuner, minutePremierCreneauxDejeuner, 0);
            DateTime dernierCreneauxDejeuner = new DateTime(jAnnee, jMois, jJour, heureDernierCreneauxDejeuner, minuteDernierCreneauxDejeuner, 0);
            premierCreneauxDejeuner = ObtenirCreneauCourant(premierCreneauxDejeuner);
            dernierCreneauxDejeuner = ObtenirCreneauCourant(dernierCreneauxDejeuner);
            if (dernierCreneauxDejeuner < maintenant)
            {
                premierCreneauxDejeuner = premierCreneauxDejeuner.AddDays(1);
                dernierCreneauxDejeuner = dernierCreneauxDejeuner.AddDays(1);
            }
            if (premierCreneauxDejeuner < ObtenirCreneauCourant(maintenant))
            {
                premierCreneauxDejeuner = ObtenirCreneauSuivant(maintenant);
            }
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
            if (premierCreneauxDiner < ObtenirCreneauCourant(maintenant))
            {
                premierCreneauxDiner = ObtenirCreneauSuivant(maintenant);
            }
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

        private DateTime ObtenirCreneauSuivant(DateTime date)
        {
            const int pas = 15; //TODO Conf
            int annee = date.Year;
            int mois = date.Month;
            int jour = date.Day;
            int heure = date.Hour;
            int minute = date.Minute;
            int minuteCreneauCourant = (minute / pas) * pas;
            return new DateTime(annee, mois, jour, heure, minuteCreneauCourant, 0).AddMinutes(pas);
        }
        private DateTime ObtenirCreneauCourant(DateTime date)
        {
            const int pas = 15; //TODO Conf
            int annee = date.Year;
            int mois = date.Month;
            int jour = date.Day;
            int heure = date.Hour;
            int minute = date.Minute;
            int minuteCreneauCourant = (minute / pas) * pas;
            return new DateTime(annee, mois, jour, heure, minuteCreneauCourant, 0);
        }
    }
}
