using System;
using System.Collections.Generic;
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
        public static string NomPourUrl(this string nom)
        {
            return nom.TrimEnd(' ').Replace("-", "_").Replace(" ", "-");
        }
    }
}