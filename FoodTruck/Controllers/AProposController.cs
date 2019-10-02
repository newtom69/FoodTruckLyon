using FoodTruck.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FoodTruck.Controllers
{
    public class AProposController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.PanierAbsent = false;
            Panier lePanier;
            if (this.Session["MonPanier"] == null) lePanier = new Panier();
            else lePanier = (Panier)this.Session["MonPanier"];
            this.Session["MonPanier"] = lePanier;
            ViewBag.Panier = lePanier;

            Utilisateur lUtilisateur;
            if (this.Session["Utilisateur"] != null)
            {
                lUtilisateur = (Utilisateur)this.Session["Utilisateur"];
                ViewBag.lUtilisateur = lUtilisateur;
            }

            return View();
        }


        public ActionResult Stationnement()
        {
            ViewBag.PanierAbsent = false;
            Panier lePanier;
            if (this.Session["MonPanier"] == null) lePanier = new Panier();
            else lePanier = (Panier)this.Session["MonPanier"];
            this.Session["MonPanier"] = lePanier;
            ViewBag.Panier = lePanier;
            Utilisateur lUtilisateur;
            if (this.Session["Utilisateur"] != null)
            {
                lUtilisateur = (Utilisateur)this.Session["Utilisateur"];
                ViewBag.lUtilisateur = lUtilisateur;
            }

            return View();
        }
    }
}