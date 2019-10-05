using FoodTruck.Models;
using System.Web.Mvc;

namespace FoodTruck.Controllers
{
    public class AProposController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.PanierAbsent = false;
            PanierUI lePanier;
            if (this.Session["Panier"] == null) lePanier = new PanierUI();
            else lePanier = (PanierUI)this.Session["Panier"];
            this.Session["Panier"] = lePanier;
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
            PanierUI lePanier;
            if (this.Session["Panier"] == null) lePanier = new PanierUI();
            else lePanier = (PanierUI)this.Session["Panier"];
            this.Session["Panier"] = lePanier;
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