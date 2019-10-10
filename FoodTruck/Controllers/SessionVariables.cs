using FoodTruck.Models;
using FoodTruck.ViewModels;
using System.Web;

namespace FoodTruck.Controllers
{   public class SessionVariables
    {
        private HttpContextBase Context { get; set; }
        public Utilisateur Utilisateur;
        public PanierViewModel PanierViewModel;

        public SessionVariables(HttpContextBase context)
        {
            Context = context;

            if (Context.Session["Utilisateur"] != null) Utilisateur = (Utilisateur)Context.Session["Utilisateur"];

            if (Context.Session["Panier"] == null)
                Context.Session["Panier"] = PanierViewModel = new PanierViewModel();
            else
                PanierViewModel = (PanierViewModel)Context.Session["Panier"];
        }
    }
}