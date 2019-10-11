﻿using FoodTruck.DAL;
using FoodTruck.Models;
using FoodTruck.ViewModels;
using System;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace FoodTruck.Controllers
{
    public class UtilisateurController : Controller
    {
        [HttpGet]
        public ActionResult Profil()
        {
            SessionVariables session = new SessionVariables();
            ViewBag.Panier = session.PanierViewModel;
            ViewBag.Utilisateur = session.Utilisateur;
            return View(session.Utilisateur);
        }

        [HttpGet]
        public ActionResult Connexion()
        {
            SessionVariables session = new SessionVariables();
            ViewBag.Panier = session.PanierViewModel;
            ViewBag.Utilisateur = session.Utilisateur;

            if (session.Utilisateur.Id == 0)
                return View();
            else
                return RedirectToAction("../");
        }

        [HttpPost]
        public ActionResult Connexion(string Email, string Mdp)
        {
            SessionVariables session = new SessionVariables();
            ViewBag.Panier = session.PanierViewModel;

            Utilisateur lUtilisateur;
            UtilisateurDAL lUtilisateurDAL;
            if (session.Utilisateur.Id == 0)
            {
                lUtilisateurDAL = new UtilisateurDAL();
                lUtilisateur = lUtilisateurDAL.Connexion(Email, Mdp);
                HttpCookie cookie = new HttpCookie("Email");
                cookie.Value = lUtilisateur.Email;
                Response.Cookies.Add(cookie);
            }
            else
            {
                lUtilisateur = (Utilisateur)Session["Utilisateur"];
            }
            Session["Utilisateur"] = lUtilisateur;
            ViewBag.Utilisateur = lUtilisateur;

            if (lUtilisateur != null)
            {
                session = new SessionVariables();
                //SynchroniserPanier(lUtilisateur);
                session.SynchroniserPanier();
                ViewBag.Panier = Session["Panier"];
                VisiteDAL.Enregistrer(lUtilisateur.Id);
                return RedirectToAction("../");
            }
            else
            {
                Session["Utilisateur"] = null;
                ViewBag.Utilisateur = null;
                ViewBag.MauvaisEmailMdp = true;
                VisiteDAL.Enregistrer(0);
                return View();
            }
        }

        [HttpGet]
        public ActionResult Deconnexion()
        {
            HttpCookie newCookie = new HttpCookie("Email");
            newCookie.Expires = DateTime.Now.AddDays(-30);
            Response.Cookies.Add(newCookie);

            SessionVariables session = new SessionVariables(0);
            ViewBag.Panier = session.PanierViewModel;
            ViewBag.Utilisateur = session.Utilisateur;
            return View();
        }

        [HttpGet]
        public ActionResult Creation()
        {
            SessionVariables session = new SessionVariables();
            ViewBag.Panier = session.PanierViewModel;
            ViewBag.Utilisateur = session.Utilisateur;
            return View();
        }

        [HttpPost]
        public ActionResult Creation(string Email, string Mdp, string Mdp2, string Nom, string Prenom, string Telephone)
        {
            SessionVariables session = new SessionVariables();
            ViewBag.Panier = session.PanierViewModel;
            ViewBag.Utilisateur = session.Utilisateur;

            Utilisateur lUtilisateur;
            UtilisateurDAL lUtilisateurDAL;
            if (session.Utilisateur.Id == 0)
            {
                lUtilisateurDAL = new UtilisateurDAL();
                if (!VerifMdp(Mdp, Mdp2))
                {
                    ViewBag.MdpIncorrect = true;
                    ViewBag.Nom = Nom;
                    ViewBag.Prenom = Prenom;
                    ViewBag.Email = Email;
                    ViewBag.Telephone = Telephone;
                    return View();
                }
                lUtilisateur = lUtilisateurDAL.Creation(Email, Mdp, Nom, Prenom, Telephone);
            }
            else
            {
                lUtilisateur = session.Utilisateur;
            }
            Session["Utilisateur"] = lUtilisateur;
            ViewBag.Utilisateur = lUtilisateur;
            VisiteDAL.Enregistrer(lUtilisateur != null ? lUtilisateur.Id : 0);
            if (lUtilisateur != null)
            {
                return RedirectToAction($"./Profil");
            }
            else
            {
                Session["Utilisateur"] = null;
                ViewBag.Utilisateur = null;
                ViewBag.MauvaisEmailMdp = true;
                return View();
            }
        }

        bool VerifMdp(string mdp1, string mdp2)
        {
            bool valeurRetour = true;
            if (mdp1 != mdp2) valeurRetour = false;
            if (mdp1.Length < 8) valeurRetour = false;
            return valeurRetour;
        }
    }
}
