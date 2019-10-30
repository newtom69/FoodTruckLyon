using FoodTruck.DAL;
using System;
using System.Net;
using System.Web.Mvc;

namespace FoodTruck.Controllers
{
    public class AdministrerPlanningController : ControllerParent
    {
        [HttpGet]
        public ActionResult FermeturesExceptionnelles()
        {
            if (AdminPlanning)
                return View(new OuvertureDAL().ListerFutursFermeturesExceptionnelles());
            else
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
        }
        [HttpPost]
        public ActionResult FermeturesExceptionnelles(string action, DateTime dateId, DateTime dateDebut, TimeSpan heureDebut, DateTime dateFin, TimeSpan heureFin)
        {
            if (AdminPlanning)
            {
                DateTime maintenant = DateTime.Now;
                DateTime dateDebutComplete = dateDebut + heureDebut;
                DateTime dateFinComplete = dateFin + heureFin;
                OuvertureDAL ouvertureDAL = new OuvertureDAL();
                if (action != "Supprimer" && (dateFinComplete <= dateDebutComplete || dateDebutComplete < maintenant))
                {
                    ViewBag.DatesIncompatibles = true;
                }
                else
                {
                    JourExceptionnel chevauchement = null;
                    if (action == "Ajouter")
                    {
                        chevauchement = ouvertureDAL.AjouterFermeture(dateDebutComplete, dateFinComplete);
                        if (chevauchement == null)
                            ViewBag.AjouterFermeture = true;
                        else
                            ViewBag.AjouterFermeture = false;
                    }
                    else if (action == "Modifier")
                    {
                        chevauchement = ouvertureDAL.ModifierFermeture(dateId, dateDebutComplete, dateFinComplete);
                        if (chevauchement == null)
                            ViewBag.ModifierFermeture = true;
                        else
                            ViewBag.ModifierFermeture = false;
                    }
                    else if (action == "Supprimer")
                    {
                        if (ouvertureDAL.SupprimerFermeture(dateId))
                            ViewBag.SupprimerFermeture = true;
                        else
                            ViewBag.SupprimerFermeture = false;
                    }
                    ViewBag.Chevauchement = chevauchement;
                }
                return View(ouvertureDAL.ListerFutursFermeturesExceptionnelles());
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
        }

        [HttpGet]
        public ActionResult OuverturesExceptionnelles()
        {
            if (AdminPlanning)
                return View(new OuvertureDAL().ListerFutursOuverturesExceptionnelles());
            else
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
        }

        [HttpPost]
        public ActionResult OuverturesExceptionnelles(string valider, DateTime dateId, DateTime dateDebut, TimeSpan heureDebut, TimeSpan heureFin)
        {
            if (AdminPlanning)
            {
                DateTime maintenant = DateTime.Now;
                DateTime dateDebutComplete = dateDebut + heureDebut;
                DateTime dateFinComplete = dateDebut + heureFin;
                OuvertureDAL ouvertureDAL = new OuvertureDAL();
                if (heureFin <= heureDebut || dateDebutComplete < maintenant)
                {
                    ViewBag.DatesIncompatibles = true;
                }
                else
                {
                    JourExceptionnel chevauchement = null;
                    if (valider == "Ajouter")
                    {
                        chevauchement = ouvertureDAL.AjouterOuverture(dateDebutComplete, dateFinComplete);
                        if (chevauchement == null)
                            ViewBag.AjouterOuverture = true;
                        else
                            ViewBag.AjouterOuverture = false;
                    }
                    else if (valider == "Modifier")
                    {
                        chevauchement = ouvertureDAL.ModifierOuverture(dateId, dateDebutComplete, dateFinComplete);
                        if (chevauchement == null)
                            ViewBag.ModifierOuverture = true;
                        else
                            ViewBag.ModifierOuverture = false;
                    }
                    else if (valider == "Supprimer")
                    {
                        if (ouvertureDAL.SupprimerOuverture(dateId))
                            ViewBag.SupprimerOuverture = true;
                        else
                            ViewBag.SupprimerOuverture = false;
                    }
                    ViewBag.Chevauchement = chevauchement;
                }
                return View(ouvertureDAL.ListerFutursOuverturesExceptionnelles());
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
        }

        [HttpGet]
        public ActionResult OuverturesHebdomadaires()
        {
            if (AdminPlanning)
                return View(new OuvertureHebdomadaireDAL().ListerOuverturesHebdomadaires());
            else
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
        }
    }
}