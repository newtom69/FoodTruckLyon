using FoodTruck.DAL;
using FoodTruck.Models;
using FoodTruck.ViewModels;
using System.Web;
using System.Web.Mvc;

namespace FoodTruck.Controllers
{
    public class SessionVariables
    {
        public Utilisateur Utilisateur { get; set; }
        public PanierViewModel PanierViewModel { get; set; }

        public SessionVariables()
        {
            if (HttpContext.Current.Session["Utilisateur"] == null)
            {
                HttpCookie cookie = HttpContext.Current.Request.Cookies.Get("Email");
                if (cookie != null)
                {
                    UtilisateurDAL utilisateurDAL = new UtilisateurDAL();
                    HttpContext.Current.Session["Utilisateur"] = Utilisateur = utilisateurDAL.ConnexionCookies(cookie.Value);
                }
                else
                    HttpContext.Current.Session["Utilisateur"] = Utilisateur = new Utilisateur();
            }
            else
                Utilisateur = (Utilisateur)HttpContext.Current.Session["Utilisateur"];

            if (HttpContext.Current.Session["Panier"] == null)
                HttpContext.Current.Session["Panier"] = PanierViewModel = new PanierViewModel();
            else
            {
                PanierViewModel = (PanierViewModel)HttpContext.Current.Session["Panier"];
                PanierViewModel.Trier();
            }
        }
        public SessionVariables(int pourSurchargeUniquement)
        {
            HttpContext.Current.Session["Utilisateur"] = Utilisateur = new Utilisateur();
            HttpContext.Current.Session["Panier"] = PanierViewModel = new PanierViewModel();
        }
    }
}