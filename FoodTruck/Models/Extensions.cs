using FoodTruck.DAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FoodTruck.Models
{
    public static class Extensions
    {
        public static List<PlageHoraireRetrait> PlageHoraireRetrait(this DateTime date)
        {
            OuvertureDAL ouvertureDAL = new OuvertureDAL();
            List<PlageHoraireRetrait> plagesHorairesRetrait = new List<PlageHoraireRetrait>();
            PlageHoraireRetrait plage1 = ouvertureDAL.ProchainOuvert(date);
            PlageHoraireRetrait plage2 = ouvertureDAL.ProchainOuvert(plage1.Creneaux.Last().AddMinutes(1)); //TODO refaire algo pour ne pas avoir à rajouter 1 mn
            plagesHorairesRetrait.Add(plage1);
            plagesHorairesRetrait.Add(plage2);
            return plagesHorairesRetrait;
        }
        public static string UrlVersNom(this string url)
        {
            return url.Replace("-", " ").Replace("_", "-");
        }
        public static string ToUrl(this string nom)
        {
            return nom.TrimEnd(' ').Replace("-", "_").Replace(" ", "-");
        }
        public static string NomAdmis(this string nom)
        {
            const char espace = ' ';
            const string interdit = "@=&#_;%^";
            return nom.Replace(interdit, espace).Replace(Path.GetInvalidFileNameChars(), espace);
        }
        public static string Replace(this string orig, string to, char by)
        {
            foreach (char car in to)
            {
                orig = orig.Replace(car, by);
            }
            return orig;
        }
        public static string Replace(this string orig, char[] to, char by)
        {
            foreach (char car in to)
            {
                orig = orig.Replace(car, by);
            }
            return orig;
        }

        public static string OuiNon(this bool lebool)
        {
            if (lebool)
                return "Oui";
            else
                return "Non";
        }
    }
}