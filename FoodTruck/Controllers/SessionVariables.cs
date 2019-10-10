using FoodTruck.Models;
using FoodTruck.ViewModels;
using System.Web;

namespace FoodTruck.Controllers
{
    public class SessionVariables
    {
        public Utilisateur Utilisateur { get; set; }
        public PanierViewModel PanierViewModel { get; set; }

        public SessionVariables()
        {
            if (HttpContext.Current.Session["Utilisateur"] == null)
                HttpContext.Current.Session["Utilisateur"] = Utilisateur = new Utilisateur();
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
        public SessionVariables(int iPourSurcharge)
        {
                HttpContext.Current.Session["Utilisateur"] = Utilisateur = new Utilisateur();
                HttpContext.Current.Session["Panier"] = PanierViewModel = new PanierViewModel();
        }
    }
}