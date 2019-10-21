using FoodTruck.DAL;
using FoodTruck.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web;
using System.Web.Routing;

namespace FoodTruck.Controllers
{
    public class ControllerParent : Controller
    {
        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            if (Session["UtilisateurId"] != null)
                ViewBag.Utilisateur = new UtilisateurDAL().Details((int)Session["UtilisateurId"]);
            else
                ViewBag.Utilisateur = null;
        }

        protected SessionVariables VariablesSession;
        public ControllerParent()
        {
            VariablesSession = new SessionVariables();
            VisiteDAL.Enregistrer(VariablesSession.Utilisateur.Id);

            if (VariablesSession.Utilisateur.AdminSuper)
            {
                ViewBag.AdminArticle = true;
                ViewBag.AdminCommande = true;
                ViewBag.AdminUtilisateur = true;
            }
            else
            {
                if (VariablesSession.Utilisateur.AdminArticle) ViewBag.AdminArticle = true;
                else ViewBag.AdminArticle = false;
                if (VariablesSession.Utilisateur.AdminCommande) ViewBag.AdminCommande = true;
                else ViewBag.AdminCommande = false;
                if (VariablesSession.Utilisateur.AdminUtilisateur) ViewBag.AdminUtilisateur = true;
                else ViewBag.AdminUtilisateur = false;
            }

            if (VariablesSession.Utilisateur.Id != 0)
            {
                PanierDAL panierDAL = new PanierDAL(VariablesSession.Utilisateur.Id);
                VariablesSession.PanierViewModel = new PanierViewModel(panierDAL.ListerPanierUtilisateur());
            }
            else
            {
                PanierProspectDAL panierProspectDAL = new PanierProspectDAL(VariablesSession.ProspectGuid);
                VariablesSession.PanierViewModel = new PanierViewModel(panierProspectDAL.ListerPanierProspect());
            }
            VariablesSession.PanierViewModel.Trier();
            ViewBag.Panier = VariablesSession.PanierViewModel;
        }
    }
}