﻿using FoodTruck.Models;
using System;
using System.Configuration;
using System.Linq;

namespace FoodTruck.DAL
{
    class VisiteDAL
    {
        public VisiteDAL(Visite laVisite)
        {
            using (dbEntities db = new dbEntities())
            {
                db.Visite.Add(laVisite);
                db.SaveChanges();
            }
        }

        public static void Enregistrer(int lUtilisateurId)
        {
            string adresseIP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            string ipsNonTracees = ConfigurationManager.AppSettings["ListIpDoNotTrack"];
            string[] tabIpsNonTracees = ipsNonTracees.Split(';');
            if (!tabIpsNonTracees.Any(ip => adresseIP == ip))
            {
                string url = System.Web.HttpContext.Current.Request.Url.ToString();
                string navigateur = System.Web.HttpContext.Current.Request.Browser.Browser;
                string UrlOrigine = "";
                if (System.Web.HttpContext.Current.Request.UrlReferrer != null)
                    UrlOrigine = System.Web.HttpContext.Current.Request.UrlReferrer.ToString();
                Visite laVisite = new Visite
                {
                    Url = url,
                    DateTimeVisite = DateTime.Now,
                    AdresseIp = adresseIP,
                    UtilisateurId = lUtilisateurId,
                    Navigateur = navigateur,
                    UrlOrigine = UrlOrigine
                };
                using (dbEntities db = new dbEntities())
                {
                    db.Visite.Add(laVisite);
                    db.SaveChanges();
                }
            }
        }
    }
}
