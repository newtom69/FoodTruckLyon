using FoodTruck.Models;
using FoodTruck.ViewModels;
using System.Web.Mvc;

namespace FoodTruck.Controllers
{
    public class AProposController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.PanierAbsent = false;
            PanierViewModel lePanier;
            if (Session["Panier"] == null) lePanier = new PanierViewModel();
            else lePanier = (PanierViewModel)Session["Panier"];
            Session["Panier"] = lePanier;
            ViewBag.Panier = lePanier;

            Utilisateur lUtilisateur;
            if (Session["Utilisateur"] != null)
            {
                lUtilisateur = (Utilisateur)Session["Utilisateur"];
                ViewBag.lUtilisateur = lUtilisateur;
            }

            return View();
        }

        [HttpGet]
        public ActionResult Stationnement()
        {
            ViewBag.PanierAbsent = false;
            PanierViewModel lePanier;
            if (Session["Panier"] == null) lePanier = new PanierViewModel();
            else lePanier = (PanierViewModel)Session["Panier"];
            Session["Panier"] = lePanier;
            ViewBag.Panier = lePanier;
            Utilisateur lUtilisateur;
            if (Session["Utilisateur"] != null)
            {
                lUtilisateur = (Utilisateur)Session["Utilisateur"];
                ViewBag.lUtilisateur = lUtilisateur;
            }

            return View();
        }
    }
}