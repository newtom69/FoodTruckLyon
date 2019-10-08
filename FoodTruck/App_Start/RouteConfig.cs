using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace FoodTruck
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("PanierIndex", "Panier/Index",
            defaults: new { controller = "Panier", action = "Index" });

            routes.MapRoute("PanierRetirer", "Panier/Retirer/{id}",
            defaults: new { controller = "Panier", action = "Retirer" });

            routes.MapRoute("ArticleAjouterEnBase", "Article/AjouterEnBase/{id}",
            defaults: new { controller = "Article", action = "AjouterEnBase", id = UrlParameter.Optional });

            routes.MapRoute("ArticleDirect", "Article/{nom}",
            defaults: new { controller = "Article", action = "Details" });

            routes.MapRoute("PanierAjoutArticleDirect", "Panier/{nom}",
            defaults: new { controller = "Panier", action = "Ajouter" });

            routes.MapRoute("Articles", "Article/{action}/{nom}",
            defaults: new { controller = "Article", action = "Index", nom = UrlParameter.Optional });

            routes.MapRoute("Panier", "Panier/{action}/{nom}",
            defaults: new { controller = "Panier", action = "Ajouter" });

            routes.MapRoute("Default", "{controller}/{action}/{id}",
            defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional });




        }
    }
}
