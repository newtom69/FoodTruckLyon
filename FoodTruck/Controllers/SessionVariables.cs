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
            if (HttpContext.Current.Session["Utilisateur"] != null)
                Utilisateur = (Utilisateur)HttpContext.Current.Session["Utilisateur"];

            if (HttpContext.Current.Session["Panier"] == null)
                HttpContext.Current.Session["Panier"] = PanierViewModel = new PanierViewModel();
            else
                PanierViewModel = (PanierViewModel)HttpContext.Current.Session["Panier"];
        }
    }
}