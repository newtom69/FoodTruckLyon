using FoodTruck.DAL;
using FoodTruck.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FoodTruck.Controllers
{
    public class UtilisateurController : Controller
    {
        public ActionResult Connexion()
        {
            ViewBag.PanierAbsent = false;
            Panier lePanier;
            if (this.Session["MonPanier"] == null) lePanier = new Panier();
            else lePanier = (Panier)this.Session["MonPanier"];
            this.Session["MonPanier"] = lePanier;
            ViewBag.Panier = lePanier;

            if (this.Session["Utilisateur"] == null)
                return View();
            else
                return RedirectToAction("../");
        }

        [HttpPost]
        public ActionResult Connexion(string Email, string Mdp)
        {
            ViewBag.PanierAbsent = false;
            Panier lePanier;
            if (this.Session["MonPanier"] == null) lePanier = new Panier();
            else lePanier = (Panier)this.Session["MonPanier"];
            this.Session["MonPanier"] = lePanier;
            ViewBag.Panier = lePanier;

            Utilisateur lUtilisateur;
            UtilisateurDAL lUtilisateurDAL;
            if (this.Session["Utilisateur"] == null)
            {
                lUtilisateur = new Utilisateur();
                lUtilisateurDAL = new UtilisateurDAL();
                lUtilisateur = lUtilisateurDAL.Connexion(Email, Mdp);
            }
            else
            {
                lUtilisateur = (Utilisateur)this.Session["Utilisateur"];
            }
            this.Session["Utilisateur"] = lUtilisateur;
            ViewBag.lUtilisateur = lUtilisateur;

            if (SynchroniserPanier(lUtilisateur)) 
            {
                return RedirectToAction("../");
            }

            this.Session["Utilisateur"] = null;
            ViewBag.lUtilisateur = null;
            ViewBag.MauvaisEmailMdp = true;
            return View();
        }

        public ActionResult Deconnexion()
        {
            ViewBag.PanierAbsent = true;
            this.Session["Utilisateur"] = null;
            this.Session["MonPanier"] = null;
            return View();
        }

        public ActionResult Detail(int id)
        {
            ViewBag.PanierAbsent = false;
            Panier lePanier;
            if (this.Session["MonPanier"] == null) lePanier = new Panier();
            else lePanier = (Panier)this.Session["MonPanier"];
            this.Session["MonPanier"] = lePanier;
            ViewBag.Panier = lePanier;

            Utilisateur lUtilisateur;

            if (this.Session["Utilisateur"] == null)
                return View();
            else
            {
                lUtilisateur = (Utilisateur)this.Session["Utilisateur"];
            }
            this.Session["Utilisateur"] = lUtilisateur;
            ViewBag.lUtilisateur = lUtilisateur;

            return View();
        }

        public ActionResult Creation()
        {
            ViewBag.PanierAbsent = false;
            Panier lePanier;
            if (this.Session["MonPanier"] == null) lePanier = new Panier();
            else lePanier = (Panier)this.Session["MonPanier"];
            this.Session["MonPanier"] = lePanier;
            ViewBag.Panier = lePanier;

            return View();
        }

        [HttpPost]
        public ActionResult Creation(string Email, string Mdp, string Mdp2, string Nom, string Prenom, string Telephone)
        {
            ViewBag.PanierAbsent = false;
            Panier lePanier;
            if (this.Session["MonPanier"] == null) lePanier = new Panier();
            else lePanier = (Panier)this.Session["MonPanier"];
            this.Session["MonPanier"] = lePanier;
            ViewBag.Panier = lePanier;

            Utilisateur lUtilisateur;
            UtilisateurDAL lUtilisateurDAL;
            if (this.Session["Utilisateur"] == null)
            {
                lUtilisateur = new Utilisateur();
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
                lUtilisateur = (Utilisateur)this.Session["Utilisateur"];
            }
            this.Session["Utilisateur"] = lUtilisateur;
            ViewBag.lUtilisateur = lUtilisateur;
            if (lUtilisateur.Id != 0)
            {
                return RedirectToAction($"./Detail/{lUtilisateur.Id}");
            }
            this.Session["Utilisateur"] = null;
            ViewBag.lUtilisateur = null;
            ViewBag.MauvaisEmailMdp = true;
            return View();
        }

        bool VerifMdp(string mdp1, string mdp2)
        {
            bool valeurRetour = true;
            if (mdp1 != mdp2) valeurRetour = false;
            if (mdp1.Length<8) valeurRetour = false;
            return valeurRetour;
        }

        bool SynchroniserPanier(Utilisateur lUtilisateur)
        {
            if (lUtilisateur != null && lUtilisateur.Id != 0)
            {
                PanierDAL lePanierDal = new PanierDAL(lUtilisateur.Id);
                lePanierDal.Lister();
                Panier lePanier;
                if (this.Session["MonPanier"] == null)
                    lePanier = new Panier();
                else
                    lePanier = (Panier)this.Session["MonPanier"];

                // boucle sur panier local
                foreach (var lArticle in lePanier.listeArticles)
                {
                    var t = lePanierDal.listeArticles.Find(art => art.Id == lArticle.Id);
                    if (t == null)
                        lePanierDal.Ajouter(lArticle);
                    else
                        lePanierDal.ModifierQuantite(lArticle, 1);
                }

                foreach (var lArticle in lePanierDal.listeArticles)
                {
                    var t = lePanier.listeArticles.Find(art => art.Id == lArticle.Id);
                    if (t == null)
                    {
                        lePanier.listeArticles.Add(lArticle);
                        lePanier.PrixTotal += lArticle.Prix * lArticle.Quantite;
                    }
                    else
                    {
                        t.Quantite += lArticle.Quantite;
                        lePanier.PrixTotal += lArticle.Prix * lArticle.Quantite;
                    }
                }

                Session["MonPanier"] = lePanier;
                return true;
            }

            return false;

        }

      
    }
}
