using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace FoodTruck.Extensions
{
    public static class Extensions
    {
        public static string OuiNon(this bool lebool)
        {
            if (lebool)
                return "Oui";
            else
                return "Non";
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
    }
}