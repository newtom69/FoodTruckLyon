using System.Web.Mvc;
using System.Web.Routing;

namespace FoodTruck
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("PanierIndexTest", "Panier/IndexTest", //TODO supprimer
            defaults: new { controller = "Panier", action = "IndexTest" });

            routes.MapRoute("PanierIndex", "Panier/Index",
            defaults: new { controller = "Panier", action = "Index" });

            routes.MapRoute("PanierRetirer", "Panier/Retirer",
            defaults: new { controller = "Panier", action = "Retirer" });

            routes.MapRoute("ArticleAjouterEnBase", "Article/AjouterEnBase/{id}",
            defaults: new { controller = "Article", action = "AjouterEnBase", id = UrlParameter.Optional });

            routes.MapRoute("ArticleDirect", "Article/{nom}",
            defaults: new { controller = "Article", action = "Details" });

            routes.MapRoute("Article", "Article/{action}/{nom}",
            defaults: new { controller = "Article", action = "Index", nom = UrlParameter.Optional });

            routes.MapRoute("Articles", "Articles",
            defaults: new { controller = "Article", action = "Index" });

            routes.MapRoute("PanierAjoutArticleDirect", "Panier/{nom}",
            defaults: new { controller = "Panier", action = "Ajouter" });

            routes.MapRoute("Panier", "Panier/{action}/{nom}",
            defaults: new { controller = "Panier", action = "Ajouter" });

            routes.MapRoute("Compte", "Compte/Profil",
            defaults: new { controller = "Compte", action = "Profil" });

            routes.MapRoute("Default", "{controller}/{action}/{id}",
            defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional });




        }
    }
}
