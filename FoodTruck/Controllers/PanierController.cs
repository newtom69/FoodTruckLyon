using FoodTruck.DAL;
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
            int heurePremierCreneauDejeuner = 0, minutePremierCreneauDejeuner = 0;
            int heureDernierCreneauDejeuner = 0, minuteDernierCreneauDejeuner = 0;
            int heurePremierCreneauDiner = 0, minutePremierCreneauDiner = 0;
            int heureDernierCreneauDiner = 0, minuteDernierCreneauDiner = 0;
            string[] tabpremierCreneauDejeuner = ConfigurationManager.AppSettings["PremierCreneauDejeuner"].Split(':');
            string[] tabdernierCreneauDejeuner = ConfigurationManager.AppSettings["DernierCreneauDejeuner"].Split(':');
            string[] tabpremierCreneauDiner = ConfigurationManager.AppSettings["PremierCreneauDiner"].Split(':');
            string[] tabdernierCreneauDiner = ConfigurationManager.AppSettings["DernierCreneauDiner"].Split(':');
            int pas = int.Parse(ConfigurationManager.AppSettings["IntervalleCreneaux"]);
            TimeSpan pasTimeSpan = new TimeSpan(0, pas, 0);
            if (!ObtenirHeureMinute(tabpremierCreneauDejeuner, ref heurePremierCreneauDejeuner, ref minutePremierCreneauDejeuner))
            {
                heurePremierCreneauDejeuner = 12;
                minutePremierCreneauDejeuner = 0;
            }
            if (!ObtenirHeureMinute(tabdernierCreneauDejeuner, ref heureDernierCreneauDejeuner, ref minuteDernierCreneauDejeuner))
            {
                heureDernierCreneauDejeuner = 14;
                minuteDernierCreneauDejeuner = 0;
            }
            if (!ObtenirHeureMinute(tabpremierCreneauDiner, ref heurePremierCreneauDiner, ref minutePremierCreneauDiner))
            {
                heurePremierCreneauDiner = 19;
                minutePremierCreneauDiner = 0;
            }
            if (!ObtenirHeureMinute(tabdernierCreneauDiner, ref heureDernierCreneauDiner, ref minuteDernierCreneauDiner))
            {
                heureDernierCreneauDiner = 22;
                minuteDernierCreneauDiner = 0;
            }
            DateTime maintenant = DateTime.Now;


            maintenant = new DateTime(2019, 10, 25, 23, 0, 0); //TODO TEST


            int maintenantAnnee = maintenant.Year;
            int maintenantMois = maintenant.Month;
            int maintenantJour = maintenant.Day;


            DateTime premierCreneauDejeunerJ = new DateTime(maintenantAnnee, maintenantMois, maintenantJour, heurePremierCreneauDejeuner, minutePremierCreneauDejeuner, 0);
            DateTime dernierCreneauDejeunerJ = new DateTime(maintenantAnnee, maintenantMois, maintenantJour, heureDernierCreneauDejeuner, minuteDernierCreneauDejeuner, 0);
            DateTime premierCreneauDinerJ = new DateTime(maintenantAnnee, maintenantMois, maintenantJour, heurePremierCreneauDiner, minutePremierCreneauDiner, 0);
            DateTime dernierCreneauDinerJ = new DateTime(maintenantAnnee, maintenantMois, maintenantJour, heureDernierCreneauDiner, minuteDernierCreneauDiner, 0);
            premierCreneauDejeunerJ = ObtenirCreneauCourant(premierCreneauDejeunerJ);
            dernierCreneauDejeunerJ = ObtenirCreneauCourant(dernierCreneauDejeunerJ);
            premierCreneauDinerJ = ObtenirCreneauCourant(premierCreneauDinerJ);
            dernierCreneauDinerJ = ObtenirCreneauCourant(dernierCreneauDinerJ);

            PlageHoraireRetrait plageHoraireRetraitDejeunerJ = new PlageHoraireRetrait(premierCreneauDejeunerJ, dernierCreneauDejeunerJ, pasTimeSpan);
            PlageHoraireRetrait plageHoraireRetraitDinerJ = new PlageHoraireRetrait(premierCreneauDinerJ, dernierCreneauDinerJ, pasTimeSpan);
            PlageHoraireRetrait plageHoraireRetraitRepas1;
            PlageHoraireRetrait plageHoraireRetraitRepas2;

            DateTime jourOuvert = maintenant;
            OuvertureDAL ouvertureDAL = new OuvertureDAL();
            if (plageHoraireRetraitDinerJ.Avant(maintenant))
            {
                jourOuvert = maintenant.AddDays(1);
            }
            while (!ouvertureDAL.EstOuvert(jourOuvert, 1))
            {
                jourOuvert = jourOuvert.AddDays(1);
            }
            if (jourOuvert != maintenant)
            {
                jourOuvert = new DateTime(jourOuvert.Year, jourOuvert.Month, jourOuvert.Day, 0, 0, 0);

                plageHoraireRetraitRepas1 = new PlageHoraireRetrait(heurePremierCreneauDejeuner, minutePremierCreneauDejeuner, heureDernierCreneauDejeuner, minuteDernierCreneauDejeuner, pasTimeSpan, jourOuvert.Year, jourOuvert.Month, jourOuvert.Day);
                plageHoraireRetraitRepas2 = new PlageHoraireRetrait(heurePremierCreneauDiner, minutePremierCreneauDiner, heureDernierCreneauDiner, minuteDernierCreneauDiner, pasTimeSpan, jourOuvert.Year, jourOuvert.Month, jourOuvert.Day);
            }
            else
            {
                if (plageHoraireRetraitDejeunerJ.Contient(jourOuvert) || plageHoraireRetraitDejeunerJ.Apres(jourOuvert))
                {
                    // dejeuner
                    plageHoraireRetraitRepas1 = new PlageHoraireRetrait(heurePremierCreneauDejeuner, minutePremierCreneauDejeuner, heureDernierCreneauDejeuner, minuteDernierCreneauDejeuner, pasTimeSpan, jourOuvert.Year, jourOuvert.Month, jourOuvert.Day);
                    //TODO heure minutes premier creneau si jour J
                    plageHoraireRetraitRepas2 = new PlageHoraireRetrait(heurePremierCreneauDiner, minutePremierCreneauDiner, heureDernierCreneauDiner, minuteDernierCreneauDiner, pasTimeSpan, jourOuvert.Year, jourOuvert.Month, jourOuvert.Day);
                }
                else
                {
                    // diner 
                    plageHoraireRetraitRepas1 = new PlageHoraireRetrait(heurePremierCreneauDiner, minutePremierCreneauDiner, heureDernierCreneauDiner, minuteDernierCreneauDiner, pasTimeSpan, jourOuvert.Year, jourOuvert.Month, jourOuvert.Day);
                    //TODO heure mintutes premier creneau sur jour J
                    plageHoraireRetraitRepas2 = new PlageHoraireRetrait(heurePremierCreneauDejeuner, minutePremierCreneauDejeuner, heureDernierCreneauDejeuner, minuteDernierCreneauDejeuner, pasTimeSpan, jourOuvert.Year, jourOuvert.Month, jourOuvert.Day);
                    plageHoraireRetraitRepas2 = plageHoraireRetraitRepas2.AddDays(1);
                    while (!ouvertureDAL.EstOuvert(plageHoraireRetraitRepas2.PremierCreneau, plageHoraireRetraitRepas2.TypeRepas))
                    {
                        plageHoraireRetraitRepas2 = plageHoraireRetraitRepas2.AddDays(1);
                    }
                }

            }
            List<DateTime> retour = new List<DateTime>();
            retour.AddRange(plageHoraireRetraitRepas1.Creneaux);
            retour.AddRange(plageHoraireRetraitRepas2.Creneaux);
            return retour;
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
