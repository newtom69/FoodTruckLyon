using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace FoodTruck.Extensions
{
    public static class Extensions
    {
        public static string UrlVersNom(this string url)
        {
            return url.Replace("-", " ").Replace("_", "-");
        }
        public static string ToUrl(this string nom)
        {
            return nom.TrimEnd(' ').Replace("-", "_").Replace(" ", "-");
        }

        public static string FormatAutoriseNom(this string nom)
        {
            const string espace = " "; 
            nom = nom.Replace("_", espace).Replace("&", espace).Replace("@", espace).Replace("#", espace).Replace("?", espace);
            return string.Join(espace, nom.Split(Path.GetInvalidFileNameChars()));
        }


    }
}